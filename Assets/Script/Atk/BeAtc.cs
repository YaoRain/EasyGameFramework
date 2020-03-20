using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeAtc : MonoBehaviour
{

    private void Awake() {
        EventCenter.Instance.AddEventListener("atkMonster",BeAtcAction);
    }

    // private void OnTriggerEnter(Collider hit) {
    //     if(hit.gameObject.layer == LayerMask.NameToLayer("PlayerAtk")){
    //         ActorController actC = hit.GetComponentInParent<ActorController>();
    //         if(actC.isAtkAnim){
    //             Debug.Log("Ohhhhhhh!");
    //         }
    //     };
    // }

    public void BeAtcAction(object obj){
        List<GameObject> atkAction = (List<GameObject>) obj;
        if(atkAction[1].GetComponent<MonsterInfo>().name == this.GetComponent<MonsterInfo>().name){
            string name = this.transform.GetComponent<MonsterInfo>().name;
            string playerName = atkAction[0].name;
            Debug.Log(name+" be atked by "+playerName);

            AudioManagerMy.Instance.PlayAudio("hit_05");
        }
    }
}
