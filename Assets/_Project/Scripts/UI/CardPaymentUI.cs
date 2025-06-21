using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardPaymentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totaltext;

    public void UpdateScene(float total)
    {
        totaltext.text = "$" + total.ToString();
    }
    
    public float GetTotal() 
    {
        return float.Parse(totaltext.text.Substring(1));
    }
}
