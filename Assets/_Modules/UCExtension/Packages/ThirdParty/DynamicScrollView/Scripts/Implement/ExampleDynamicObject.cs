using dynamicscroll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleDynamicObject : DynamicScrollObject<ExampleData>
{
    public override float CurrentHeight { get; set; }
    public override float CurrentWidth { get; set; }

    private Text idText;
    private Text nameEmailText;
    private Text bodyText;

    public void Awake()
    {
        CurrentHeight = GetComponent<RectTransform>().rect.height;
        CurrentWidth = GetComponent<RectTransform>().rect.width;

        idText = transform.Find("PostId").GetComponent<Text>();
        nameEmailText = transform.Find("NameEmail").GetComponent<Text>();
        bodyText = transform.Find("Body").GetComponent<Text>();
    }

    public override void UpdateScrollObject(ExampleData item, int index)
    {
        base.UpdateScrollObject(item, index);

        idText.text = item.id.ToString();
        nameEmailText.text = string.Format("{0} ({1})", item.name, item.email);
        bodyText.text = item.body;
    }
}
