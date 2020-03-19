using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Dictionary<string,List<GameObject>> Monster;
    public Vector3 monsterPosition;
    public GameManager(){
        monsterPosition = GameObject.Find("CanBeCall").transform.position;
    }

    public void InitMonster(){
        //ResourceManager.Instance.LoadAsyn<GameObject>("Prefeb/Monster",);
    }

    private void SetMonster(GameObject monster){
        monster.transform.position = monsterPosition;
    }
}
