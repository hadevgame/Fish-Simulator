using API.Ads;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RewardGUI : BaseGUI
{
    [SerializeField] private Button btnLater;
    [SerializeField] private Button btnGet;
    [SerializeField] private Button btnExit;

    private void Start()
    {
        btnLater.onClick.AddListener(ExitReward);
        btnGet.onClick.AddListener(GetReward);
        btnExit.onClick.AddListener(ExitReward);
    }
    void GetReward() 
    {
        //if (AdManager.Ins.CanShowFull())
        //{
        //    AdManager.Ins.ShowFull("get_reward", () =>
        //    {
        //        MoneyManager.instance.AddMoney(10);
        //        ExitReward();
        //    });
        //}
        //else NotifiGUI.Instance.ShowPopup("Waiting for loading ad");
        Vibrator.SoftVibrate();
        AdManager.Ins.ShowRewardedVideo("get_reward", (finish) =>
            {
                if (finish) 
                {
                    Reward();
                    ExitReward();
                }
                
            });
    }

    void Reward() 
    {
        MoneyManager.instance.AddMoney(30);
    }
    void ExitReward() 
    {
        Vibrator.SoftVibrate();
        if (AppRemoteDatas.Ins.CanPlayInter) 
        {
            AdManager.Ins.ShowFull("exit_reward", () =>
            {
                GUIController.Ins.Close<RewardGUI>();
            });
        }
        else GUIController.Ins.Close<RewardGUI>();

    }
}
