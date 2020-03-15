using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UIPanelInfo : ISerializationCallbackReceiver
{
    [NonSerialized]
    public UIPanelType panelType;
    public string panelPath;
    public string panelTypeString;
    // {
    //     get
    //     {
    //         return panelType.ToString();
    //     }
    //     set
    //     {
    //         panelType = (UIPanelType)System.Enum.Parse(typeof(UIPanelType),value);
    //     }
    // }

    //序列化指从对象到文本，反序列化就是反过来
    public void OnAfterDeserialize()
    {
        panelType = (UIPanelType)System.Enum.Parse(typeof(UIPanelType),panelTypeString);
    }
    public void OnBeforeSerialize()
    {

    }
}

[Serializable]
public class UIPanelList {
    public List<UIPanelInfo> uiPanelList;
}
