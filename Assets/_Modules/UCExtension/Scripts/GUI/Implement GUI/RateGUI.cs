using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public class RateGUI : BasePopupGUI
    {
        [SerializeField] float timeDelayClose = 0.5f;

        [SerializeField] int minStar = 4;

        [SerializeField] Sprite activeSprite;

        [SerializeField] Sprite deactiveSprite;

        [SerializeField] List<RateStar> stars;

        [SerializeField] Button closeButton;

        bool rated = false;

        public static Action<int> OnRate;

        protected override void SetUp()
        {
            base.SetUp();
            int index = 1;
            foreach (var item in stars)
            {
                int star = index;
                item.SetStar(star);
                item.SetSprite(deactiveSprite);
                index++;
                item.OnSelect += OnSelect;
            }
            closeButton.onClick.AddListener(OnClickClose);
        }

        public override void Open()
        {
            base.Open();
            rated = false;
        }

        void OnClickClose()
        {
            Close();
            OnRate?.Invoke(0);
        }

        void OnSelect(int star)
        {
            if (rated) return;
            for (int i = 0; i < stars.Count; i++)
            {
                bool active = i <= star - 1;
                stars[i].SetSprite(active ? activeSprite : deactiveSprite);
            }
            DOVirtual.DelayedCall(timeDelayClose, () =>
            {
                if (star >= minStar)
                {
                    GameExtensions.OpenGoogleStore();
                }
                Close();
            });
            OnRate?.Invoke(star);
        }

#if UNITY_EDITOR
        [Button]
        public void SetStarImage()
        {
            foreach (var item in stars)
            {
                item.SetSprite(activeSprite);
                item.SetDirtyAndSave();
            }
        }

#endif
    }
}