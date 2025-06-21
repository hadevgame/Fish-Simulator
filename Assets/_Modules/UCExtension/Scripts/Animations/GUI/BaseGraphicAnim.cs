using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public abstract class BaseGraphicAnim : MonoBehaviour
    {
        public bool ControlByGUIAnim = true;

        [SerializeField] protected bool mustWait = true;

        public abstract void PlayAnimation(GraphicState state, Action finish = null);

        public abstract void KillAnimation();

        public abstract void SetState(GraphicState state);
    }
}