using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UCExtension;
using UCExtension.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "LockedFeatureData", menuName = "UCExtension/LockedFeatureData")]
public class LockedFeatureData : BaseData
{
    [FoldoutGroup("Locked settings")]
    public bool DefaultUnlocked;

    [FoldoutGroup("Locked settings")]
    public bool DefaultOwned;

    [Tooltip("Features to unlock when owned this")]
    [FoldoutGroup("Locked settings")]
    public List<LockedFeatureData> FeaturesToUnlock = new List<LockedFeatureData>();

    [Tooltip("Features to own when owned this")]
    [FoldoutGroup("Locked settings")]
    public List<LockedFeatureData> FeaturesToOwn = new List<LockedFeatureData>();

    public static Action<string> OnOwned;

    public static Action<string> OnUnlocked;

    string OWNED_KEY => "FEATURE_OWNED_" + ID;

    string UNLOCK_KEY => "FEATURE_UNLOCKED_" + ID;

    public bool IsOwned
    {
        get
        {
            return PlayerPrefsExtension.GetBool(OWNED_KEY, DefaultOwned);
        }
        protected set
        {
            PlayerPrefsExtension.SetBool(OWNED_KEY, value);
        }
    }

    public bool IsUnlocked
    {
        get
        {
            return PlayerPrefsExtension.GetBool(UNLOCK_KEY, DefaultUnlocked);
        }
        private set
        {
            PlayerPrefsExtension.SetBool(UNLOCK_KEY, value);
        }
    }

    public void Unlock()
    {
        IsUnlocked = true;
        OnUnlocked?.Invoke(ID);
    }

    public void Lock()
    {
        IsUnlocked = DefaultUnlocked;
    }

    public virtual void Own(string placement = "")
    {
        if (!IsUnlocked) Unlock();
        UnlockNextFeatures();
        IsOwned = true;
        OnOwned?.Invoke(ID);
    }

    void UnlockNextFeatures()
    {
        foreach (var item in FeaturesToUnlock)
        {
            item.Unlock();
        }
        foreach (var item in FeaturesToOwn)
        {
            item.Own();
        }
    }

    public void ResetData()
    {
        IsOwned = DefaultOwned;
        IsUnlocked = DefaultUnlocked;
    }
}