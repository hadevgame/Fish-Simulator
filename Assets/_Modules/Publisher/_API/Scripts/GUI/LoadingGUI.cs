using API.Ads;
using API.RemoteConfig;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace API
{
    public class LoadingGUI : BaseGUI
    {
        [SerializeField] Image loadImage;

        [SerializeField] float endLoadTime = 1;

        [SerializeField] bool waitFirebase;

        Tween loadTween;

        Func<bool> Wait;

        bool needWait;

        bool loadEnded;

        public override void Open()
        {
            base.Open();
            StartLoad();
        }

        public LoadingGUI SetWait(Func<bool> wait)
        {
            needWait = true;
            Wait = wait;
            return this;
        }

        void StartLoad()
        {
            loadImage.fillAmount = 0;
            float timeLoad = AdManager.Ins.RemoteSettings.MaxTimeLoadAOAInStart + 1;
            Debug.Log($"Load in {timeLoad}s.");
            Load(timeLoad, 1, WaitEndLoad);
            StartCoroutine(IEWaitAOA());
        }

        void Load(float time, float amount, Action finish = null)
        {
            loadTween?.Kill();
            loadTween = loadImage.DOFillAmount(amount, time)
                .SetUpdate(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    finish?.Invoke();
                });
        }

        IEnumerator IEWaitAOA()
        {
            yield return new WaitUntil(() => CheckEnd());
            if (!loadEnded)
            {
                Load(endLoadTime, 1, () =>
                {
                    WaitEndLoad();
                });
            }
        }

        public bool CheckEnd()
        {
            if (waitFirebase)
            {
                return AdManager.Ins.IsAOAShowed && RemoteConfigManager.Ins.IsFetchedDatas;
            }
            return AdManager.Ins.IsAOAShowed;
        }

        void WaitEndLoad()
        {
            loadEnded = true;
            StartCoroutine(IEEndLoad());
        }

        IEnumerator IEEndLoad()
        {
            if (needWait)
            {
                yield return new WaitUntil(Wait);
            }
            AdManager.Ins.ShowBanner();
            Close();
        }

    }
}