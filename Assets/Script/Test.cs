using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    private Transform _canvas = null;
    private Transform canvas {
        get {
            if (_canvas == null) {
                _canvas = GameObject.Find ("Backgroud").transform;
            }
            return _canvas;
        }
        set {
            _canvas = value;
        }
    }

    // private void Awake () {
    //    Transform _gameInfoPanel = UIManager.Instance.GetPanelInstance (UIPanelType.GameInfo).transform;
    // }
    
}