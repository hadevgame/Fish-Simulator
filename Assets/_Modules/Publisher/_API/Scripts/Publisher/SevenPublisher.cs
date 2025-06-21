using API.Ads;
using Sirenix.OdinInspector;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using AppLovinMax.Scripts.IntegrationManager.Editor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UCExtension.Common;
using UCExtension;
using System;

public class SevenPublisher : Singleton<SevenPublisher>
{
    public const string SET_INFOR_MENU = "Publisher/SetUpInfors";

    public const string CREATE_PUBLISHER_MENU = "GameObject/Publisher/Create Publisher";

#if UNITY_EDITOR
    [MenuItem(CREATE_PUBLISHER_MENU)]
    public static void Create()
    {
        var exist = FindObjectOfType<SevenPublisher>();
        if (exist)
        {
            EditorGUIUtility.PingObject(exist);
        }
        else
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Modules/Publisher/_API/Prefabs/SevenPublisher.prefab");
            var obj = PrefabUtility.InstantiatePrefab(prefab);
            EditorGUIUtility.PingObject(obj);
            (obj as GameObject).transform.SetAsLastSibling();
        }
    }

    public void SetUpInfors(SheetData data)
    {
        // basic infors
        string product_name = data.GetValue("product_name");
        string package = data.GetValue("package");
        string signing_team_id = data.GetValue("signing_team_id");
        // applovin ids
        string applovin_sdk_key = data.GetValue("applovin_sdk_key");
        string admob_app_id = data.GetValue("admob_app_id");
        string applovin_aoa_id = data.GetValue("applovin_aoa_id");
        string banner_id = data.GetValue("banner_id");
        string inter_id = data.GetValue("inter_id");
        string reward_video_id = data.GetValue("reward_video_id");
        // urls
        string privacy_policy_url = data.GetValue("privacy_policy_url");
        string term_of_services_url = data.GetValue("term_of_services_url");
        // firebase
        googleServiceFileID = data.GetValue("google_service_file_id");
        // Player settings
        PlayerSettings.companyName = product_name;
        PlayerSettings.productName = product_name;
#if UNITY_ANDROID
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, package);
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
#else
        PlayerSettings.iOS.appleDeveloperTeamID = signing_team_id;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, package);
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        PlayerSettings.iOS.targetOSVersionString = "13.0";
#endif
        //
        var admanager = GetComponentInChildren<AdManager>();
        admanager.Configs.Applovin.SdkKey = applovin_sdk_key;
        //"bZncTTqqX6Kebm0dMxUgLHPAC7HVzkQo4iVz2zT32odFJeB7qTQbIf2SfSB9Q65Q3lhWc0EI9OEDhFyxrxo7f2";
        admanager.Configs.Applovin.InterID = inter_id;
        admanager.Configs.Applovin.RewardedID = reward_video_id;
        admanager.Configs.Applovin.BannerID = banner_id;
        // AOA settings
        admanager.Configs.Applovin.AppOpenAdsID = applovin_aoa_id;
        // settings
        admanager.Configs.RemoteConfigKey = $"{package.ToFireBaseEvent()}_{PublisherConst.RemoteKeySuffixes}";
#if UNITY_ANDROID
        admanager.RemoteSettings.IsShowAOA = true;
#else
        admanager.RemoteSettings.IsShowAOA = false;
#endif

        //Admobs app id
        var publishConfiguration = PublishConfiguration.Instance;
#if UNITY_ANDROID
        publishConfiguration.AdMobAndroidAppId = admob_app_id;
#else
        publishConfiguration.AdMobIosAppId = admob_app_id;
