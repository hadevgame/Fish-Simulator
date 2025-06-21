using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PublishConfiguration : ScriptableObject
{
    public string ApplovinSDKKey;

    public string AdMobAndroidAppId;

    public string AdMobIosAppId;

    public PublishConsentSettings Settings
    {
        get
        {
#if UNITY_ANDROID
            return SettingsAndroid;
#else
            return SettingsIOS;
#endif
        }
    }

#if UNITY_ANDROID
    public PublishConsentSettings SettingsAndroid;
#else
    public PublishConsentSettings SettingsIOS;
#endif

#if UNITY_EDITOR

    public const string FileFolder = "Publish";

    public const string FileName = "PublishConfiguration.asset";

    public static string AssetPath => Path.Combine("Assets", FileFolder, FileName);

    public static string FilePath => Path.Combine(Application.dataPath, FileFolder, FileName);

    public static string FolderPath => Path.Combine(Application.dataPath, FileFolder);

    public static PublishConfiguration Instance
    {
        get
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            if (!File.Exists(FilePath))
            {
                PublishConfiguration newConfiguration = ScriptableObject.CreateInstance<PublishConfiguration>();
                AssetDatabase.CreateAsset(newConfiguration, AssetPath);
                EditorUtility.SetDirty(newConfiguration);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorGUIUtility.PingObject(newConfiguration);
            }
            PublishConfiguration instance = AssetDatabase.LoadAssetAtPath<PublishConfiguration>(AssetPath);
            return instance;
        }
    }
#endif
}

[System.Serializable]
public class PublishConsentSettings
{
    public string PrivacyPolicyUrl;

    public string TermOfServiceUrl;
}
