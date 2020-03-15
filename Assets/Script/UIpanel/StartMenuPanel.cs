using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuPanel : BasePanel {

    Transform _startMenuPanel;
    private Button _gameInfoButton;
    private Button _settingButton;
    private Button _startButton;
    private CanvasGroup _startMenuPanelCG;

    private void Awake() {
        Init();
    }

    public override void Init () {
        _startMenuPanel = this.transform;
        _gameInfoButton = _startMenuPanel.Find ("GameInfoButton").GetComponent<Button> ();
        _settingButton = _startMenuPanel.Find ("SettingButton").GetComponent<Button> ();
        _startButton = _startMenuPanel.Find("StartButton").GetComponent<Button> ();
        _startMenuPanelCG = _startMenuPanel.GetComponent<CanvasGroup> ();
        SetListener ();
    }

    private void SetListener () {
        _gameInfoButton.onClick.AddListener (() => {
            UIManager.Instance.PushPanel(UIPanelType.GameInfo);
        });
        _settingButton.onClick.AddListener (() => {
            UIManager.Instance.PushPanel (UIPanelType.Setting);
        });
        _startButton.onClick.AddListener(()=>
        {
            // 场景加载完成后，清空栈和Panel字典
            ScenesManager.Instance.LoadScenes("Chapter0",()=>
            {
                UIManager.Instance.ClearPanelStack();
                UIManager.Instance.ClearPanelDict();
            });
        });
    }
    
    public override void onPause () {
        _startMenuPanelCG.blocksRaycasts = false;
    }

    public override void onResume () {
        _startMenuPanelCG.blocksRaycasts = true;
    }
}