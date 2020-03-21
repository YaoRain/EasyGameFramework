using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {
    private Transform model;
    private Animator anim;
    private MoveController moveController;
    private Rigidbody rb;
    private OnGroundSensor gSensor;
    
    private float jumpFoward;
    public float jumpSpeed = 0;
    private Vector3 jumpSpeedV = new Vector3 (0, 0, 0);

    public bool isAtkAnim = false;

    public PlayerInfo playerInfo;

    private void Awake () {
        moveController = this.GetComponent<MoveController> ();
        model = this.transform.Find ("Girl");
        anim = model.GetComponent<Animator> ();
        rb = this.GetComponent<Rigidbody> ();
        gSensor = this.GetComponentInChildren<OnGroundSensor> ();
        this.transform.Find("Girl/Hip/ULeg_L_/DLeg_L_/Foot_L_").GetComponent<CapsuleCollider>().enabled = false;
        EventCenter.Instance.AddEventListener("enterAtkAnim",EnterAtkAnim);
        EventCenter.Instance.AddEventListener("exitAtkAnim",ExitAtkAnim);
        playerInfo = this.GetComponent<PlayerInfo>();
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
        if (moveController.isJump) {
            anim.SetTrigger ("Jump");
        }

        // 切换跳跃和下落
        if (rb.velocity.y > 0.5) {
            jumpFoward = 1.0f;
        } else if (rb.velocity.y < -0.8) {
            jumpFoward = Mathf.Lerp (jumpFoward, -1.0f, 0.25f);
        } else {
            jumpFoward = Mathf.Lerp (jumpFoward, 0, 0.25f);
        }
        anim.SetBool ("IsOnGround", gSensor.isOnGround);
        anim.SetFloat ("JumpSpeed", jumpFoward);

        // 翻滚
        if (moveController.isRoll || rb.velocity.y < -10.0f) {
            anim.SetTrigger ("Roll");
            moveController.isRoll = false;
        }

        // 攻击
        if (moveController.isAtk&&!isAtkAnim) {
            anim.SetTrigger("Atk");
        }
    }

    private void Move () {
        // 如果moveSingle为0的话，modelForward为0向量，角色面向没有方向的向量会出问题，会自动回到初始位置
        if (moveController.moveSingle > 0.1) {
            // 用插值的办法，解决角色转动过于唐突的问题
            model.transform.forward = Vector3.Slerp (model.transform.forward, moveController.modelForward, 0.25f); // 角色相对于父物体旋转
        }

        // 旋转镜头
        if (Mathf.Abs (moveController.rMoveSingle) > 0.1) {
            this.transform.RotateAround (this.transform.position, Vector3.up, moveController._rotaSpeed * moveController.rMoveSingle * Time.fixedDeltaTime);
        }
        // 跳跃
        if (moveController.isJump && gSensor.isOnGround) {
            Jump ();
        }
        Vector3 planeMove;
        if (moveController.isOnRollAnim) { // 翻滚
            planeMove = model.transform.forward.normalized * moveController._runSpeed *0.7f;
            rb.velocity = new Vector3(planeMove.x, rb.velocity.y, planeMove.z);
        } else { // 平面移动
            planeMove = moveController.modelForward * moveController._runSpeed; // 描述平面上的运动
            rb.velocity = new Vector3(planeMove.x, rb.velocity.y, planeMove.z);
            rb.velocity += jumpSpeedV; // 跳跃
            jumpSpeedV.y = 0;
        }

        // 攻击
        if(moveController.isAtk&&!isAtkAnim){
            Atk();
        }
    }
    private void Jump () {
        jumpSpeedV.y = jumpSpeed;
        moveController.isJump = false;
    }
    private void Atk () {
        moveController.isAtk = false;
        EventCenter.Instance.TiggerEvent("Atk",this.gameObject);
    }

    public void EnterAtkAnim(object obj){
        this.transform.Find("Girl/Hip/ULeg_L_/DLeg_L_/Foot_L_").GetComponent<CapsuleCollider>().enabled = true;
        isAtkAnim = true;
        Debug.Log("isAnim");
    }
    public void ExitAtkAnim(object obj){
        this.transform.Find("Girl/Hip/ULeg_L_/DLeg_L_/Foot_L_").GetComponent<CapsuleCollider>().enabled = false;
        isAtkAnim =false;
    }
}