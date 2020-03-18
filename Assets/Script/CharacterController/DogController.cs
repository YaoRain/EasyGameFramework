using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DogController : MonoBehaviour {
    // Start is called before the first frame update
    Animator dogAnimator;
    Rigidbody rb;
    Collider coll;
    public float _runSpeed = 4f;
    public float _horzSpeed = 2f;
    public float _rotaSpeed = 30f;
    public float _backSpeed = 2f;
    public float _vSpeed = 0; // 用来存放实际的前后方向的速度
    public float _jumpSpeed = 4f;
    public float hightOfGround = 0;

    public Vector3 vJumpSpeed = new Vector3 (0, 0, 0);
    public bool isRunning = false, isAtk = false,
        isMoveLeft = false, isMoveRight = false,
        isTurnLeft = false, isTurnRight = false,
        isGoBack = false, isJump = false; // 这些变量表示现在正在按下什么按键
    private bool isAtkAnim = false; // 是否正在播放攻击动画，用来实现攻击硬直
    public bool isOnGroud = true;

    public float targetZMoveSingle = 0; // 前后轴向的输入信号
    public float targetXMoveSingle = 0; // 水平轴向的输入信号
    public float targetRMoveSingle = 0; // 旋转的输入信号
    public float transTime = 0.1f; // 平滑函数的过渡时间
    public float zMoveSingle = 0, xMoveSingle = 0, rMoveSingle = 0; // 平滑函数计算出的信号
    private float ZTempSingle,XTempSingle,RTempSingle; // 临时变量

    RaycastHit hit; // 存储射线检测的详细信息

    void Start () {
        dogAnimator = this.GetComponent<Animator> ();
            rb = this.transform.GetComponent<Rigidbody> ();
        //rb = this.transform.Find("Body").GetComponent<Rigidbody> (); 
        EventCenter.Instance.AddEventListener ("keyKeep", KeyKeepDogAction);
        EventCenter.Instance.AddEventListener ("keyUp", KeyUpDogAction);
        EventCenter.Instance.AddEventListener ("keyDown", KeyDownDogAction);
    }

    private void FixedUpdate () {
        GetHight();
        isAtkAnim = dogAnimator.GetBool ("isAtk1") || dogAnimator.GetBool ("isAtk2");
        SwitchAnim ();
        if (!isAtkAnim) {
            MoveS ();
        }
        if (isAtk && isOnGroud) {
            StopStopBattleAnim ();
            Atk ();
            StopBattleAnim ();
        }
    }

    // 碰撞检测
    private void OnCollisionEnter (Collision other) {
        isOnGroud = true;
    }

    private void GetHight(){ // 获取角色跳跃高度
        // 增加射线高度，避免射线初始位置低于地板
        bool rayTouchGround = Physics.Raycast(transform.position+0.2f*Vector3.up, -Vector3.up,out hit,3);
        if(rayTouchGround){
            hightOfGround = hit.distance;
            if(hightOfGround > 0.5f){
                isOnGroud = false;
            }else{
                isOnGroud = true;
            }
        }
        else{
            isOnGroud = false;
            Debug.Log("rayNotTouch");
            Debug.DrawLine(transform.position,transform.position-Vector3.up,Color.blue,3);
        }
    }

    // 处理角色在平面维度的移动
    private void MoveS () {
        targetZMoveSingle = (isRunning ? 1.0f : 0f) - (isGoBack ? 1.0f : 0f);
        targetXMoveSingle = (isMoveRight ? 1.0f : 0f) - (isMoveLeft ? 1.0f : 0f);
        targetRMoveSingle = (isTurnRight ? 1.0f : 0f) - (isTurnLeft ? 1.0f : 0f);

        zMoveSingle = Mathf.SmoothDamp (zMoveSingle, targetZMoveSingle, ref ZTempSingle, transTime);
        xMoveSingle = Mathf.SmoothDamp (xMoveSingle, targetXMoveSingle, ref XTempSingle, transTime);
        rMoveSingle = Mathf.SmoothDamp (rMoveSingle, targetRMoveSingle, ref RTempSingle, transTime);

        if (zMoveSingle > 0) {
            _vSpeed = _runSpeed;
        } else {
            _vSpeed = _backSpeed;
        }
        this.transform.Translate (new Vector3 (xMoveSingle * _horzSpeed, 0, zMoveSingle * _vSpeed) * Time.fixedDeltaTime);
        this.transform.Rotate (new Vector3 (0, rMoveSingle * _rotaSpeed, 0) * Time.deltaTime);
        Jump ();
    }

    // 切换角色动画
    private void SwitchAnim () {
        dogAnimator.SetBool ("isRunning", isRunning);
        dogAnimator.SetBool ("isWalk", isTurnLeft || isTurnRight || isMoveLeft || isMoveRight || isGoBack);
        if (isAtk && isOnGroud) {
            if (dogAnimator.GetBool ("isAtk1") && isAtk) {
                dogAnimator.SetBool ("isAtk2", true);
            } else {
                dogAnimator.SetBool ("isAtk1", true);
            }
        }
        dogAnimator.SetBool ("isJump", !isOnGroud); // 按下跳跃键或者不在地面上
        dogAnimator.SetFloat("hightOfGround", hightOfGround);

        // 根据平面移动的信号量，控制角色动画混合树
        if(zMoveSingle > 0){
            dogAnimator.SetFloat("runSpeed",zMoveSingle+Mathf.Abs(xMoveSingle)*0.5f); // 向前混合水平移动
        } else{
            // 向后混合水平移动
            if(Mathf.Abs(zMoveSingle)*0.5f +Mathf.Abs(xMoveSingle)*0.5f > 0.5f){ 
                dogAnimator.SetFloat("runSpeed",0.5f); 
            }
            else{
                dogAnimator.SetFloat("runSpeed",Mathf.Abs(zMoveSingle)*0.5f +Mathf.Abs(xMoveSingle)*0.5f);
            }
        }
        
    }

    // 攻击
    public void Atk () {
        isAtk = false;
    }

    // 跳跃
    public void Jump () {
        if (isJump && isOnGroud) {
            vJumpSpeed.y = _jumpSpeed;
            rb.velocity = vJumpSpeed;
            isJump = false;
        }
    }

    // 检测按键按下
    private void KeyDownDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.J) {
            isAtk = true;
        }
        if (key == KeyCode.Space) {
            isJump = true;
        }
    }
    // 检测按键抬起
    private void KeyUpDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.W) {
            isRunning = false;
        }
        if (key == KeyCode.A) {
            isMoveLeft = false;
        }
        if (key == KeyCode.D) {
            isMoveRight = false;
        }
        if (key == KeyCode.Q) {
            isTurnLeft = false;
        }
        if (key == KeyCode.E) {
            isTurnRight = false;
        }
        if (key == KeyCode.J) {
            isAtk = false;
        }
        if (key == KeyCode.S) {
            isGoBack = false;
        }
        if (key == KeyCode.Space) {
            isJump = false;
        }
    }

    private void KeyKeepDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.W) {
            isRunning = true;
        }
        if (key == KeyCode.A) {
            isMoveLeft = true;
        }
        if (key == KeyCode.D) {
            isMoveRight = true;
        }
        if (key == KeyCode.Q) {
            isTurnLeft = true;
        }
        if (key == KeyCode.E) {
            isTurnRight = true;
        }
        if (key == KeyCode.S) {
            isGoBack = true;
        }
    }

    void StopStopBattleAnim () {
        StopCoroutine ("GoBattle");
        StopCoroutine ("StopBattle");
    }

    void StopBattleAnim () {
        StartCoroutine ("GoBattle");
        StartCoroutine ("StopBattle");
    }

    IEnumerator StopBattle () {
        yield return new WaitForSeconds (3f);
        dogAnimator.SetBool ("isBattle", false);
    }
    IEnumerator GoBattle () // 从第一段攻击回到备战状态
    {
        yield return new WaitForSeconds (0.35f);
        dogAnimator.SetBool ("isBattle", true);
    }

    // 动画事件
    void EndAtk1 () {
        dogAnimator.SetBool ("isAtk1", false);
    }
    void EndAtk2 () {
        dogAnimator.SetBool ("isAtk2", false);
    }
}