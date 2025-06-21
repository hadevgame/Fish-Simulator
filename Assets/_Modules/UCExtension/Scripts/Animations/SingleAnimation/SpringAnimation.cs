using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace UCExtension.Animation
{
    public class SpringAnimation : SingleAnimation
    {
        [SerializeField] bool playOnEnable;

        [SerializeField] float timeScaleUp = 0.1f;

        [SerializeField] float timeScaleDown = 0.2f;

        [SerializeField] Vector3 startScale = Vector3.one;

        [SerializeField] Vector3 scaleUp = Vector3.one * 1.2f;

        [SerializeField] Vector3 originalScale = Vector3.one;

        [SerializeField] AudioClip sound;

        Tween animTween;

        private void OnEnable()
        {
            if (playOnEnable)
            {
                Play();
            }
        }

        public override void Play(UnityAction finish = null)
        {
            Stop();
            if (AudioManager.Ins)
            {
                AudioManager.Ins.PlaySFX(sound);
            }
            transform.localScale = startScale;
            animTween = transform.DOScale(scaleUp, timeScaleUp).SetUpdate(true).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                animTween = transform.DOScale(originalScale, timeScaleDown).SetUpdate(true).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    finish?.Invoke();
                });
            });
        }

        public override void Stop()
        {
            animTween?.Complete();
        }

        [Button]
        void SetUpScale(float scale = 1.2f)
        {
            originalScale = transform.localScale;
            scaleUp = originalScale * scale;
        }
    }
}