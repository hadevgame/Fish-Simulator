using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntBooster
{
    [SerializeField] int boostValue;

    [SerializeField] int levelIncreaseValue;

    public int GetBoostValue(int level)
    {
        return boostValue + levelIncreaseValue * level;
    }

    public string GetDescriptionPrefix(int level)
    {
        if (GetBoostValue(level) > 0) return "+";
        return "";
    }
}
