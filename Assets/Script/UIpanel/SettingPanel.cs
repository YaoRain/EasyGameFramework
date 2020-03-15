using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel {
    Transform _settingPanel = null;
    private Button _closeButton;

    private void Awake() {
        Init();
    }

    override public void Init () {
        _settingPanel = this.transform;
        _closeButton = _settingPanel.Find ("CloseButton").GetComponent<Button> ();
        SetListener();
    }

    public void SetListener () {
        _closeButton.onClick.AddListener (() => {
            UIManager.Instance.PopPanel ();
        });
    }

    override public void onEnter () {
        _settingPanel.gameObject.SetActive (true);
    }

    override public void onExit () {
        _settingPanel.gameObject.SetActive (false);
    }
}