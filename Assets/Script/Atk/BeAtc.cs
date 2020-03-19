using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeAtc : MonoBehaviour
{

    private void Awake() {
        EventCenter.Instance.AddEventListener("Atk",AtkAction);
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.layer == LayerMask.NameToLayer("Player")){
            ActorController actC = hit.GetComponentInParent<ActorController>();
            if(actC.isAtkAnim){
                Debug.Log("Ohhhhhhh!");
            }
        };
    }

    public void AtkAction(object obj){

    }
}