#endif
        publishConfiguration.ApplovinSDKKey = applovin_sdk_key;
        publishConfiguration.Settings.PrivacyPolicyUrl = privacy_policy_url;
        publishConfiguration.Settings.TermOfServiceUrl = term_of_services_url;
        EditorUtility.SetDirty(publishConfiguration);
        //save
        this.SetDirtyAndSave();
        EditorUtility.SetDirty(admanager);
        AssetDatabase.SaveAssets();
    }

    [BoxGroup(PublisherConst.PUBLISH_INFORS_GROUP)]
    [Button]
    void SetApplovinInfors()
    {
        EditorApplication.ExecuteMenuItem(SET_INFOR_MENU);
    }

    [Button]
    [BoxGroup(PublisherConst.PUBLISH_INFORS_GROUP)]
    public void UpdateAndroidBundleVersionCode()
    {
        string version = PlayerSettings.bundleVersion;
        PlayerSettings.bundleVersion = version;
        string[] versionSplit = version.Split(".");
        string versionMerge = "";
        foreach (var item in versionSplit)
        {
            versionMerge += item;
        }
        int bundlerVersionCode = int.Parse(versionMerge);
        PlayerSettings.Android.bundleVersionCode = bundlerVersionCode;
    }

    [Button]
    [BoxGroup(PublisherConst.PUBLISH_INFORS_GROUP)]
    public void ResetVersion()
    {
        PlayerSettings.bundleVersion = "1.0.0";
    }

    [Button]
    [BoxGroup(PublisherConst.PUBLISH_INFORS_GROUP)]
    public void UpVersion()
    {
        string version = PlayerSettings.bundleVersion;
        string[] versionSplit = version.Split(".");
        int[] versionInts = new int[versionSplit.Length];
        for (int i = 0; i < versionInts.Length; i++)
        {
            versionInts[i] = int.Parse(versionSplit[i]);
        }
        for (int i = versionInts.Length - 1; i >= 0; i--)
        {
            if (i > 0 && versionInts[i] == 9)
            {
                versionInts[i] = 0;
            }
            else
            {
                versionInts[i]++;
                break;
            }
        }
        version = "";
        for (int i = 0; i < versionInts.Length; i++)
        {
            version += versionInts[i];
            if (i < versionInts.Length - 1)
            {
                version += ".";
            }
        }
        Debug.Log("Up to version: " + version);
        PlayerSettings.bundleVersion = version;
        UpdateAndroidBundleVersionCode();
    }


    [BoxGroup(PublisherConst.OTHERS_GROUP)]
    [HorizontalGroup(PublisherConst.GOOGLE_SERVICE_FILE_GROUP)]
    [SerializeField] string googleServiceFileID;


    [BoxGroup(PublisherConst.OTHERS_GROUP)]
    [HorizontalGroup(PublisherConst.GOOGLE_SERVICE_FILE_GROUP)]
    [Button]
    public void Download()
    {
        if (string.IsNullOrEmpty(googleServiceFileID)) return;
        string savePath = $"Assets/{PublisherConst.GoogleServiceFileName}";
        bool isExistFile = File.Exists(savePath);
        if (isExistFile)
        {
            File.Delete(savePath);
            AssetDatabase.Refresh();
        }
        var link = $"https://drive.google.com/uc?export=download&id={googleServiceFileID}";
        var req = UnityWebRequest.Get(link);
        var op = req.SendWebRequest();
        op.completed += (aop) =>
        {
            DownloadHandlerFile file = req.downloadHandler as DownloadHandlerFile;
            File.WriteAllBytes(savePath, req.downloadHandler.data);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("Download file sucessfully");
        };
    }

    [BoxGroup(PublisherConst.OTHERS_GROUP)]
    [Button]
    public void ResolveAndroidDependencies()
    {

        EditorApplication.ExecuteMenuItem("Assets/External Dependency Manager/Android Resolver/Force Resolve");
    }

    [BoxGroup(PublisherConst.OTHERS_GROUP)]
    [Button("Set Android Manifest Debuggable False")]
    public void FixAndroidManifest()
    {
        string path = "Assets/Plugins/Android/AndroidManifest.xml";
        AndroidManifest manifest = new AndroidManifest(path);
        //var atrributes = manifest.ChildNodes[1].FirstChild.NextSibling.Attributes;
        var atrributes = manifest.GetElementsByTagName("application")[0].Attributes;
        for (int i = 0; i < atrributes.Count; i++)
        {
            var androidDebuggable = atrributes.GetNamedItem("android:debuggable");
            if (androidDebuggable != null)
            {
                androidDebuggable.InnerText = "false";
            }
        }
        manifest.Save();
        AssetDatabase.Refresh();
    }

    [Button]
    [BoxGroup("Ad Manager")]
    void TurnOnCheat()
    {
        var ad = GetComponentInChildren<AdManager>();
        ad.IsCheating = true;
        ad.SetDirtyAndSave();
    }

    [Button]
    [BoxGroup("Ad Manager")]
    void TurnOffCheat()
    {
        var ad = GetComponentInChildren<AdManager>();
        ad.IsCheating = false;
        ad.SetDirtyAndSave();
    }

    [Button]
    [BoxGroup("Ad Manager")]
    public void CopyAdRemoteConfigKey()
    {
        var ad = GetComponentInChildren<AdManager>();
        EditorGUIUtility.systemCopyBuffer = ad.Configs.RemoteConfigKey;
        Debug.Log("Copy success: " + ad.Configs.RemoteConfigKey);
    }

    [Button]
    [BoxGroup("Ad Manager")]
    public void CopyAdRemoteConfigJson()
    {
        var ad = GetComponentInChildren<AdManager>();
        string value = JsonUtility.ToJson(ad.RemoteSettings);
        EditorGUIUtility.systemCopyBuffer = value;
        Debug.Log("Copy success: " + value);
    }
#endif
}

public static class PublisherConst
{
    public const string PUBLISH_INFORS_GROUP = "Publish infors";

    public const string SHEET_LINK_GROUP = PUBLISH_INFORS_GROUP + "/Sheet link";

    public const string OTHERS_GROUP = "Others";

    public const string VERSION_GROUP = OTHERS_GROUP + "/Version";

    public const string GOOGLE_SERVICE_FILE_GROUP = OTHERS_GROUP + "/GOOGLE_SERVICE_FILE_GROUP";

    public static string GoogleServiceFileName
    {
        get
        {
#if UNITY_ANDROID
            return "google-services.json";
#else
            return "GoogleService-Info.plist";
#endif
        }
    }

    public static string RemoteKeySuffixes
    {
        get
        {
#if UNITY_ANDROID
            return "android";
#else
            return "ios";
#endif
        }
    }
}