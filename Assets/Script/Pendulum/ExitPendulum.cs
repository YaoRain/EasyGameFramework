using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPendulum : MonoBehaviour
{
    private void OnTriggerEnter(Collider player) {
        if(player.gameObject.layer != LayerMask.NameToLayer("InputHandle")){
            return;
        }
        Debug.Log("exitPendulum");
        EventCenter.Instance.TiggerEvent("exitPendulum",null);
    }
}
