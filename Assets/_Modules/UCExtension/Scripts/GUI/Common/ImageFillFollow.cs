using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class ImageFillFollow : MonoBehaviour
{
    [SerializeField] Image followImage;

    RectTransform rect;

    RectTransform imageRect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        imageRect = followImage.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rect.anchoredPosition = GetAnchorPos();
    }

    Vector2 GetAnchorPos()
    {
        return new Vector2(imageRect.sizeDelta.x * followImage.fillAmount - imageRect.sizeDelta.x / 2, 0);
    }
}
