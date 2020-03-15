using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
    
public class GameInfoPanel : BasePanel {
    private Transform _gameInfoPanel = null;
    private Button _closeButton;

    private void Awake() {
        Init();
    }

    override public void Init () {
        _gameInfoPanel = this.transform;
        _closeButton = _gameInfoPanel.Find ("CloseButton").GetComponent<Button> ();
        SetListener ();
    }

    public void SetListener () {
        _closeButton.onClick.AddListener (() => {
            //UIManager.Instance.PopPanel ();
            UIManager.Instance.PopPanel();
        });
    }

    override public void onEnter () {
        //_gameInfoPanel.gameObject.SetActive (true);
    }

    override public void onExit () {
        _gameInfoPanel.gameObject.SetActive (false);
        Debug.Log("onexit");
    }
    override public void onExit_pool()
    {
        Debug.Log("onexit");
    }
}