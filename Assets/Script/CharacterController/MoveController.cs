using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {
    public bool inputEnable = true; // 控制按键是否可以产生输入信号
    public bool rotaEnable = true; // 控制模型是否可以转向

    public float _runSpeed = 4f;
    public float _walkSpeed = 2f;
    public float _rotaSpeed = 30f;
    public float _jumpSpeed = 4f;
    public float _rollSpeed = 2f;

    public float hightOfGround = 0;

    public float targetZMoveSingle = 0; // 前后轴向的输入信号
    public float targetXMoveSingle = 0; // 水平轴向的输入信号
    public float targetRMoveSingle = 0; // 旋转的输入信号
    public float transTime = 3f; // 平滑函数的过渡时间
    public float zMoveSingle = 0, xMoveSingle = 0, rMoveSingle = 0; // 平滑函数计算出的信号
    private float ZTempSingle, XTempSingle, RTempSingle; // 临时变量
    public float singleWeight = 1.0f; // 移动信号输入强度

    public float moveSingle = 0;
    public Vector3 modelForward = new Vector3 (); // 模型的面向
    public Vector3 cameraForward = new Vector3 (); // 摄像机朝向

    public bool isWalk = false, isRunning = false, isAtk = false,
        isMoveLeft = false, isMoveRight = false,
        isTurnLeft = false, isTurnRight = false,
        isGoBack = false, isJump = false, isRoll = false; // 这些变量表示现在正在按下什么按键

    public bool isOnRollAnim = false;
    public bool isOnAtkAnim = false;

    RaycastHit hit; // 存储射线检测的详细信息

    void Awake () {
        EventCenter.Instance.AddEventListener ("keyKeep", KeyKeepDogAction);
        EventCenter.Instance.AddEventListener ("keyUp", KeyUpDogAction);
        EventCenter.Instance.AddEventListener ("keyDown", KeyDownDogAction);
        EventCenter.Instance.AddEventListener ("enterJumpAnim", DisableRota);
        EventCenter.Instance.AddEventListener ("exitJumpAnim", EnableRota);
        EventCenter.Instance.AddEventListener ("enterRoll",EnterRoll);
        EventCenter.Instance.AddEventListener ("exitRoll",exitRoll);
        EventCenter.Instance.AddEventListener ("enterAtkAnim",DisableControl);
        EventCenter.Instance.AddEventListener ("exitAtkAnim",EnableControl);
    }

    // Update is called once per frame
    void Update () {
        FromKeyToSingle ();
    }

    void FromKeyToSingle () {
        if (!inputEnable) {
            targetZMoveSingle = 0;
            targetXMoveSingle = 0;
            targetRMoveSingle = 0;
        } else {
            targetZMoveSingle = (isRunning ? 1.0f : 0f) - (isGoBack ? 1.0f : 0f);
            targetXMoveSingle = (isMoveRight ? 1.0f : 0f) - (isMoveLeft ? 1.0f : 0f);
            targetRMoveSingle = (isTurnRight ? 1.0f : 0f) - (isTurnLeft ? 1.0f : 0f);
        }

        zMoveSingle = Mathf.SmoothDamp (zMoveSingle, targetZMoveSingle, ref ZTempSingle, transTime);
        xMoveSingle = Mathf.SmoothDamp (xMoveSingle, targetXMoveSingle, ref XTempSingle, transTime);
        rMoveSingle = Mathf.SmoothDamp (rMoveSingle, targetRMoveSingle, ref RTempSingle, transTime);
        // 通过映射函数把方形的值域转化为圆形
        float zMoveSingleU = zMoveSingle * Mathf.Sqrt (1 - xMoveSingle * xMoveSingle / 2);
        float xMoveSingleV = xMoveSingle * Mathf.Sqrt (1 - zMoveSingle * zMoveSingle / 2);

        float targetWeigth = isWalk ? 0.5f : 1.0f; // 通过给信号强度加权，控制走路和跑步
        singleWeight = Mathf.Lerp (singleWeight, targetWeigth, 0.1f);

        if(rotaEnable){ // 如果允许旋转模，则更新输入向量的方向和大小
            modelForward = singleWeight * (this.transform.forward * zMoveSingleU + this.transform.right * xMoveSingleV); //模型朝向
        }
        moveSingle = modelForward.magnitude; //移动信号向量的模
    }

    public void ClearSpeed(){
        modelForward = Vector3.zero;
    } // 使角色速度为0

    public void EnableRota (object obj) {
        rotaEnable = true;
    }

    public void DisableRota (object obj) {
        rotaEnable = false;
    }

    public void EnableControl(object obj) {
        inputEnable = true;
    }

    public void DisableControl(object obj) {
        inputEnable = false;
    }

    public void EnterRoll(object obj){
        DisableRota(obj);
        isOnRollAnim = true;
    }

    public void exitRoll(object obj){
        EnableRota(obj);
        isOnRollAnim = false;
    }

    // 检测按键按下
    public void KeyDownDogAction (object obj) {
        KeyCode key = (KeyCode) obj;
        if (key == KeyCode.J) {
            isAtk = true;
        }
        if (key == KeyCode.Space) {
            isJump = true;
        }
        if (key == KeyCode.P) {
            isWalk = !isWalk;
        }
        if (key == KeyCode.LeftShift){
            isRoll = true;
        }
    }
    // 检测按键按住
    public void KeyKeepDogAction (object obj) {
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
    // 检测按键抬起
    public void KeyUpDogAction (object obj) {
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
            //isJump = false;
        }
        if (key == KeyCode.LeftShift){
            isRoll = false;
        }
    }
}