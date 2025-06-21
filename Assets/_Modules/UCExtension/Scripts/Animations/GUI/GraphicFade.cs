using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GraphicFade : GraphicAnim<GraphicFadeConfig>
    {
        CanvasGroup group; 

        Tween doingTween;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        public override void KillAnimation()
        {
            doingTween?.Kill();
        }

        public override void PlayAnimation(GraphicState state, Action finish = null)
        {
            group.alpha = GetOpositeConfigs(state).Alpha;
            Fade(GetConfigs(state), finish);
        }

        void Fade(GraphicFadeConfig config, Action finish)
        {
            group.DOFade(config.Alpha, config.TimeDo)
                .SetEase(config.Ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    finish?.Invoke();
                });
        }

        protected override void ApplyConfigs(GraphicFadeConfig configs)
        {
            if (group)
            {
                group.alpha = configs.Alpha;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = configs.Alpha;
            }
        }


#if UNITY_EDITOR
        protected override void GetCurrentConfigs(GraphicFadeConfig configs)
        {
            configs.Alpha = GetComponent<CanvasGroup>().alpha;
        }

        protected override void Save()
        {
            GetComponent<CanvasGroup>().SetDirtyAndSave();
        }
        protected override void SetBasicConfigs()
        {
            hiddenConfigs.Alpha = 0;
            visibleConfigs.Alpha = 1;
            visibleConfigs.Ease = Ease.Linear;
            hiddenConfigs.Ease = Ease.Linear;
            base.SetBasicConfigs();
        }
#endif
    }

    [System.Serializable]
    public class GraphicFadeConfig : GraphicAnimConfigs
    {
        public float Alpha;
    }
}