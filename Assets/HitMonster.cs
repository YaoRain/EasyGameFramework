using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMonster : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider monster) {
        if(monster.gameObject.layer == LayerMask.NameToLayer("Monster")){
            List<GameObject> playerAndMonster = new List<GameObject>{this.GetComponentInParent<ActorController>().gameObject,monster.gameObject};
            EventCenter.Instance.TiggerEvent("atkMonster",playerAndMonster);
            //monster.GetComponent<BeAtc>().BeAtcAction();
            this.GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
