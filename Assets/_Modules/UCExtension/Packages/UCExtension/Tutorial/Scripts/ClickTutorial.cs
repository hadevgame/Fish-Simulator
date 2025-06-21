using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClickTutorial : RecyclableObject
{
    [SerializeField] RectTransform root;

    [SerializeField] RectTransform mask;

    [SerializeField] Animator handAnim;

    UnityAction onFinish;

    Button currentButton;

    public void Show(Button button, UnityAction finish)
    {
        root.transform.localScale = Vector3.one;
        root.anchorMin = Vector2.zero;
        root.anchorMax = new Vector2(1, 1);
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;
        Debug.Log("Root: " + root.offsetMin);
        currentButton = button;
        onFinish = finish;
        button.onClick.AddListener(FinishTutorial);
        handAnim.SetTrigger("Play");
        Debug.Log($"Tutorial for {button.name}: ");
        var buttonTrans = button.GetComponent<RectTransform>();
        mask.sizeDelta = buttonTrans.sizeDelta;
        Debug.Log("Button Position: " + button.transform.position);
        mask.transform.position = button.transform.position;
        Debug.Log("Mask Position: " + mask.transform.position);
    }

    void FinishTutorial()
    {
        onFinish?.Invoke();
        Recycle();
        currentButton.onClick.RemoveListener(FinishTutorial);
    }
}

[System.Serializable]
public class ListClickTutorialData
{
    public List<ClickTutorialData> Steps;
}

[System.Serializable]
public class ClickTutorialData
{
    public Vector2 AnchoredPosition;

    public Vector2 AnchorMin;

    public Vector2 AnchorMax;

    public Vector2 SizeDelta;
}