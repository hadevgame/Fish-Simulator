using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCLogger : MonoBehaviour
{
    public static void Log(string content, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        string logContent = $"<color=#{colorHex}>{content}</color>";
        Debug.Log(logContent);
    }
}
