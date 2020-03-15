using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// UI管理器，管理UI的实例和生命周期
/// </summary>
public class UIManager : Singleton<UIManager> {
    private Dictionary<UIPanelType, string> panelPathDict; // 存储所有面板的Prefab路径
    private Dictionary<UIPanelType, BasePanel> panelDict; // 存储所有面板的实例
    private Stack<BasePanel> panelStack; // 用栈保留在显示的页面的信息

    public UIManager () {
        ParseUIPanelTypeJson ();
    }

    // 获取canvas
    private Transform _canvas = null;
    private Transform canvas {
        get {
            if (_canvas == null) {
                _canvas = GameObject.Find ("Canvas").transform;
            }
            return _canvas;
        }
        set {
            _canvas = value;
        }
    }

    /// <summary>
    /// 把UI面板压入栈
    /// </summary>
    /// <param name="panelType">面板类型</param>
    public void PushPanel (UIPanelType panelType) {
        if (panelStack == null) {
            panelStack = new Stack<BasePanel> ();
        }
        if (panelStack.Count > 0) {
            panelStack.Peek ().onPause (); // 暂停栈顶页面
        }
        BasePanel panelInstance = GetPanelInstance (panelType);
        panelStack.Push (panelInstance);
        panelStack.Peek ().onEnter ();
    }

    /// <summary>
    /// 把UI面板出栈
    /// </summary>
    public void PopPanel () {
        if (panelStack == null) {
            panelStack = new Stack<BasePanel> ();
        }
        if (panelStack.Count <= 0) {
            return;
        } else {
            panelStack.Pop ().onExit ();
        }
        if (panelStack.Count > 0) {
            panelStack.Peek ().onResume (); // 恢复栈顶页面
        }
    }

    /// <summary>
    /// 清空UI栈
    /// </summary>
    public void ClearPanelStack () {
        if (panelStack.Count > 0) {
            panelStack.Clear ();
        }
    }

    /// <summary>
    /// 清空UI字典
    /// </summary>
    public void ClearPanelDict () {
        if (panelDict.Count > 0) {
            panelDict.Clear ();
        }
    }

    /// <summary>
    /// 按面板类型获取面板实例（同步）
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <returns>面板实例</returns>
    private BasePanel GetPanelInstance (UIPanelType panelType) {
        if (panelDict == null) {
            panelDict = new Dictionary<UIPanelType, BasePanel> ();
        }
        BasePanel panelInstance = panelDict.TryGet (panelType);
        if (panelInstance == null) // 如果面板未实例化，那么就实例化一个，并且把它挂到canvas上
        {
            string path = panelPathDict.TryGet (panelType);
            GameObject panelInstanceObj = ResourceManager.Instance.Load<GameObject>(path);
            panelInstanceObj.transform.SetParent (canvas, false);
            panelInstance = panelInstanceObj.GetComponent<BasePanel> ();
            panelDict.Add (panelType, panelInstance);
        }
        return panelInstance;
    }

    /// <summary>
    /// 按面板类型获取面板实例（异步）
    /// </summary>
    /// <param name="panelType">面板类型</param>
    /// <param name="action">需要对面板执行的方法</param>
    private void GetPanelInstanceAsyn (UIPanelType panelType,UnityAction<BasePanel> action) {
        if (panelDict == null) {
            panelDict = new Dictionary<UIPanelType, BasePanel> ();
        }
        BasePanel panelInstance = panelDict.TryGet (panelType);
        if (panelInstance == null) // 如果面板未实例化，那么就实例化一个，并执行回调函数
        {
            string path = panelPathDict.TryGet (panelType);
            //GameObject panelInstanceObj = GameObject.Instantiate (Resources.Load<GameObject> (path)) as GameObject;
            ResourceManager.Instance.LoadAsyn<GameObject>(path,(obj)=>
            {
                GameObject panelInstanceObj = (GameObject) obj;
                panelInstanceObj.transform.SetParent (canvas, false);
                panelInstance = panelInstanceObj.GetComponent<BasePanel> ();
                panelDict.Add (panelType, panelInstance);
                action.Invoke(panelInstance);
            });
        }
    }

    /// <summary>
    /// 解析配置文件获取prefab
    /// </summary>
    private void ParseUIPanelTypeJson () {
        panelPathDict = new Dictionary<UIPanelType, string> ();
        TextAsset ta = ResourceManager.Instance.Load<TextAsset>("UIPanelType");
        UIPanelList panelInfoList = JsonUtility.FromJson<UIPanelList> (ta.text);
        foreach (UIPanelInfo info in panelInfoList.uiPanelList) {
            panelPathDict.Add (info.panelType, info.panelPath);
        }
    }
}