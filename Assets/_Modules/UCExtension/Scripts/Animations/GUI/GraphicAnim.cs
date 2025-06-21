using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public abstract class GraphicAnim<T> : BaseGraphicAnim where T : GraphicAnimConfigs
    {
        [SerializeField] protected float delayPlay;

        [FoldoutGroup("Visible")]
        [SerializeField] protected T visibleConfigs;

        [FoldoutGroup("Hidden")]
        [SerializeField] protected T hiddenConfigs;

        protected T GetOpositeConfigs(GraphicState state)
        {
            switch (state)
            {
                case GraphicState.Visible:
                    return hiddenConfigs;
                case GraphicState.Hidden:
                default:
                    return visibleConfigs;
            }
        }

        protected T GetConfigs(GraphicState state)
        {
            switch (state)
            {
                case GraphicState.Visible:
                    return visibleConfigs;
                case GraphicState.Hidden:
                default:
                    return hiddenConfigs;
            }
        }

        protected abstract void ApplyConfigs(T configs);

        public override void SetState(GraphicState state)
        {
            ApplyConfigs(GetConfigs(state));
        }

#if UNITY_EDITOR
        private void Reset()
        {
            SetBasicConfigs();
        }

        [Button]
        protected virtual void SetBasicConfigs()
        {
            visibleConfigs.TimeDo = 0.5f;
            hiddenConfigs.TimeDo = 0.5f;
            this.SetDirtyAndSave();
        }

        protected abstract void GetCurrentConfigs(T configs);

        [Button("GetConfigs")]
        [FoldoutGroup("Visible")]
        protected void GetVisibleConfigs()
        {
            GetCurrentConfigs(visibleConfigs);
            this.SetDirtyAndSave();
        }

        [Button("GetConfigs")]
        [FoldoutGroup("Hidden")]
        protected void GetHiddenConfigs()
        {
            GetCurrentConfigs(hiddenConfigs);
            this.SetDirtyAndSave();
        }

        protected abstract void Save();

        [Button("Set State")]
        [FoldoutGroup("Visible")]
        protected void SetVisibleStateEditor()
        {
            SetState(GraphicState.Visible);
            Save();
        }

        [Button("Set State")]
        [FoldoutGroup("Hidden")]
        protected void SetHiddenStateEditor()
        {
            SetState(GraphicState.Hidden);
            Save();
        }


#endif
    }

    [System.Serializable]
    public class GraphicAnimConfigs
    {
        public Ease Ease;

        public float TimeDo = 0.5f;
    }

    public enum GraphicState
    {
        Visible,
        Hidden
    }
}