using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloatBooster
{
    [SerializeField] float boostValue;

    [SerializeField] float levelIncreaseValue;

    public float GetBoostValue(int level)
    {
        return boostValue + levelIncreaseValue * level;
    }
    public string GetDescriptionPrefix(int level)
    {
        if (GetBoostValue(level) > 0) return "+";
        return "";
    }
}
