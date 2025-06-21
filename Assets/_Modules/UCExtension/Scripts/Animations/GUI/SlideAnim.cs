using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UCExtension.Animation
{
    //public class SlideAnim : GUIAnim
    //{
    //    [SerializeField] SlideAnimDirection openDirection;

    //    [SerializeField] SlideAnimDirection closeDirection;

    //    [SerializeField] Ease openEase;

    //    [SerializeField] Ease closeEase;

    //    [SerializeField] RectTransform rootMotion;

    //    [SerializeField] float timeMove = 0.5f;

    //    Vector3 rootAnchorPosition;

    //    private void Awake()
    //    {
    //        rootAnchorPosition = rootMotion.anchoredPosition;
    //    }

    //    protected override void Open(UnityAction finish = null)
    //    {
    //        DOTween.Kill(GetInstanceID());
    //        Vector2 anchorPos = GetPosition(openDirection);
    //        rootMotion.anchoredPosition = anchorPos;
    //        rootMotion.DOAnchorPos(rootAnchorPosition, timeMove).OnComplete(() =>
    //        {
    //            finish?.Invoke();
    //        }).SetUpdate(true).SetEase(openEase).SetId(GetInstanceID());
    //    }

    //    protected override void Close(UnityAction finish = null)
    //    {
    //        DOTween.Kill(GetInstanceID());
    //        Vector2 anchorPos = GetPosition(closeDirection);
    //        rootMotion.anchoredPosition = rootAnchorPosition;
    //        rootMotion.DOAnchorPos(anchorPos, timeMove).OnComplete(() =>
    //        {
    //            finish?.Invoke();
    //        }).SetUpdate(true).SetEase(closeEase).SetId(GetInstanceID());
    //    }

    //    public Vector2 GetPosition(SlideAnimDirection direction)
    //    {
    //        switch (direction)
    //        {
    //            case SlideAnimDirection.Left:
    //                return Vector2.left * Screen.width;
    //            case SlideAnimDirection.Right:
    //                return Vector2.right * Screen.width;
    //            case SlideAnimDirection.Up:
    //                return Vector2.up * Screen.height;
    //            case SlideAnimDirection.Down:
    //            default:
    //                return Vector2.down * Screen.height;
    //        }
    //    }
    //}

    //public enum SlideAnimDirection
    //{
    //    Left,
    //    Right,
    //    Up,
    //    Down
    //}
}