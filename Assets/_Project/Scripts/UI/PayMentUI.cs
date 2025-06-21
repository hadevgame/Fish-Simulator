using API.LogEvent;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PayMentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI receivedText;
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI changeText;
    [SerializeField] private TextMeshProUGUI givingText;
    public static Action<float> OnUpdateGiving = null;
    float curGiving ;
    float change ;
    float total;
    public float CurGiving => curGiving;
    public float Change => change;
    public float Total => total;
    private void OnEnable()
    {
        OnUpdateGiving = UpdateGiving;
    }
    private void OnDisable()
    {
        OnUpdateGiving = null;
    }
    public void UpdateScene(float money, int count , float price)
    {
        receivedText.text = "$" + money.ToString("F2");
        totalText.text = "$" + price.ToString("F2");
        change = money - price;
        changeText.text = "$" + change.ToString("F2");
        changeText.color = Color.yellow;
        givingText.text = "$" + 0f.ToString("F2");
        total = money - change;
    }

    public void UpdateGiving(float value)
    {
        curGiving += value;
        givingText.text = "$" + curGiving.ToString("F2");
        CheckGivingAmount();
    }

    private void CheckGivingAmount()
    {
        if (IsMoneyEqual(curGiving, change))
        {
            givingText.color = Color.green;
        }
        else if (curGiving > change)
        {
            givingText.color = Color.red;
        }
        else
        {
            givingText.color = Color.yellow;
        }
    }

    //public bool CheckPayment()
    //{
    //    iscard = false;
    //    if (IsMoneyEqual(curGiving, change))
    //    {
    //        success = true;
    //        NotifiGUI.Instance.ShowPopup("Payment successful");
    //        FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
    //        SetDefault();
    //        MoneyManager.instance.AddMoney(total);
    //        this.gameObject.SetActive(false);
    //        payment.SetActive(false);
    //        totalScene.SetActive(true);
    //        CheckOutController.Ins.ToggleButton(true);
    //        btnExit.SetActive(true);

    //        return true;
    //    }
    //    else
    //    {
    //        success = false;
    //        givingText.color = Color.red;
    //        NotifiGUI.Instance.ShowPopup("Payment fail", Color.red);
    //        FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
    //        return false;
    //    }
    //}

    private bool IsMoneyEqual(float a, float b)
    {
        return a.ToString("F2") == b.ToString("F2");
    }

    public void SetDefault() 
    {
        receivedText.text = "$" +0.ToString();
        totalText.text = "$" + 0.ToString();
        change = 0;
        changeText.text = "$" + change;
        givingText.text = "$" + 0.ToString();
        givingText.color = Color.yellow;
        curGiving = 0;
    }
}
