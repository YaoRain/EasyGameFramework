using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Root : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.PushPanel(UIPanelType.MainUI);
        AudioManagerMy.Instance.PlayBackgroundMusic("Bgm1");
        Debug.Log("playBgm");
        InputManager.Instance.StartInputListenAny();
    }
}
