using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class SceneGUI : BaseGUI
{
    [SerializeField] private Button btnSetting;
    [SerializeField] private ShopLevelManager shopLevelManager;
    private void Start()
    {
        btnSetting.onClick.AddListener(OpenSetting);
    }

    void OpenSetting() 
    {
        Vibrator.SoftVibrate();
        GUIController.Ins.Open<SettingGUI>();
    }

    public ShopLevelManager GetLevel() 
    {
        return shopLevelManager;
    }
}
