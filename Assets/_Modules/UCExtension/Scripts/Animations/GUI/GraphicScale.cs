using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public class GraphicScale : GraphicAnim<ScaleConfigs>
    {
        Tween doingTween;

        public override void KillAnimation()
        {
            doingTween?.Kill();
        }

        public override void PlayAnimation(GraphicState state, Action finish)
        {
            if (!mustWait)
            {
                finish?.Invoke();
            }
            transform.localScale = GetOpositeConfigs(state).Scale;
            ScaleConfigs targetConfig = GetConfigs(state);
            KillAnimation();
            doingTween = DOVirtual.DelayedCall(delayPlay, () =>
            {
                doingTween = transform.DOScale(targetConfig.Scale, targetConfig.TimeDo)
                    .SetEase(targetConfig.Ease)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        if (mustWait)
                        {
                            finish?.Invoke();
                        }
                    });
            }).SetUpdate(true);
        }

        protected override void ApplyConfigs(ScaleConfigs configs)
        {
            transform.localScale = configs.Scale;
        }

#if UNITY_EDITOR
        protected override void GetCurrentConfigs(ScaleConfigs configs)
        {
            configs.Scale = transform.localScale;
        }

        protected override void SetBasicConfigs()
        {
            visibleConfigs = new ScaleConfigs();
            hiddenConfigs = new ScaleConfigs();
            visibleConfigs.Scale = Vector3.one;
            hiddenConfigs.Scale = Vector3.zero;
            visibleConfigs.Ease = Ease.OutBack;
            hiddenConfigs.Ease = Ease.InBack;
            base.SetBasicConfigs();
        }

        protected override void Save()
        {
            transform.SetDirtyAndSave();
        }
#endif
    }

    [System.Serializable]
    public class ScaleConfigs : GraphicAnimConfigs
    {
        public Vector3 Scale;
    }
}