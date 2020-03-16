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
    public float _vSpeed = 0;
    public float _jumpSpeed = 4f;
    public float _gravity = -40f;
    public Vector3 vJumpSpeed = new Vector3(0,0,0);
    public bool isRunning = false, isAtk = false,
        isMoveLeft = false, isMoveRight = false,
        isTurnLeft = false, isTurnRight = false,
        isGoBack = false, isJump = false; // 这些变量表示现在正在按下什么按键
    private bool isAtkAnim = false; // 是否正在播放攻击动画，用来实现攻击硬直
    public bool isOnGroud = true;

    public float zMoveSingle = 0; // 前后轴向的输入信号
    public float xMoveSingle = 0; // 水平轴向的输入信号
    public float rMoveSigle = 0; // 旋转的输入信号
    public float yMoveSigle = 0; // 垂直方向的输入信号

    void Start () {
        dogAnimator = this.GetComponent<Animator> ();
        rb = this.transform.GetComponent<Rigidbody> ();
        EventCenter.Instance.AddEventListener ("keyKeep", KeyKeepDogAction);
        EventCenter.Instance.AddEventListener ("keyUp", KeyUpDogAction);
        EventCenter.Instance.AddEventListener ("keyDown", KeyDownDogAction);
        //Physics.gravity = new Vector3(0,_gravity,0);
    }

    private void FixedUpdate () {
        isAtkAnim = dogAnimator.GetBool("isAtk1") || dogAnimator.GetBool("isAtk2");
        SwitchAnim ();
        if (!isAtkAnim) {
            MoveS ();
        }
        if (isAtk&&isOnGroud) {
            StopStopBattleAnim ();
            Atk ();
            StopBattleAnim ();
        }
    }

    // private void OnTriggerEnter(Collider other) {
    //     isOnGroud = true;
    // }

    private void OnCollisionEnter(Collision other) {
        isOnGroud = true;
    }

    private void MoveS () {
        zMoveSingle = (isRunning?1.0f : 0f) - (isGoBack?1.0f : 0f);
        xMoveSingle = (isMoveRight?1.0f : 0f) - (isMoveLeft?1.0f : 0f);
        rMoveSigle = (isTurnRight?1.0f : 0f) - (isTurnLeft?1.0f : 0f);

        if (zMoveSingle > 0) {
            _vSpeed = _runSpeed;
        } else {
            _vSpeed = _backSpeed;
        }
        this.transform.Translate (new Vector3 (xMoveSingle * _horzSpeed, 0, zMoveSingle * _vSpeed) * Time.deltaTime);
        this.transform.Rotate (new Vector3 (0, rMoveSigle * _rotaSpeed, 0) * Time.deltaTime);
        Jump();
    }

    // Update is called once per frame

    private void SwitchAnim () {
        dogAnimator.SetBool ("isRunning", isRunning);
        dogAnimator.SetBool ("isWalk", isTurnLeft || isTurnRight || isMoveLeft || isMoveRight || isGoBack);
        if (isAtk&&isOnGroud) {
            if (dogAnimator.GetBool ("isAtk1") && isAtk) {
                dogAnimator.SetBool ("isAtk2", true);
            } else {
                dogAnimator.SetBool ("isAtk1", true);
            }
        }
        dogAnimator.SetBool ("isJump", isJump||!isOnGroud); // 按下跳跃键或者不在地面上
    }

    public void Atk () {
        isAtk = false;
    }

    public void Jump(){
        if(isJump&&isOnGroud){
            vJumpSpeed.y = _jumpSpeed;
            rb.velocity = vJumpSpeed;
            isJump = false;
            isOnGroud = false;
        }
    }

    private void KeyDownDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.J) {
            isAtk = true;
        }
        if (key == KeyCode.Space) {
            isJump = true;
        }
    }

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
        if (key == KeyCode.Space){
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