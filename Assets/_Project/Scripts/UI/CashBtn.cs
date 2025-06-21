using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CashBtn : MonoBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private Button button = null;
    public int Value => value;
    public void OnClick(UnityAction onclick)
    {
        if (button == null)
            button = GetComponent<Button>();
        button.onClick.AddListener(onclick);
    }
}
