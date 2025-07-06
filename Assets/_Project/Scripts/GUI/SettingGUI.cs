
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
    [SerializeField] private Button btnExit;
    [SerializeField] private GameObject panelSetting;
    private void Start()
    {
        OpenPanel();
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
        GUIController.Ins.Close<SettingGUI>();

    }

}

