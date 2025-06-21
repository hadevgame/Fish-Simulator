using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public abstract class BasePopupGUI : BaseGUI
    {
        [SerializeField] List<Button> closeButtons;

        static int OpeningCount = 0;

        public static bool HasOpeningPopup => OpeningCount > 0;

        protected override void SetUp()
        {
            base.SetUp();
            foreach (var item in closeButtons)
            {
                item.onClick.AddListener(Close);
            }
        }

        public override void Open()
        {
            base.Open();
            OpeningCount++;
        }


        public override void OnClosed()
        {
            base.OnClosed();
            OpeningCount--;
        }
    }
}