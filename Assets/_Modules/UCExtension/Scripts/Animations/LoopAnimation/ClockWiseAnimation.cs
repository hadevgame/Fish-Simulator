using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public class ClockWiseAnimation : LoopingAnim
    {
        [SerializeField] float timeDo = 0.5f;

        [SerializeField] bool reverse;

        [SerializeField] Vector3 startAngle = new Vector3(0, 0, -15f);

        [SerializeField] Vector3 endAngle = new Vector3(0, 0, 15f);

        [SerializeField] Ease ease = Ease.InOutSine;

        Vector3 StartAngle => reverse ? endAngle : startAngle;

        Vector3 EndAngle => reverse ? startAngle : endAngle;

        Sequence currentSeq;

        float rootAngle;

        public override bool MustStartInEnable => false;

        public override void Play()
        {
            transform.localEulerAngles = StartAngle;
            currentSeq = DOTween.Sequence();
            currentSeq.Insert(0, transform.DOLocalRotate(EndAngle, timeDo).SetEase(ease));
            currentSeq.Insert(timeDo, transform.DOLocalRotate(StartAngle, timeDo).SetEase(ease));
            currentSeq.SetLoops(-1).SetUpdate(true);
        }

        public override void Stop()
        {
            currentSeq.Kill();
            transform.SetLocalEulerAngleZ(rootAngle);
        }
    }
}