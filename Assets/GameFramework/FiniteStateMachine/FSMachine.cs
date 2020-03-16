using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSMschine : Singleton<FSMschine>
{
    private string nowState;
    private Dictionary<string, string> conditionNextState; // 储存条件和对应的下一个状态

    public FSMschine(){}

    public FSMschine(string state)
    {
        nowState = state;
        string path = nowState + "FSMchineInfo";
        TextAsset fsmInfo = ResourceManager.Instance.Load<TextAsset>(path); // 获取存储状态转化信息的文件，一个状态机对应一个文件
        conditionNextState = JsonUtility.FromJson<Dictionary<string,string>>(fsmInfo.text);
    }

    void Init(string state)
    {
        nowState = state;
        string path = nowState + "FSMchineInfo";
        TextAsset fsmInfo = ResourceManager.Instance.Load<TextAsset>(path); // 获取存储状态转化信息的文件，一个状态机对应一个文件
        conditionNextState = JsonUtility.FromJson<Dictionary<string,string>>(fsmInfo.text);
    }

    void ChangeState(string condition) // 获取条件，让状态机变成下一个状态（原状态被覆盖）
    {
        string nextState = conditionNextState[condition];
        Init(nextState);
    }

    public FSMschine GetNextState(string condition)
    {
        string nextState = conditionNextState[condition];
        return new FSMschine(nextState);
    }
}