using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeAtc : MonoBehaviour
{

    private void Awake() {
        EventCenter.Instance.AddEventListener("Atk",AtkAction);
    }

    // private void OnTriggerEnter(Collider hit) {
    //     if(hit.gameObject.layer == LayerMask.NameToLayer("PlayerAtk")){
    //         ActorController actC = hit.GetComponentInParent<ActorController>();
    //         if(actC.isAtkAnim){
    //             Debug.Log("Ohhhhhhh!");
    //         }
    //     };
    // }

    public void AtkAction(object obj){

    }
    public void BeAtcAction(){
        string name = this.transform.GetComponent<MonsterInfo>().name;
        Debug.Log(name+" be atked");
    }
}
