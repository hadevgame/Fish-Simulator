using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public class FadingSpriteRenderer : LoopingAnim
    {
        [SerializeField] SpriteRenderer mainSR;

        [SerializeField] float timeDo = 1f;

        [SerializeField] float timeDo2 = 1f;

        [SerializeField] float fadeFull = 1;

        [SerializeField] float fade = 0.5f;

        [SerializeField] int loop;

        public override bool MustStartInEnable => false;

        public override void Play()
        {
            mainSR.SetAlpha(fade);
            Sequence seq = DOTween.Sequence();
            seq.Insert(0, mainSR.DOFade(fadeFull, timeDo).SetEase(Ease.InOutSine));
            seq.Insert(timeDo, mainSR.DOFade(fade, timeDo2).SetEase(Ease.InOutSine));
            seq.SetLoops(loop).SetId(GetInstanceID()).SetUpdate(true);
        }

        public override void Stop()
        {
            DOTween.Kill(GetInstanceID());
            mainSR.SetAlpha(fadeFull);
        }
    }
}