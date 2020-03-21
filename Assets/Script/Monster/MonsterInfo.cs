using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class MonsterInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public string monsterName = "redCube";
    public string type = "Cube";
    public int hp = 100;
    public int moveSpeed = 1;
    public int isDie;
    public float[] position;
}
