using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAction : MonoBehaviour
{
    MonsterInfo monsterInfo;
    Animator anim;
    Rigidbody rb;
    private void Awake() {
        EventCenter.Instance.AddEventListener("atkMonster",BeAtcAction);
        EventCenter.Instance.AddEventListener("stepMonster",BeStepAction);
        monsterInfo = this.GetComponent<MonsterInfo>();
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    // private void OnTriggerEnter(Collider hit) {
    //     if(hit.gameObject.layer == LayerMask.NameToLayer("PlayerAtk")){
    //         ActorController actC = hit.GetComponentInParent<ActorController>();
    //         if(actC.isAtkAnim){
    //             Debug.Log("Ohhhhhhh!");
    //         }
    //     };
    // }

    private void FixedUpdate() {
        SwitchAnim();
        Move();
    }

    private void SwitchAnim(){

    }

    private void Move(){

    }

    public void BeAtcAction(object obj){
        List<GameObject> atkAction = (List<GameObject>) obj;
        GameObject player = atkAction[0], monster = atkAction[1];
        if(monster.GetComponent<MonsterInfo>().monsterName == monsterInfo.monsterName){
            string name = monsterInfo.monsterName;
            string playerName = player.GetComponent<PlayerInfo>().playerName;
            monsterInfo.hp -= player.GetComponent<PlayerInfo>().atk;
            anim.SetTrigger("beHit");
            if(monsterInfo.hp <= 0){
                anim.SetBool("isDie",true);
                this.GetComponent<Collider>().isTrigger = true;
                this.gameObject.layer = 0;
                rb.useGravity = false;
            }
            Debug.Log(name+" be atked by "+playerName+" HP: "+monsterInfo.hp);
            AudioManagerMy.Instance.PlayAudio("hit_05");
        }
    }

    public void BeStepAction(object obj){
        List<GameObject> atkAction = (List<GameObject>) obj;
        GameObject player = atkAction[0], monster = atkAction[1];
        if(monster.GetComponent<MonsterInfo>().monsterName == monsterInfo.monsterName){
            string name = monsterInfo.monsterName;
            string playerName = player.GetComponent<PlayerInfo>().playerName;
            monsterInfo.hp = 0;
            anim.SetTrigger("beHit");
            if(monsterInfo.hp <= 0){
                anim.SetBool("isDie",true);
                this.GetComponent<Collider>().isTrigger = true;
                this.gameObject.layer = 0;
                rb.useGravity = false;
            }
            Debug.Log(name+" be steped by "+playerName+" HP: "+monsterInfo.hp);
            AudioManagerMy.Instance.PlayAudio("hit_05");
        }
    }
}
