
using System;
using System.IO;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using UnityEditor;
public class SevenPublisherEditor
{
    private const string AppLovinSettingsExportPath = "MaxSdk/Resources/AppLovinSettings.asset";

    public const string AdsSettingPath = "AdSetting/AdSetting.asset";

    [MenuItem(SevenPublisher.SET_INFOR_MENU)]
    public static void SetUpInfors()
    {
        var publishConfiguration = PublishConfiguration.Instance;
        AppLovinSettings.Instance.SdkKey = publishConfiguration.ApplovinSDKKey;
        AppLovinSettings.Instance.AdMobIosAppId = publishConfiguration.AdMobIosAppId;
        AppLovinSettings.Instance.AdMobAndroidAppId = publishConfiguration.AdMobAndroidAppId;
        //AppLovinSettings.Instance.ConsentFlowEnabled = true;
        //AppLovinSettings.Instance.ConsentFlowTermsOfServiceUrl = publishConfiguration.Settings.TermOfServiceUrl;
        //AppLovinSettings.Instance.ConsentFlowPrivacyPolicyUrl = publishConfiguration.Settings.PrivacyPolicyUrl;
        AppLovinSettings.Instance.SaveAsync();
        AppLovinInternalSettings.Instance.ConsentFlowEnabled = true;
        AppLovinInternalSettings.Instance.ConsentFlowTermsOfServiceUrl = publishConfiguration.Settings.TermOfServiceUrl;
        AppLovinInternalSettings.Instance.ConsentFlowPrivacyPolicyUrl = publishConfiguration.Settings.PrivacyPolicyUrl;
        AppLovinInternalSettings.Instance.Save();
        //
    }
}
