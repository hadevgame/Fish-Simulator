using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using AppLovinMax.Scripts.IntegrationManager.Editor;

public class PublisherPrebuild : IPreprocessBuildWithReport
{
    private const string AppLovinSettingsExportPath = "MaxSdk/Resources/AppLovinSettings.asset";

    public const string PublishSettingPath = "Publish/PublishConfiguration.asset";

    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Run This Prebuild");
        UpdateApplovinSetting();
    }

    public static void UpdateApplovinSetting()
    {
        var settingsFileName = GetAppLovinSettingsAssetPath();
        Debug.LogError("Setting File Name " + settingsFileName + " " + File.Exists(settingsFileName));
        if (!File.Exists(settingsFileName))
        {
            Debug.LogError("Asset Not Found");
            return;
        }
        var obj = AssetDatabase.LoadAssetAtPath(settingsFileName, Type.GetType("AppLovinSettings, MaxSdk.Scripts.IntegrationManager.Editor"));
        AppLovinSettings appLovinSettings = (AppLovinSettings)obj;
        Debug.Log("Id" + appLovinSettings.AdMobAndroidAppId + " " + appLovinSettings.AdMobIosAppId);
        string settingFilePath = Path.Combine("Assets", PublishSettingPath);
        PublishConfiguration adSetting = AssetDatabase.LoadAssetAtPath<PublishConfiguration>(settingFilePath);
        appLovinSettings.SdkKey = adSetting.ApplovinSDKKey;
        appLovinSettings.AdMobAndroidAppId = adSetting.AdMobAndroidAppId;
        appLovinSettings.AdMobIosAppId = adSetting.AdMobIosAppId;
        EditorUtility.SetDirty(appLovinSettings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //var publishConfiguration = PublishConfiguration.Instance;
        //AppLovinSettings.Instance.AdMobIosAppId = publishConfiguration.AdMobIosAppId;
        //AppLovinSettings.Instance.AdMobAndroidAppId = publishConfiguration.AdMobAndroidAppId;
        //AppLovinSettings.Instance.ConsentFlowEnabled = true;
        //AppLovinSettings.Instance.ConsentFlowTermsOfServiceUrl = publishConfiguration.Settings.TermOfServiceUrl;
        //AppLovinSettings.Instance.ConsentFlowPrivacyPolicyUrl = publishConfiguration.Settings.PrivacyPolicyUrl;
        //AppLovinSettings.Instance.SaveAsync();
        //AppLovinInternalSettings.Instance.ConsentFlowEnabled = true;
        //AppLovinInternalSettings.Instance.ConsentFlowTermsOfServiceUrl = publishConfiguration.Settings.TermOfServiceUrl;
        //AppLovinInternalSettings.Instance.ConsentFlowPrivacyPolicyUrl = publishConfiguration.Settings.PrivacyPolicyUrl;
        //AppLovinInternalSettings.Instance.Save();
    }

    public static string GetAppLovinSettingsAssetPath()
    {
        // Since the settings asset is generated during compile time, the asset label will contain platform specific path separator. So, use platform specific export path.  
        var assetLabel = "l:al_max_export_path-" + AppLovinSettingsExportPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var guids = AssetDatabase.FindAssets(assetLabel);
        var defaultPath = Path.Combine("Assets", AppLovinSettingsExportPath);
        if (guids.Length > 0)
        {
            Debug.LogError("If");
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }
        else
        {
            assetLabel = "l:al_max_export_path-" + AppLovinSettingsExportPath.Replace(Path.AltDirectorySeparatorChar, '/');
            guids = AssetDatabase.FindAssets(assetLabel);
            Debug.LogError("Else " + guids.Length);
            return guids.Length > 0 ? AssetDatabase.GUIDToAssetPath(guids[0]) : defaultPath;
        }
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        throw new NotImplementedException();
    }
}
