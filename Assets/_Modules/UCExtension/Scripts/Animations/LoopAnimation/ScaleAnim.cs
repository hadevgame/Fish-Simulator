using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace UCExtension.Animation
{
    public class ScaleAnim : LoopingAnim
    {
        [SerializeField] Vector3 originalScale = Vector3.one;

        [SerializeField] Vector3 targetScale = Vector3.one * 0.9f;

        [SerializeField] float time = 0.5f;

        [SerializeField] float time2 = 0.5f;

        Sequence seq;
        public override bool MustStartInEnable => false;

        public override void Play()
        {
            transform.localScale = originalScale;
            seq = DOTween.Sequence();
            seq.Insert(0, transform.DOScale(targetScale, time).SetEase(Ease.InOutSine));
            seq.Insert(time, transform.DOScale(originalScale, time2).SetEase(Ease.InOutSine));
            seq.SetLoops(-1).SetUpdate(true);
        }

        public override void Stop()
        {
            seq?.Kill();
            transform.localScale = originalScale;
        }
    }
}