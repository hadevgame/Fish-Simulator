using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Animation;
using UnityEngine;

public class ShakingAnim : LoopingAnim
{
    [SerializeField] float timeDo;

    [SerializeField] float strength;

    Vector3 rootPosition;

    Tween tween;
    public override bool MustStartInEnable => false;

    private void Awake()
    {
        rootPosition = transform.localPosition;
    }

    public override void Play()
    {
        tween = transform.DOShakePosition(timeDo, strength)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    public override void Stop()
    {
        tween?.Kill();
        transform.localPosition = rootPosition;
    }
}
