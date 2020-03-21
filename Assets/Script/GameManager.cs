using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterList{
    [SerializeField]
    List<MonsterInfo> monsterList;
}

public class GameManager : Singleton<GameManager>
{
    Dictionary<string,GameObject> Monster;
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

    private void InitMonsterFromJson(string path){
        TextAsset monsterJson = ResourceManager.Instance.Load<TextAsset>(path); // 获取json文本
        if(monsterJson != null){
            MonsterList monsterList = JsonUtility.FromJson<MonsterList>(monsterJson.text);
        }
    }
}
