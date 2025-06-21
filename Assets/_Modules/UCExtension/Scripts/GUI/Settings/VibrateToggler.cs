using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.GUI
{
    public class VibrateToggler : SettingToggle
    {
        protected override void OnToggle(bool value)
        {
            PlayerDatas.CanVibrate = value;
        }

        protected override bool StartState => PlayerDatas.CanVibrate;
    }
}