using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UCExtension.Animation
{
    public class YoYoAnimation : LoopingAnim
    {
        [SerializeField] bool reverse;

        [SerializeField] float distance = 0.1f;

        [SerializeField] float timeDo1 = 1f;

        [SerializeField] float timeDo2 = 1f;

        [SerializeField] float timeDelay = 0f;

        [SerializeField] bool useStay;

        [ShowIf("useStay")]
        [SerializeField] float timeStay1 = 0f;

        [ShowIf("useStay")]
        [SerializeField] float timeStay2 = 0f;

        [SerializeField] float gizmosSize = 0.1f;

        [SerializeField] Ease ease = Ease.InOutSine;

        [SerializeField] AnimAxis axis = AnimAxis.Y;

        [SerializeField] AnimPivot pivot = AnimPivot.Positive;

        [SerializeField] LoopType loopType = LoopType.Yoyo;

        [SerializeField] bool alwayUpdate = true;

        Tween playingTween;

        float TimeDo1;

        public override bool MustStartInEnable => true;

        Vector3 root;

        private void Awake()
        {
            root = transform.localPosition;
        }

        private void OnDrawGizmos()
        {
            if (transform.parent)
            {
                foreach (var item in GetMovePath())
                {
                    var point = transform.parent.TransformPoint(item);
                    Gizmos.DrawSphere(point, gizmosSize);
                }
            }
        }

        public override void Play()
        {
            StartCoroutine(IEPlay());
        }

        IEnumerator IEPlay()
        {
            transform.localPosition = root;
            var path = GetMovePath();
            transform.localPosition = path[0];
            if (alwayUpdate)
            {
                yield return new WaitForSecondsRealtime(timeDelay);
            }
            else
            {
                yield return new WaitForSeconds(timeDelay);
            }
            while (true)
            {
                yield return IEMove(path[1], timeDo1, timeStay1);
                yield return IEMove(path[0], timeDo2, timeStay2);
            }
        }

        IEnumerator IEMove(Vector3 position, float timeDo, float timeStay)
        {
            playingTween = transform.DOLocalMove(position, timeDo)
                    .SetEase(ease)
                    .SetUpdate(alwayUpdate);
            yield return playingTween.WaitForCompletion();
            if (useStay)
            {
                if (alwayUpdate)
                {
                    yield return new WaitForSecondsRealtime(timeStay);
                }
                else
                {
                    yield return new WaitForSeconds(timeStay);
                }
            }
        }

        Vector3[] GetMovePath()
        {
            Vector3 negativePos = transform.localPosition;
            Vector3 positivePos = transform.localPosition;
            float deltaNegative = 0;
            float deltaPositive = 0;
            switch (pivot)
            {
                case AnimPivot.Positive:
                    deltaNegative = 0;
                    deltaPositive = distance;
                    break;
                case AnimPivot.Center:
                    deltaNegative = -distance / 2;
                    deltaPositive = distance / 2;
                    break;
                case AnimPivot.Negative:
                    deltaNegative = -distance;
                    deltaPositive = 0;
                    break;
                default:
                    break;
            }
            switch (axis)
            {
                case AnimAxis.X:
                    negativePos.x += deltaNegative;
                    positivePos.x += deltaPositive;
                    break;
                case AnimAxis.Y:
                    negativePos.y += deltaNegative;
                    positivePos.y += deltaPositive;
                    break;
                case AnimAxis.Z:
                    negativePos.z += deltaNegative;
                    positivePos.z += deltaPositive;
                    break;
                default:
                    break;
            }
            Vector3[] path = new Vector3[2];
            if (reverse)
            {
                path[0] = positivePos;
                path[1] = negativePos;
            }
            else
            {
                path[0] = negativePos;
                path[1] = positivePos;
            }
            return path;
        }

        public override void Stop()
        {
            playingTween?.Kill();
            StopAllCoroutines();
        }
    }
}

public enum AnimPivot
{
    Positive,
    Center,
    Negative,
}
public enum AnimAxis
{
    X,
    Y,
    Z
}