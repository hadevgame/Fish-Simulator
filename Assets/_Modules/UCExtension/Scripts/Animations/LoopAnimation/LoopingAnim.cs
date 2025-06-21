using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public abstract class LoopingAnim : MonoBehaviour
    {
        [SerializeField] bool autoPlay = true;

        [SerializeField] bool startInEnable;

        public abstract bool MustStartInEnable { get; }

        bool StartInEnable => MustStartInEnable ? true : startInEnable;

        private void Start()
        {
            if (autoPlay && !StartInEnable)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (autoPlay && StartInEnable)
            {
                Play();
            }
        }

        public abstract void Play();

        public abstract void Stop();
    }

}