using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;
    [SerializeField] private float currentMoney ;
    public float CurrentMoney => currentMoney;
    [SerializeField] private Text moneyText;
    [SerializeField] private TextMeshProUGUI floatText;
    private Vector3 startPos;

    [SerializeField] private float income = 0;  
    public float Income => income;

    [SerializeField] private float expense = 0; 
    public float Expense => expense;
    private void Awake()
    {
        if (instance == null)
            instance = this;

        startPos = floatText.transform.localPosition;
    }

    public void ResetStat() 
    {
        income = 0;
        expense = 0;
    }
    
    public bool CanAfford(float amount)
    {
        return currentMoney >= amount;
    }

    public void SpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            expense += amount;
            currentMoney -= amount;
            moneyText.text = currentMoney.ToString("F2");
            ShowFloatingText(-amount, Color.red);
        }
    }

    public void AddMoney(float amount)
    {
        income += amount;
        currentMoney += amount;
        moneyText.text = currentMoney.ToString("F2");
        ShowFloatingText(+amount, Color.green);
    }
    public void SetMoney(float amount, float incomeload , float expenseload)
    {
        currentMoney = amount;
        income = incomeload; 
        expense = expenseload;
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString("F2");
        }
        
    }
    private void ShowFloatingText(float amount, Color textColor)
    {
        if (floatText == null) return;

        floatText.text = (amount > 0 ? "+$" : "-$") + Mathf.Abs(amount).ToString("F2");
        floatText.color = textColor;
        floatText.gameObject.SetActive(true);
        floatText.alpha = 1f;

        floatText.transform.localPosition = startPos; 
        floatText.transform.DOLocalMoveY(floatText.transform.localPosition.y + 50, 1f).SetEase(Ease.OutQuad); 
        floatText.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            floatText.gameObject.SetActive(false);
        });
    }
}
