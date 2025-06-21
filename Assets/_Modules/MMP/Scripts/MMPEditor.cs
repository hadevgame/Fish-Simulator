using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MMPEditor : MonoBehaviour
{
#if UNITY_EDITOR
    const string apiKey = "AIzaSyBqDNy_IF7b1jgCLWOz2rZ-7suuWQUD8F4";

#if UNITY_ANDROID
    const string sheetID = "1vIV_9pizxnzfd74oBPffu2G_d0NoXZUVvSzqEmdVNvM";
#else
    const string sheetID = "1XyqX0eLlwl3VKerfGVamaM-F-vJTgMbRLzaSYicYDU0";
#endif

    string SheetName
    {
        get
        {
#if UNITY_ANDROID
            return sheetNameAndroid;
#else
            return sheetNameIOS;
#endif
        }
    }

#if UNITY_ANDROID
    [BoxGroup("Read Infors")]
    [HorizontalGroup("Read Infors/Sheet")]
    [SerializeField] string sheetNameAndroid = "";

#else
    [BoxGroup("Read Infors")]
    [HorizontalGroup("Read Infors/Sheet")]
    [SerializeField] string sheetNameIOS = "";
#endif


    [BoxGroup("Read Infors")]
    [HorizontalGroup("Read Infors/Sheet", 100)]
    [Button]
    public void OpenSheet()
    {
        string sheetLink = @$"https://docs.google.com/spreadsheets/d/{sheetID}/edit#gid=356163562";
        Application.OpenURL(sheetLink);
    }

    [BoxGroup("Read Infors")]
    [HorizontalGroup("Read Infors/Sheet", 100)]
    [Button]
    public void TestLink()
    {
        SheetReader.OpenLink(apiKey, sheetID, SheetName);
    }

    [BoxGroup("Read Infors")]
    [Button]
    public void ReadInfors()
    {
        EditorApplication.ExecuteMenuItem(SevenPublisher.CREATE_PUBLISHER_MENU);
        SheetReader.ReadFromSheet(apiKey, sheetID, SheetName, data =>
        {
            // read infors
            string appflyer_dev_key = data.GetValue("appflyer_dev_key");
            string apple_store_id = data.GetValue("apple_store_id");
            //settings
            AppsFlyerObjectScript afObject = GetComponentInChildren<AppsFlyerObjectScript>();
            afObject.devKey = appflyer_dev_key;
#if UNITY_IOS
            afObject.appID = apple_store_id;
#endif
            // publisher
            var publisher = FindObjectOfType<SevenPublisher>();
            publisher.SetUpInfors(data);
            EditorUtility.SetDirty(afObject);
            AssetDatabase.SaveAssets();
        });
    }

    [MenuItem("GameObject/Publisher/Create MMP")]
    public static void Create()
    {
        var exist = FindObjectOfType<MMPEditor>();
        if (exist)
        {
            EditorGUIUtility.PingObject(exist);
        }
        else
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Modules/MMP/Prefabs/MMP.prefab");
            var obj = PrefabUtility.InstantiatePrefab(prefab);
            EditorGUIUtility.PingObject(obj);
            (obj as GameObject).transform.SetAsLastSibling();
        }
    }

#endif
}
