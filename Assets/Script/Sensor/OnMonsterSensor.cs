using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMonsterSensor : MonoBehaviour
{
    public CapsuleCollider capcol;
    public Collider[] outputCols;
    public bool isOnMonster = true;
    private Vector3 point1;
    private float radius;
    public GameObject playerInfo;

    public List<GameObject> playerStepMonster; // 向事件中心发送攻击和被攻击的对象
    void Awake() {
        capcol = this.GetComponentInParent<CapsuleCollider>();
        playerInfo = this.transform.parent.gameObject;
        radius = capcol.radius;
    }

    private void FixedUpdate() {
        point1 = (transform.position + transform.up*radius*0.5f); // 降低碰撞盒高度
        outputCols = Physics.OverlapSphere(point1,radius*1.2f,LayerMask.GetMask("Monster"));
        if (outputCols.Length > 0){
            isOnMonster = true;
            foreach(var monster in outputCols){
                playerStepMonster = new List<GameObject>() {playerInfo,monster.gameObject};
                EventCenter.Instance.TiggerEvent("stepMonster",playerStepMonster);
            }
        }else{
            isOnMonster = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
