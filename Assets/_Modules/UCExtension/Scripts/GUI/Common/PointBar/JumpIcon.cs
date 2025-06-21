using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class JumpIcon : RecyclableObject
{
    [SerializeField] Image iconImage;

    [SerializeField] float timeCollect;

    public void SetIcon(Sprite icon, bool setNative = true)
    {
        iconImage.sprite = icon;
        if (setNative)
        {
            iconImage.SetNativeSize();
        }
    }

    public void Jump(Vector3 collectPosition, Transform point, float timeScaleUp, float timeCollect, Action finish)
    {
        transform.position = collectPosition;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, timeScaleUp).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.DOMove(point.position, timeCollect).SetEase(Ease.Linear).OnComplete(() =>
            {
                Recycle();
                finish?.Invoke();
            });
        });
    }
}
