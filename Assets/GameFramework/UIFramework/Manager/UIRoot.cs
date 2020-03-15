using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour {
    void Start () {
        UIManager.Instance.PushPanel (UIPanelType.BackGround);
        UIManager.Instance.PushPanel (UIPanelType.StartMenu);
    }
}