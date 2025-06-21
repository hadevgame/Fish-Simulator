
using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UCExtension;
using API.Ads;

public class SettingGUI : BaseGUI
{
    //[SerializeField] private Button btnSetting;
    [SerializeField] private Button btnExit;
    [SerializeField] private GameObject panelSetting;
    private void Start()
    {
        OpenPanel();
        //panelSetting.SetActive(false); 
    }

    public void OpenPanel()
    {
        Vibrator.SoftVibrate();
        panelSetting.SetActive(true);
        btnExit.onClick.AddListener(ClosePanel);
    }

    public void ClosePanel()
    {
        Vibrator.SoftVibrate();
        if (AppRemoteDatas.Ins.CanPlayInter) 
        {
            AdManager.Ins.ShowFull("exit_settings", () =>
            {
                GUIController.Ins.Close<SettingGUI>();
            });
        }
        else GUIController.Ins.Close<SettingGUI>();

    }

}

