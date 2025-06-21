using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.GUI
{
    public class BasePauseGUI : BasePopupGUI
    {
        bool isPaused;

        public override void Open()
        {
            base.Open();
            if (!isPaused)
            {
                isPaused = true;
                UCEManager.Ins.Pause();
            }
        }

        public override void Close()
        {
            base.Close();
            if (isPaused)
            {
                isPaused = false;
                UCEManager.Ins.UnPause();
            }
        }
    }
}