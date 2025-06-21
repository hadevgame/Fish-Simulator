using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public class SpinningAnim : LoopingAnim
    {
        [SerializeField] bool spinX;

        [SerializeField] bool spinY;

        [SerializeField] bool spinZ;

        [ShowIf("spinX")]
        [SerializeField] bool reverseX;

        [ShowIf("spinY")]
        [SerializeField] bool reverseY;

        [ShowIf("spinZ")]
        [SerializeField] bool reverseZ;

        [SerializeField] bool alwayUpdate = true;

        public float timeDo = 5;

        Tween playingTween;

        Vector3 rootRotation;
        public override bool MustStartInEnable => false;

        private void Awake()
        {
            rootRotation = transform.localEulerAngles;
        }

        public override void Play()
        {
            Stop();
            Vector3 targetRotate = new Vector3();
            if (spinX)
            {
                targetRotate.x = 360 * GetMult(reverseX);
            }
            if (spinY)
            {
                targetRotate.y = 360 * GetMult(reverseY);
            }
            if (spinZ)
            {
                targetRotate.z = 360 * GetMult(reverseZ);
            }
            playingTween = transform.DOLocalRotate(targetRotate, timeDo, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(alwayUpdate);
        }

        public override void Stop()
        {
            playingTween.Kill();
            transform.localEulerAngles = rootRotation;
        }

        float GetMult(bool reverse)
        {
            if (reverse) return -1;
            return 1;
        }
    }
}