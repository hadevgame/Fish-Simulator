using API.Ads;
using API.LogEvent;
using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;

public class CashPanelGUI : BaseGUI
{
    public enum CashingType 
    {
        Cash,
        Card
    }

    [SerializeField] private GameObject totalScene;
    [SerializeField] private CardPaymentUI cardUI;
    [SerializeField] private PayMentUI cashUI;
    [SerializeField] private CardPanel cardPanel;
    [SerializeField] private MoneyPanel moneyPanel;
    [SerializeField] private CashDesk cashDesk;

    private bool iscard = false;
    private bool success = false;
    private string cashingType;
    private void Start()
    {
        CardPanel.OnConfirmPayment = CheckTotal;
        MoneyPanel.OnCheckPayment = CheckPayment;
        //cashDesk = CashDesk.Instance;
        //totalScene = CheckOutController.Ins.totalScene.gameObject;
        //cardUI = CheckOutController.Ins.cardpaymentScene;
    }

    public void ToggleCash(bool active) 
    {
        moneyPanel.gameObject.SetActive(active);
    }

    public void ToggleCard(bool active) 
    {
        cardPanel.gameObject.SetActive(active);
    }
    
    public void CheckTotal(float total)
    {
        iscard = true;
        cashingType = CashingType.Card.ToString();
        float requiredTotal = cardUI.GetTotal();
        string totalStr = total.ToString("F2");
        string requiredStr = requiredTotal.ToString("F2");
        
        if (totalStr == requiredStr)
        {
            success = true;
            Debug.Log("Thanh toán thành công!");
            FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
            //FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("cashing_type", cashingType), new Parameter("success", success.ToString()) });
            cardUI.gameObject.SetActive(false);
            ToggleCard(false);
            totalScene.SetActive(true);
            CheckOutController.Ins.ToggleButton(true);
            MoneyManager.instance.AddMoney(total);
            CustomerStateMachine payCus = cashDesk.GetFirstCustomerInQueue() as CustomerStateMachine;

            if (payCus != null)
            {
                payCus.ChangeState<TakingState>();
                ShopLevelManager.OnAddExp?.Invoke(5);
            }
            NotifiGUI.Instance.ShowPopup("Payment successful", null,1f, ()=> 
            {
                AdManager.Ins.ShowFull("payment_success");
                //AdManager.Ins.ShowFull("payment_success", () =>
                //{
                    
                //});
            });
            
            
        }
        else
        {
            success = false;
            NotifiGUI.Instance.ShowPopup("Payment fail", Color.red);
            FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
            //FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("cashing_type", cashingType), new Parameter("success", success.ToString()) });
        }
    }

    public bool CheckPayment()
    {
        iscard = false;
        cashingType = CashingType.Cash.ToString();
        if (IsMoneyEqual(cashUI.CurGiving, cashUI.Change))
        {
            success = true;
            NotifiGUI.Instance.ShowPopup("Payment successful");
            FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
            //FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("cashing_type", cashingType), new Parameter("success", success.ToString()) });
            cashUI.SetDefault();
            MoneyManager.instance.AddMoney(cashUI.Total);
            
            cashUI.gameObject.SetActive(false);
            ToggleCash(false);
            totalScene.SetActive(true);
            CheckOutController.Ins.ToggleButton(true);

            return true;
        }
        else
        {
            success = false;
            NotifiGUI.Instance.ShowPopup("Payment fail", Color.red);
            FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("is_card", iscard.ToString()), new Parameter("success", success.ToString()) });
            //FirebaseLogger.Ins.LogEvent($"cashing", new Parameter[] { new Parameter("cashing_type", cashingType), new Parameter("success", success.ToString()) });
            return false;
        }
    }

    private bool IsMoneyEqual(float a, float b)
    {
        return a.ToString("F2") == b.ToString("F2");
    }
}
