using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAnim : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    public void Show(float duration = 0.5f)
    {
        if (canvasGroup == null || rectTransform == null) return;

        rectTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1, duration);
        rectTransform.DOScale(1, duration).SetEase(Ease.OutBack);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide(float duration = 0.5f)
    {
        if (canvasGroup == null || rectTransform == null) return;

        rectTransform.DOScale(0, duration).SetEase(Ease.InBack);
        canvasGroup.DOFade(0, duration).OnComplete(() => {
        });

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void StartPulse(Transform target, float scaleAmount = 1.05f, float duration = 0.5f)
    {
        if (target == null) return;

        target.DOScale(scaleAmount, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

}
