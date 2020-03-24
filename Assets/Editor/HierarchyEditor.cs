
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// <summary>
/// 添加Hierarychy中的右键菜单
/// 说明："GameObject/"下[priority小于等于49]的按钮会出现在Hierarychy的右键菜单中
/// </summary>
public class HierarchyEditor : MonoBehaviour {
    static readonly TextEditor CopyTool = new TextEditor();
    /// <summary>
    /// 将一个GameObject在Hierarchy中的完整路径拷贝的剪切板
    /// </summary>
    [MenuItem("GameObject/Copy Path",priority =20)]
    static void CopyTransPath()
    {
        Transform trans = Selection.activeTransform;
        if (trans == null) return;
        CopyTool.text = GetTransPath(trans);
        CopyTool.SelectAll();
        CopyTool.Copy();
    }
    /// <summary>
    /// 获得GameObject在Hierarchy中的完整路径
    /// </summary>
    /// <returns>The trans path.</returns>
    /// <param name="trans">Trans.</param>
    public static string GetTransPath(Transform trans)
    {
        if (null == trans) return string.Empty;
        if (null == trans.parent) return trans.name;
        return GetTransPath(trans.parent) + "/" + trans.name;
    }
}

