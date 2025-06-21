using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    [RequireComponent(typeof(RectTransform))]
    public class GraphicMove : GraphicAnim<MoveConfigs>
    {
        RectTransform target;

        Tween moveTween;

        private void Awake()
        {
            target = GetComponent<RectTransform>();
        }

        void Move(MoveConfigs config, Action finish = null)
        {

            moveTween?.Kill();
            moveTween = DOVirtual.DelayedCall(delayPlay, () =>
            {
                moveTween = target.DOAnchorPos(config.AnchorPosition, config.TimeDo)
                    .SetEase(config.Ease)
                    .SetUpdate(true)
                    .OnComplete(() =>
                {
                    finish?.Invoke();
                });
            });
        }

        public override void PlayAnimation(GraphicState state, Action finish)
        {
            target.anchoredPosition = GetOpositeConfigs(state).AnchorPosition;
            Move(GetConfigs(state), finish);
        }

        public override void KillAnimation()
        {
            moveTween?.Kill();
        }

        protected override void ApplyConfigs(MoveConfigs configs)
        {
            if (target)
            {
                target.anchoredPosition = configs.AnchorPosition;
            }
            else
            {
                var rect = GetComponent<RectTransform>();
                rect.anchoredPosition = configs.AnchorPosition;
            }
        }


#if UNITY_EDITOR
        protected override void GetCurrentConfigs(MoveConfigs configs)
        {
            configs.AnchorPosition = GetComponent<RectTransform>().anchoredPosition;
        }

        protected override void SetBasicConfigs()
        {
            hiddenConfigs.Ease = Ease.InBack;
            visibleConfigs.Ease = Ease.OutBack;
            base.SetBasicConfigs();
        }

        protected override void Save()
        {
            var rect = GetComponent<RectTransform>();

            rect.SetDirtyAndSave();
        }

        public override void SetState(GraphicState state)
        {
            base.SetState(state);
        }

#endif
    }

    [System.Serializable]
    public class MoveConfigs : GraphicAnimConfigs
    {
        public Vector2 AnchorPosition;
    }
}