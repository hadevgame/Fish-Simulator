//using DG.Tweening;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UIElements;

//public class DailyStats : PopupAnim
//{
//    [SerializeField] private Text daytext;
//    [SerializeField] private Text incometext;
//    [SerializeField] private Text expensetext;
//    [SerializeField] private Text profittext;
//    [SerializeField] private Text sastisfiedtext;

//    private void OnEnable()
//    {
//        Show();
//        float income = MoneyManager.instance.Income;
//        float expense = MoneyManager.instance.Expense;
//        float profit = income - expense;
//        int sastisfied = CashDesk.Instance.SastisfiedCus;

//        daytext.text = GameTimeManager.Instance.CurrentDay.ToString();
//        sastisfiedtext.text = sastisfied.ToString();
//        incometext.text = "+$" + income.ToString();
//        expensetext.text = "-$" + expense.ToString();

//        if (profit >= 0)
//        {
//            profittext.text = "+$" + profit.ToString();
//        }

//        else
//        {
//            profittext.text = "-$" + Mathf.Abs(profit).ToString();
//            profittext.color = expensetext.color; // màu giống chi phí
//        }
//    }
//    public void StartNewDay()
//    {
//        GameTimeManager.Instance.StartNewDay();
//        Debug.Log("Ngày mới bắt đầu!");
//        MoneyManager.instance.ResetStat();
//        CashDesk.Instance.SetSastisfied();
//        Hide();
//        //gameObject.SetActive(false);
//    }

//}
using DG.Tweening;
using System.Collections;
using UCExtension.Audio;
using UnityEngine;
using UnityEngine.UI;

public class DailyStats : PopupAnim
{
    [SerializeField] private Text daytext;
    [SerializeField] private Text incometext;
    [SerializeField] private Text expensetext;
    [SerializeField] private Text profittext;
    [SerializeField] private Text sastisfiedtext;

    private void OnEnable()
    {
        Show();
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("ED"));
        StartCoroutine(ShowStatsSequence());
    }

    private IEnumerator ShowStatsSequence()
    {
        float income = MoneyManager.instance.Income;
        float expense = MoneyManager.instance.Expense;
        float profit = income - expense;
        int sastisfied = CashDesk.Instance.SastisfiedCus;

        daytext.text = "";
        sastisfiedtext.text = "";
        incometext.text = "";
        expensetext.text = "";
        profittext.text = "";

        yield return ShowText(daytext, $"{GameTimeManager.Instance.CurrentDay}");
        yield return ShowText(sastisfiedtext, $"{sastisfied}");
        yield return ShowText(incometext, $"+${income:F2}");
        yield return ShowText(expensetext, $"-${expense:F2}");

        if (profit >= 0)
        {
            profittext.color = incometext.color;
            yield return ShowText(profittext, $"+${profit:F2}");
        }
        else
        {
            profittext.color = expensetext.color; 
            yield return ShowText(profittext, $"-${Mathf.Abs(profit):F2}");
        }
    }

    private IEnumerator ShowText(Text textUI, string content)
    {
        textUI.text = content;
        textUI.canvasRenderer.SetAlpha(0f); 
        textUI.CrossFadeAlpha(1f, 0.5f, false); // Hiệu ứng fade-in trong 0.5 giây
        yield return new WaitForSeconds(0.6f); // Thời gian giữa các dòng (có thể điều chỉnh)
    }

    public void StartNewDay()
    {
        GameTimeManager.Instance.StartNewDay();
        Debug.Log("Ngày mới bắt đầu!");
        MoneyManager.instance.ResetStat();
        CashDesk.Instance.SetSastisfied();
        Hide();
        gameObject.SetActive(false);
    }
}
