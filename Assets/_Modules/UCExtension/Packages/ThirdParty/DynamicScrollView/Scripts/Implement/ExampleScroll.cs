using dynamicscroll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExampleScroll : MonoBehaviour
{
    public DynamicScrollRect verticalScroll;
    public GameObject referenceObject;

    private DynamicScroll<ExampleData, ExampleDynamicObject> mVerticalDynamicScroll = new DynamicScroll<ExampleData, ExampleDynamicObject>();

    public void Start()
    {
        List<ExampleData> listtest = new List<ExampleData>();
        for (int i = 1; i <= 200; i++)
        {
            listtest.Add(new ExampleData() { body = "abc", email = "abc", id = i, name = "abc", postId = 123 });
        }
        mVerticalDynamicScroll.spacing = 5f;
        mVerticalDynamicScroll.Initiate(verticalScroll, listtest.ToArray(), 0, referenceObject);
    }
}
