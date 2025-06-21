using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public class TransitionGUI : Singleton<TransitionGUI>
    {
        [SerializeField] float timeOpen;

        [SerializeField] float timeWait;

        [SerializeField] float timeClose;

        [SerializeField] Image curtain;

        TransitionSettings settings = new TransitionSettings();

        public bool IsShowing { get; private set; }

        private void Start()
        {
            curtain.gameObject.SetActive(false);
        }

        public TransitionSettings ShowTransition()
        {
            StopAllCoroutines();
            StartCoroutine(IETransition());
            return settings;
        }

        IEnumerator IETransition()
        {
            curtain.raycastTarget = true;
            IsShowing = true;
            curtain.gameObject.SetActive(true);
            var curtainColor = curtain.color;
            curtainColor.a = 0;
            curtain.color = curtainColor;
            yield return curtain.DOFade(1, timeOpen).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();
            settings.CallbackOpenComplete();
            yield return new WaitForSecondsRealtime(timeWait);
            settings.CallbackClose();
            curtain.raycastTarget = false;
            yield return curtain.DOFade(0, timeClose).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();
            curtain.gameObject.SetActive(false);
            IsShowing = false;
            settings.CallbackFinish();
        }
    }

    public class TransitionSettings
    {
        Action onOpenComplete;

        Action onClose;

        Action onFinished;

        public TransitionSettings OnOpenComplete(Action callback)
        {
            onOpenComplete = callback;
            return this;
        }

        public TransitionSettings OnClose(Action callback)
        {
            onClose = callback;
            return this;
        }

        public TransitionSettings OnFinish(Action callback)
        {
            onFinished = callback;
            return this;
        }

        public void CallbackOpenComplete()
        {
            onOpenComplete?.Invoke();
            onOpenComplete = null;
        }

        public void CallbackClose()
        {
            onClose?.Invoke();
            onClose = null;
        }

        public void CallbackFinish()
        {
            onFinished?.Invoke();
            onFinished = null;
        }
    }
}