using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UCExtension.GUI;
using System;
public class NotifiGUI : BaseGUI
{
    public static NotifiGUI Instance;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    private CanvasGroup canvasGroup;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite valid;
    [SerializeField] private Sprite invalid;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        }

        popupPanel.SetActive(false);
        canvasGroup.alpha = 0;
    }
    public void ShowPopup(string message, Color? color = null, float duration = 2f, Action callback = null)
    {
        popupText.enableAutoSizing = false;
        popupText.enableWordWrapping = false;
        popupText.text = message;

        if (color.HasValue)
        {
            popupText.color = color.Value;
            icon.sprite = invalid;
        }
        else 
        {
            popupText.color = Color.green; 
            icon.sprite = valid;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(popupText.rectTransform);

        float preferredWidth = popupText.preferredWidth;
        float targetWidth = preferredWidth + 150f;

        RectTransform panelRect = popupPanel.GetComponent<RectTransform>();
        Vector2 size = panelRect.sizeDelta;
        size.x = targetWidth;
        panelRect.sizeDelta = size;

        popupPanel.SetActive(true);
        canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutCubic);

        DOVirtual.DelayedCall(duration, () =>
        {
            canvasGroup.DOFade(0, 0.5f)
                .SetEase(Ease.InCubic)
                .OnComplete(() => 
                {
                    popupPanel.SetActive(false);
                    callback?.Invoke();
                });
        });
    }
}
