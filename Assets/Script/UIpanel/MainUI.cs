using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BasePanel {

    Transform _mainUIPanel;
    private Button _gameInfoButton;
    private Button _settingButton;
    private CanvasGroup _startMenuPanelCG;
    private Text _InfoText;

    private void Awake() {
        Init();
        EventCenter.Instance.AddEventListener("keyDown",KeyDownAction);
    }

    public override void Init () {
        _mainUIPanel = this.transform;
        _gameInfoButton = _mainUIPanel.Find ("GameInfoButton").GetComponent<Button> ();
        _settingButton = _mainUIPanel.Find ("SettingButton").GetComponent<Button> ();
        _startMenuPanelCG = _mainUIPanel.GetComponent<CanvasGroup> ();
        _InfoText = _mainUIPanel.Find("Info").GetComponent<Text>();
        SetListener ();
    }

    private void SetListener () {
        _gameInfoButton.onClick.AddListener (() => {
            UIManager.Instance.PushPanel (UIPanelType.GameInfo);
        });
        _settingButton.onClick.AddListener (() => {
            UIManager.Instance.PushPanel (UIPanelType.Setting);
        });
    }
    
    public override void onPause () {
        _startMenuPanelCG.blocksRaycasts = false;
        Debug.Log ("MainUI on pause");
    }

    public override void onResume () {
        _startMenuPanelCG.blocksRaycasts = true;
        Debug.Log("MainUI on resume");
    }

    public void KeyDownAction(object obj){
        KeyCode key = (KeyCode) obj;
        if(key == KeyCode.P){
            MoveController role = GameObject.Find("InputHandle").GetComponent<MoveController>();
            string info;
            if(role.isWalk){
                info = "role is walking";
            }else{
                info = "role is running";
            }
            _InfoText.text = info;
        }
    }
}