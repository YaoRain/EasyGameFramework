using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public Collider[] outputCols;
    public bool isOnGround = true;
    private Vector3 point1;
    private Vector3 point2;
    private float radius;
    void Awake() {
        capcol = this.GetComponentInParent<CapsuleCollider>();
        radius = capcol.radius;
    }

    private void FixedUpdate() {
        point1 = (transform.position + transform.up*radius*0.5f); // 降低碰撞盒高度
        point2 = transform.position + transform.up*capcol.height;
        //outputCols = Physics.OverlapCapsule(point1,point2,radius*0.99f,LayerMask.GetMask("Ground")); // 减少半径防止侧面碰撞
        outputCols = Physics.OverlapSphere(point1,radius*1.2f,LayerMask.GetMask("Ground"));
        if (outputCols.Length > 0){
            isOnGround = true;
        }else{
            isOnGround = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
