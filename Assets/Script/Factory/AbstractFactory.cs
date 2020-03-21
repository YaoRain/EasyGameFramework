using System;
using UnityEngine;

class AbstractFactory : Singleton<AbstractFactory>{
    public GameObject monster;
    public GameObject player;
}

class ConcreteFactory : AbstractFactory{

    // 在指定位置生成怪物
    public GameObject CreatMonster(Vector3 position){
        monster = new GameObject();
        monster.transform.SetParent(GameObject.Find("Monster").transform);
        monster.transform.position= new Vector3(position.x,position.y,position.z);
        monster.AddComponent<MonsterInfo>();
        monster.AddComponent<MonsterAction>();
        return monster;
    }
}