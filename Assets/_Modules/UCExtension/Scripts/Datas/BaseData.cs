using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BaseData : ScriptableObject
{
    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.IDGroup)]
    public string ID;

    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.NameGroup)]
    public string Name;

    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.DescriptionGroup)]
    public string Description;

    [FoldoutGroup(BaseDataConst.InforsGroup)]
    public Sprite Avatar;

    [FoldoutGroup(BaseDataConst.InforsGroup)]
    public int SortPriority;

    public bool HasSameID(BaseData data)
    {
        return ID.Equals(data.ID);
    }
    public bool HasSameID(string compareID)
    {
        return ID.Equals(compareID);
    }

#if UNITY_EDITOR
    [Button("Set Default")]
    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.IDGroup, 100)]
    public void SetID()
    {
        ID = Regex.Replace(name, @"[^0-9a-zA-Z]+", "_").Replace(" ", "_").ToLower();
        EditorUtility.SetDirty(this);
    }

    [Button("Set Default")]
    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.NameGroup, 100)]
    public void SetName()
    {
        Name = name;
        EditorUtility.SetDirty(this);
    }

    [Button("Set Default")]
    [FoldoutGroup(BaseDataConst.InforsGroup)]
    [HorizontalGroup(BaseDataConst.DescriptionGroup, 100)]
    public void SetDescriptions()
    {
        Description = "Desc: " + name;
        EditorUtility.SetDirty(this);
    }

    [Button("Set Default Infors")]
    [FoldoutGroup(BaseDataConst.InforsGroup)]
    public void SetInfors()
    {
        SetName();
        SetID();
        SetDescriptions();
    }
#endif
}

public static class BaseDataConst
{
    public const string NameGroup = InforsGroup + "/Name";

    public const string IDGroup = InforsGroup + "/ID";

    public const string DescriptionGroup = InforsGroup + "/Description";

    public const string InforsGroup = "Infors";
}
