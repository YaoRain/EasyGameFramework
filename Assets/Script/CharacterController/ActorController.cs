using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {
    private Transform model;
    private Animator anim;
    private MoveController moveController;
    private Rigidbody rb;
    private OnGroundSensor gSensor;
    private OnMonsterSensor mSensor;
    private Transform camera;

    private float jumpFoward;
    public float jumpSpeed = 0;
    private Vector3 jumpSpeedV = new Vector3 (0, 0, 0);

    public bool isAtkAnim = false;
    public bool isOnPendulum = false;

    public PlayerInfo playerInfo;

    private void Awake () {
        moveController = this.GetComponent<MoveController> ();
        model = this.transform.Find ("Girl");
        anim = model.GetComponent<Animator> ();
        rb = this.GetComponent<Rigidbody> ();
        gSensor = this.GetComponentInChildren<OnGroundSensor> ();
        mSensor = this.GetComponentInChildren<OnMonsterSensor> ();
        camera = this.transform.Find("Camera").Find("Camera");
        EventCenter.Instance.AddEventListener ("enterAtkAnim", EnterAtkAnim);
        EventCenter.Instance.AddEventListener ("exitAtkAnim", ExitAtkAnim);
        EventCenter.Instance.AddEventListener ("onPendulum", OnPendulum);
        EventCenter.Instance.AddEventListener ("exitPendulum", ExitPendulum);
        playerInfo = this.GetComponent<PlayerInfo> ();
    }

    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    private void FixedUpdate () {
        SwitchAnim ();
        Move ();
    }

    private void SwitchAnim () {
        anim.SetFloat ("MoveSingle", moveController.moveSingle);
        // 切换跳跃和下落
        if ((moveController.isJump && gSensor.isOnGround) ||(rb.velocity.y > 0.3&& !gSensor.isOnGround) ) { // 更加严格的检测，避免动画切换延迟
            jumpFoward = Mathf.Lerp (jumpFoward, 1.0f, 0.5f);
        }
        else if (rb.velocity.y < -0.8 && !gSensor.isOnGround) { // 速度向下且不在地面上
            mSensor.enabled = true; // 下落开启踩怪检测
            jumpFoward = Mathf.Lerp (jumpFoward, -1.0f, 0.25f);
            if (mSensor.isOnMonster) { // 如果是下落并且碰撞到怪物,则跳跃
                jumpFoward = 1.0f;
            }
        }else{
            jumpFoward = Mathf.Lerp (jumpFoward, 0.0f, 0.25f);
        }
        if (gSensor.isOnGround) {
            mSensor.enabled = false; // 落地后关闭踩怪检测
            mSensor.isOnMonster = false;
        }
        anim.SetBool ("IsOnGround", gSensor.isOnGround);
        anim.SetFloat ("JumpSpeed", jumpFoward);
        anim.SetBool ("IsJump", moveController.isJump);

        // 翻滚
        if (moveController.isRoll || rb.velocity.y < -10.0f) { // 按下翻滚键，或从高空落下
            anim.SetTrigger ("Roll");
            moveController.isRoll = false;
        }

        // 攻击
        if (moveController.isAtk && !isAtkAnim) {
            anim.SetTrigger ("Atk");
        }
    }

    private void Move () {
        if (moveController.rotaEnable == false) { // 如果速度向量被锁，时刻检测是不是在跳跃动画中已经落地，如果是，就解锁速度向量
            if (moveController.isJumpAnim && gSensor.isOnGround && rb.velocity.y < 0) {
                moveController.EnableRota (null); // 如果这个时候再起跳，将不会有方向锁定
            }
        }
        // 如果moveSingle为0的话，modelForward为0向量，角色面向没有方向的向量会出问题，会自动回到初始位置
        if (moveController.moveSingle > 0.1) {
            // 用插值的办法，解决角色转动过于唐突的问题
            model.transform.forward = Vector3.Slerp (model.transform.forward, moveController.modelForward, 0.25f); // 角色相对于父物体旋转
        }

        // 旋转镜头
        if (Mathf.Abs (moveController.rMoveSingle) > 0.1) {
            this.transform.RotateAround (this.transform.position, Vector3.up, moveController._rotaSpeed * moveController.rMoveSingle * Time.fixedDeltaTime);
        }
        if (Mathf.Abs(moveController.hMoveSingle) > 0.1) {
            
                camera.Rotate(new Vector3(-moveController.hMoveSingle*moveController._rotaSpeed*Time.fixedDeltaTime,0,0),Space.Self);
            camera.eulerAngles = new Vector3(Mathf.Clamp(camera.eulerAngles.x,10,35),camera.eulerAngles.y,camera.eulerAngles.z);
        }
        // 跳跃
        if (moveController.isJump && gSensor.isOnGround) {
            //if(moveController.isJumpAnim == false) // 如果已经在跳跃动画中了，说明已经起跳，不可再给冲量
            Jump ();
        }
        // 离开摆球
        if (moveController.isJump && isOnPendulum) {
            ExitPendulum (null);
        }
        // 踩到怪物后跳跃
        if (mSensor.isOnMonster && rb.velocity.y <= 0) {
            Jump ();
            moveController.ClearSpeed (); // 踩到怪后清空目前的速度
            mSensor.transform.Find ("StarBurst2D").GetComponent<ParticleSystem> ().Play ();
            rb.velocity = jumpSpeedV;
            jumpSpeedV.y = 0;
            moveController.rotaEnable = true; // 解锁转向和移动
        }

        Vector3 planeMove; // 在xz平面上的移动
        if (moveController.isOnRollAnim) { // 翻滚
            planeMove = model.transform.forward.normalized * moveController._runSpeed * 0.7f;
            rb.velocity = new Vector3 (planeMove.x, rb.velocity.y, planeMove.z);
        } else if (!mSensor.isOnMonster) { // 不在怪物身上的平面移动
            planeMove = moveController.modelForward * moveController._runSpeed; // 描述平面上的运动
            rb.velocity = new Vector3 (planeMove.x, rb.velocity.y, planeMove.z);
            rb.velocity += jumpSpeedV; // 跳跃
            jumpSpeedV.y = 0;
            //if(!gSensor.isOnGround) jumpSpeedV.y = 0;
        }

        // 攻击
        if (moveController.isAtk && !isAtkAnim) {
            Atk ();
        }
    }
    private void Jump () {
        if (rb.velocity.y < -0.1f) { // 起跳前让速度归零，防止起跳后速度仍然为负
            rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z); // 接触到地面后，此处的执行比动画状态机的检测要早
        }                                                                // 所以如果在落地前按下跳跃，则动画状态机检测不到速度为负的情况
        jumpSpeedV.y = jumpSpeed;
        moveController.isJump = false;
        // moveController.isJump = false;
        // Invoke("AnimJump",0.3f);
    }
    // 为了配合动画效果写的测试方法
    public void AnimJump () {
        if (rb.velocity.y < -0.1f) { // 起跳前让速度归零，防止起跳后速度仍然为负
            rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
        }
        jumpSpeedV.y = jumpSpeed;
        moveController.isJump = false;
        Debug.Log (jumpSpeedV.y + "rb.y: " + rb.velocity.y);
    }

    private void Atk () {
        moveController.isAtk = false;
        EventCenter.Instance.TiggerEvent ("Atk", this.gameObject);
    }
    private void Step () {

    }
    private void OnPendulum (object obj) {
        isOnPendulum = true;
        GameObject pendulum = (GameObject) obj;
        rb.useGravity = false;
        moveController.ClearSpeed ();
        rb.velocity = Vector3.zero;
        this.transform.SetParent (pendulum.transform);
        moveController.inputEnable = false;
    }
    private void ExitPendulum (object obj) {
        moveController.isJump = false;
        PendulumDynamic pendulum = this.transform.parent.GetComponent<PendulumDynamic> ();
        this.transform.up = Vector3.up;
        moveController.modelForward = model.forward;
        //model.forward.normalized* pendulum.state_vector.y*pendulum.rod_length*2; // 把摆球角速度转化为线速度
        rb.useGravity = true;
        moveController.inputEnable = true;
        this.transform.parent = null;
        isOnPendulum = false;
    }
    public void EnterAtkAnim (object obj) {
        this.transform.Find ("Girl/Hip/ULeg_L_/DLeg_L_/Foot_L_/HitColl").GetComponent<CapsuleCollider> ().enabled = true;
        isAtkAnim = true;
        Debug.Log ("isAnim");
    }
    public void ExitAtkAnim (object obj) {
        this.transform.Find ("Girl/Hip/ULeg_L_/DLeg_L_/Foot_L_/HitColl").GetComponent<CapsuleCollider> ().enabled = false;
        isAtkAnim = false;
    }
}