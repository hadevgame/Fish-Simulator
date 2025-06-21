using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class BaseTab<T> : MonoBehaviour
{
    [SerializeField] Text titleText;

    [SerializeField] Image bgImage;

    [SerializeField] Sprite selectedSprite;

    [SerializeField] Sprite unselectedSprite;

    [SerializeField] bool setBGNativeSize;

    bool isSelected = false;

    public T Data { get; private set; }

    public Action<BaseTab<T>> OnSelect;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        if (isSelected) return;
        OnSelect?.Invoke(this);
    }

    public virtual void SetData(T data)
    {
        this.Data = data;
    }

    public void SetSelect(bool isSelect)
    {
        this.isSelected = isSelect;
        bgImage.sprite = isSelect ? selectedSprite : unselectedSprite;
        if (setBGNativeSize)
        {
            bgImage.SetNativeSize();
        }
    }

    public void SetTitle(string title)
    {
        this.titleText.text = title.Replace("_", "\n");
    }
}
