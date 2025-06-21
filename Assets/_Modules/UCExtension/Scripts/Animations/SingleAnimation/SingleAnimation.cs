using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UCExtension.Animation
{
    public abstract class SingleAnimation : MonoBehaviour
    {
        public abstract void Play(UnityAction finish = null);

        public abstract void Stop();
    }
}