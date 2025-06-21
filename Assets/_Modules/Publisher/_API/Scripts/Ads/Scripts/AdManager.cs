using API.RemoteConfig;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Security.Cryptography;
using UCExtension;
using UCExtension.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace API.Ads
{
    public class AdManager : Singleton<AdManager>
    {
        public bool IsCheating;

        [SerializeField] bool showLog = true;

        public AdManagerConfig Configs
        {
            get
            {
#if UNITY_ANDROID
                return AndroidConfigs;
#else
                return IOSConfigs;
#endif
            }
        }

#if UNITY_ANDROID
        public AdManagerConfig AndroidConfigs;
#else
        public AdManagerConfig IOSConfigs;
#endif

        public AdsRemoteSetting RemoteSettings;

        private bool IsHideBanner = false;

        public static Action OnInterShowed;

        public static Action OnVideoShowed;

        public static Action OnInitilized;

        // param 1: ads type, param 2: location, param 3: Has ads
        public static Action<string, string, bool> OnCallShowAds;

        public static Action<string, string> OnShowAdsSucess;

        public static Action<string, string> OnClickAds;

        public static Action<MaxSdkBase.AdInfo> OnApplovinAdsRevenuePaid;

        //public static Action OnCallMaxApplovinShowAds;

        //public static Action OnAdsDismissed;

        public bool SettingComplete { get; private set; }

        public bool IsAdsInited { get; private set; }

        public bool IsAdsLeaveApp { get; private set; }

        float timeFirstLoadAOA;

        protected override void SetUp()
        {
            base.SetUp();
            // Register events
            RemoteConfigManager.OnFetchComplete += OnFirebaseInitComplete;
            //
            RemoteSettings = PlayerPrefsExtension.GetValue(AdPlayerPrefsKeys.Settings, RemoteSettings);
            InitAds();
            WaitAOA();
        }

        void LogID()
        {
            Log("Applovin id: " + Configs.Applovin.SdkKey);
            Log("Show full id: " + Configs.Applovin.InterID);
            Log("RV id: " + Configs.Applovin.RewardedID);
            Log("Banner id: " + Configs.Applovin.BannerID);
            Log("AOA id: " + Configs.Applovin.AppOpenAdsID);
        }

        private void OnFirebaseInitComplete()
        {
            string remoteSettingStr = RemoteConfigHelper.GetString(Configs.RemoteConfigKey);
            remoteSettingStr = remoteSettingStr.CheckCorrect();
            Log("Firebase init complete");
            Log("Key " + Configs.RemoteConfigKey + ": " + remoteSettingStr);
            if (string.IsNullOrEmpty(remoteSettingStr))
            {
                RemoteSettings = PlayerPrefsExtension.GetValue(AdPlayerPrefsKeys.Settings, RemoteSettings);
            }
            else
            {
                RemoteSettings = JsonUtility.FromJson<AdsRemoteSetting>(remoteSettingStr);
                PlayerPrefsExtension.SetValue(AdPlayerPrefsKeys.Settings, RemoteSettings);
            }
            ResetTimeShowFull();
            SettingComplete = true;
        }

        private void InitAds()
        {
            Log("InitAds");
            LogID();
            string timeData = PlayerPrefs.GetString("LastTimeRefocusShow", "O");
            ResetTimeShowFull();
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                RequestRewardBasedVideo();
                RequestFull();
                RequestApplovinBanner();
                if (RemoteSettings.IsBannerInStart && !IsHideBanner)
                {
                    Log("IsBannerInStart");
                }
                RequestApplovinAppOpen();
                IsAdsInited = true;
                Log("AdsInited");
                OnInitilized?.Invoke();
            };
            MaxSdk.InitializeSdk();
        }

        public void Log(string content)
        {
            if (!showLog) return;
            UCLogger.Log($"[Ad Manager] {content}", Color.magenta);
        }

        #region Banner
        private int numClickBanner = 0;

        private bool isOpeningBanner;

        private bool isAreadyShowBanner;

        private bool IsClickedMaxBanner => numClickBanner < RemoteSettings.MaxClickBanner;


        public void ShowBanner()
        {
            if (IsCheating) return;
            if (AdSavedSettings.IsRemovedAds || !RemoteSettings.IsShowBanner)
            {
                Log("Can't Show Banner");
                return;
            }
            if (!IsClickedMaxBanner)
            {
                Log("Can't Request Banner");
                return;
            }
            isAreadyShowBanner = true;
            Log("ShowBanner: " + Configs.Applovin.BannerID);
            MaxSdk.ShowBanner(Configs.Applovin.BannerID);
        }
        /// <summary>
        /// Hide banner ads
        /// </summary>
        public void HideBanner()
        {
            Log("Hide banner");
            if (!IsAdsInited)
            {
                IsHideBanner = true;
                return;
            }
            MaxSdk.HideBanner(Configs.Applovin.BannerID);
        }
        private void RequestApplovinBanner()
        {
            if (AdSavedSettings.IsRemovedAds)
            {
                Log("RequestCan'tShow");
                return;
            }

            if (!IsClickedMaxBanner)
            {
                Log("RequestCan'tRequest");
                return;
            }
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(Configs.Applovin.BannerID, Configs.Applovin.BannerPosition);
            Color bannerBgColor;
            switch (Configs.Applovin.BannerColor)
            {
                case BannerColor.NoColor:
                    bannerBgColor = new Color(1f, 1f, 1f, 0f);
                    break;
                default:
                    bannerBgColor = Color.black;
                    break;
            }
            MaxSdk.SetBannerBackgroundColor(Configs.Applovin.BannerID, bannerBgColor);
        }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnApplovinAdsRevenuePaid?.Invoke(adInfo);
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            numClickBanner++;
            //LogEventManager.Ins.OnAdClickLogEvent("Banner", "Admob", numClickBanner);
            IsAdsLeaveApp = true;
            if (!IsClickedMaxBanner)
            {
                HideBanner();
            }
        }

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Log("FailLoadBanner");
        }
        #endregion

        #region Full
        private int curRetryFull;

        private float lastTimeShowFull;

        private int numClickFull = 0;

        private UnityAction OnFullClosed;

        public bool IsInterReady => MaxSdk.IsInterstitialReady(Configs.Applovin.InterID);

        private bool CanRequestFull()
        {
            return numClickFull < RemoteSettings.MaxClickFull && !AdSavedSettings.IsRemovedAds;
        }

        public bool CanShowFull()
        {
            if (IsCheating) return false;
            if (!IsAdsInited)
                return false;
            return CheckTimeBetweenShowFull();
        }

        public bool CheckTimeBetweenShowFull()
        {
            Log($"Show full available in {Mathf.Max(RemoteSettings.TimeBetweenShowFull - (Time.unscaledTime - lastTimeShowFull), 0)} ");
            return Time.unscaledTime - lastTimeShowFull >= RemoteSettings.TimeBetweenShowFull;
        }

        public void ResetTimeShowFull()
        {
            lastTimeShowFull = Time.unscaledTime;
        }

        public void ShowFull(string location, UnityAction callback = null)
        {
            OnFullClosed = callback;
            if (!IsAdsInited || IsCheating || AdSavedSettings.IsRemovedAds)
            {
                StartCoroutine(CompleteMethodfull());
                return;
            }
            OnCallShowAds?.Invoke(AdName.Inter, location, IsInterReady);
            if (CanShowFull())
            {
                if (IsInterReady)
                {
                    IsAdsLeaveApp = true;
                    ResetTimeShowFull();
                    Log("Show inter: " + Configs.Applovin.InterID);
                    MaxSdk.ShowInterstitial(Configs.Applovin.InterID, location);
                }
                else
                {
                    StartCoroutine(CompleteMethodfull());
                }
            }
            else
            {
                StartCoroutine(CompleteMethodfull());
            }
        }

        private void RequestFull()
        {
            if (!CanRequestFull())
            {
                return;
            }
            if (IsInterReady)
            {
                return;
            }
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += Interstitial_OnAdDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
            LoadInter();
        }

        private void Interstitial_OnAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            ResetTimeShowFull();
            OnInterShowed?.Invoke();
            OnShowAdsSucess?.Invoke(AdName.Inter, adInfo.Placement);
            Log("AdDisplayed " + adUnitId + " " + adInfo.NetworkName + " " + adInfo.AdFormat + " " + adInfo.AdUnitIdentifier);
        }


        private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnApplovinAdsRevenuePaid?.Invoke(adInfo);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            curRetryFull = 0;
            Log("AdLoaded " + adUnitId + " " + adInfo.NetworkName + " " + adInfo.AdFormat + " " + adInfo.AdUnitIdentifier);
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            ResetLastTimeCloseRewardInter();
            ResetTimeShowFull();
            //OnAdsDismissed?.Invoke();
            LoadInter();
            StartCoroutine(CompleteMethodfull());
            if (AdSavedSettings.NumInterShowed < 3)
            {
                AdSavedSettings.NumInterShowed++;
            }
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnClickAds?.Invoke(AdName.Inter, adInfo.Placement);
            IsAdsLeaveApp = true;
            numClickFull++;
            //LogEventManager.Ins.OnAdClickLogEvent("Interstitial", "Admob", numClickFull);
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Log("Interstitial AppLovin load failed");
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            curRetryFull++;
            float retryDelay = Mathf.Pow(2, Math.Min(6, curRetryFull));
            Invoke(nameof(WaitLoadFull), retryDelay);
        }

        private void WaitLoadFull()
        {
            LoadInter();
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogError(errorInfo.Message);
            LoadInter();
            StartCoroutine(CompleteMethodfull());
        }

        void LoadInter()
        {
            MaxSdk.LoadInterstitial(Configs.Applovin.InterID);
        }

        private IEnumerator CompleteMethodfull()
        {
            //Log("StartCompleteMethod");
            yield return null;
            if (OnFullClosed != null)
            {
                OnFullClosed();
                OnFullClosed = null;
            }
        }

        #endregion

        #region AppOpen
        public bool IsAOAShowed { get; private set; } = false;

        bool firstShowAOACalled = false;

        float lastTimeCloseRewardInter;

        bool needReopenBanner;

        public void ResetLastTimeCloseRewardInter()
        {
            lastTimeCloseRewardInter = Time.unscaledTime;
        }

        bool IsDelayInterRewardAOA()
        {
            Log("Last time: " + lastTimeCloseRewardInter);
            Log("Last time: " + Time.unscaledTime);
            if (Time.unscaledTime < lastTimeCloseRewardInter + RemoteSettings.TimeBetweenInterRewardAOA)
            {
                return true;
            }
            return false;
        }

        void WaitAOA()
        {
            timeFirstLoadAOA = Time.time;
            DOVirtual.DelayedCall(RemoteSettings.MaxTimeLoadAOAInStart, () =>
            {
                if (!firstShowAOACalled)
                {
                    Log("Cant show AOA. Time Over!");
                    IsAOAShowed = true;
                    firstShowAOACalled = true;
                }
            });
        }
        private void RequestApplovinAppOpen()
        {
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAOADisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAOADisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAOAClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAOAHiddenEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAOALoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAOALoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAOARevenuePaidEvent;
            MaxSdk.LoadAppOpenAd(Configs.Applovin.AppOpenAdsID);
        }

        public void ShowAppOpen(string location, UnityAction callback = null)
        {
            Log("Show AOA: " + location);
            if (IsCheating || AdSavedSettings.IsRemovedAds || !RemoteSettings.IsShowAOA)
            {
                Log("Cant show aoa (Cheating/RemoveAds/RemoteSettings).");
                IsAOAShowed = true;
                return;
            }
            bool isReady = MaxSdk.IsAppOpenAdReady(Configs.Applovin.AppOpenAdsID);
            OnCallShowAds?.Invoke(AdName.AOA, location, isReady);
            if (isReady)
            {
                MaxSdk.ShowAppOpenAd(Configs.Applovin.AppOpenAdsID, location);
                IsAdsLeaveApp = true;
            }
        }

        void CheckShowFirstAOA()
        {
            Log($"Check show AOA: Total time Load: {Time.time - timeFirstLoadAOA}. Max time load: {RemoteSettings.MaxTimeLoadAOAInStart}");
            if (firstShowAOACalled) return;
            firstShowAOACalled = true;
            bool isAOALoadTimeOver = Time.time > timeFirstLoadAOA + RemoteSettings.MaxTimeLoadAOAInStart;
            if (isAOALoadTimeOver)
            {
                Log($"Skip AOA");
                return;
            }
            if (RemoteSettings.SkipAOAInFirstTimeLogin && !AdSavedSettings.IsSkippedAOA)
            {
                AdSavedSettings.IsSkippedAOA = true;
                return;
            }
            ShowAppOpen("first_open");
        }

        private void OnAOADisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnShowAdsSucess?.Invoke(AdName.AOA, adInfo.Placement);
            needReopenBanner = !IsHideBanner;
            HideBanner();
        }

        private void OnAOALoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            CheckShowFirstAOA();
        }

        private void OnAOAClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnClickAds?.Invoke(AdName.AOA, adInfo.Placement);
        }

        private void OnAOALoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Log($"LoadAOAFailed: {errorInfo.Code} {errorInfo.AdLoadFailureInfo} - {errorInfo}");
            MaxSdk.LoadAppOpenAd(Configs.Applovin.AppOpenAdsID);

        }

        private void OnAOADisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Log("ShowAOAFailed");
            MaxSdk.LoadAppOpenAd(Configs.Applovin.AppOpenAdsID);
        }

        private void OnAOARevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnApplovinAdsRevenuePaid?.Invoke(adInfo);
        }

        private void OnAOAHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            MaxSdk.LoadAppOpenAd(Configs.Applovin.AppOpenAdsID);
            IsAOAShowed = true;
            if (needReopenBanner && isAreadyShowBanner)
            {
                ShowBanner();
            }
            Log("AOA Hidden");
        }

        void CheckShowRefocusAOA()
        {
            if (RemoteSettings == null) return;
            if (!RemoteSettings.IsRefocusShowAds) return;
            if ((DateTime.Now - AdSavedSettings.LastTimeRefocusShow).TotalSeconds < RemoteSettings.TimeBetweenRefocusShow)
            {
                return;
            }
            if (IsDelayInterRewardAOA())
            {
                return;
            }
            if (IsAdsLeaveApp)
            {
                IsAdsLeaveApp = false;
            }
            else
            {
                ShowAppOpen("app_unpause");
                AdSavedSettings.LastTimeRefocusShow = DateTime.Now;
            }
        }
        #endregion

        #region Reward
        private int numClickReward = 0;

        private bool triggerCompleteMethod;

        private int currentRetryRewardedVideo;

        private int maxRetryCount = 3;

        private UnityAction<bool> OnCompleteMethod;

        public bool IsRewardedReady => MaxSdk.IsRewardedAdReady(Configs.Applovin.RewardedID);

        void LoadRewarded()
        {
            MaxSdk.LoadRewardedAd(Configs.Applovin.RewardedID);
        }

        public void ShowRewardedVideo(string location, UnityAction<bool> callback)
        {
            triggerCompleteMethod = true;
            OnCompleteMethod = callback;
            OnCallShowAds?.Invoke(AdName.Reward, location, IsRewardedReady);
            if (IsCheating)
            {
                callback?.Invoke(true);
                return;
            }
            if (IsRewardedReady)
            {
                IsAdsLeaveApp = true;
                //OnCallMaxApplovinShowAds?.Invoke();
                Log("Show vd: " + Configs.Applovin.RewardedID);
                MaxSdk.ShowRewardedAd(Configs.Applovin.RewardedID, location);
                return;
            }
            else
            {
                LoadRewarded();
            }
            callback?.Invoke(false);
        }

        private void RequestRewardBasedVideo()
        {
            if (!CanRequestVideo()) return;
            if (IsRewardedReady) return;
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            LoadRewarded();
        }


        IEnumerator CompleteMethodRewardedVideo(bool val)
        {
            ResetTimeShowFull();
            yield return null;
            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(val);
                OnCompleteMethod = null;
            }
        }

        private bool CanRequestVideo()
        {
            return numClickReward < RemoteSettings.MaxClickVideo;
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnApplovinAdsRevenuePaid?.Invoke(adInfo);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnVideoShowed?.Invoke();
            OnShowAdsSucess?.Invoke(AdName.Reward, adInfo.Placement);
            lastTimeShowFull = Time.unscaledTime;
        }

        public void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //OnAdsDismissed?.Invoke();
            ResetLastTimeCloseRewardInter();
            ResetTimeShowFull();
            LoadRewarded();
            if (triggerCompleteMethod)
            {
                StartCoroutine(CompleteMethodRewardedVideo(false));
            }
        }
        public void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            triggerCompleteMethod = false;
            LoadRewarded();
            StartCoroutine(CompleteMethodRewardedVideo(true));
            if (AdSavedSettings.NumRewardShowed < 3)
            {
                AdSavedSettings.NumRewardShowed++;
            }
        }
        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            OnClickAds?.Invoke(AdName.Reward, adInfo.Placement);
            IsAdsLeaveApp = true;
            numClickReward++;
        }

        public void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Log("Rewarded ad failed to load with error code: " + errorInfo.Code);
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;
                LoadRewarded();
            }
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            LoadRewarded();
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            currentRetryRewardedVideo = 0;
        }
        #endregion

        #region RefocusAds
        private void OnApplicationFocus(bool focus)
        {
            Log("Application focuse: " + focus);
            if (focus)
            {
                CheckShowRefocusAOA();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            //if (!pause)
            //{
            //    CheckShowRefocusAOA();
            //}
        }
        #endregion

        #region RemoveAds
        public void RemoveAds()
        {
            AdSavedSettings.IsRemovedAds = true;
            HideBanner();
        }
        #endregion
    }

    public static class AdName
    {
        public const string Inter = "interstitial";

        public const string Reward = "rewarded";

        public const string AOA = "aoa";
    }

    [System.Serializable]
    public class AdsRemoteSetting
    {
        const string BannerSettingsGroup = "Banner Settings";

        const string RewardVideoSettingsGroup = "Inter & Reward Video Settings";

        const string AOASettingsGroup = "AOA Settings";

        [FoldoutGroup(BannerSettingsGroup)] public bool IsShowBanner = true;

        [FoldoutGroup(BannerSettingsGroup)] public bool IsBannerInStart = true;

        [FoldoutGroup(BannerSettingsGroup)] public int MaxClickBanner = 3;

        [FoldoutGroup(RewardVideoSettingsGroup)] public int TimeBetweenShowFull = 20;

        [FoldoutGroup(RewardVideoSettingsGroup)] public int MaxClickFull = 3;

        [FoldoutGroup(RewardVideoSettingsGroup)] public int MaxClickVideo = 3;

        [FoldoutGroup(AOASettingsGroup)] public bool IsShowAOA = true;

        [FoldoutGroup(AOASettingsGroup)] public float MaxTimeLoadAOAInStart = 10;

        [FoldoutGroup(AOASettingsGroup)] public int TimeBetweenRefocusShow = 5;

        [FoldoutGroup(AOASettingsGroup)] public int TimeBetweenInterRewardAOA = 15;

        [FoldoutGroup(AOASettingsGroup)] public bool IsRefocusShowAds = true;

        [FoldoutGroup(AOASettingsGroup)] public bool SkipAOAInFirstTimeLogin = false;
    }

    [System.Serializable]
    public enum BannerColor
    {
        Black,
        NoColor
    }

    [System.Serializable]
    public class AdManagerConfig
    {
        public string RemoteConfigKey = "test_android";

        public ApplovinConfigs Applovin;
    }

    [System.Serializable]
    public class ApplovinConfigs
    {
        [FoldoutGroup("IDs")]
        public string SdkKey = "bZncTTqqX6Kebm0dMxUgLHPAC7HVzkQo4iVz2zT32odFJeB7qTQbIf2SfSB9Q65Q3lhWc0EI9OEDhFyxrxo7f2";

        [FoldoutGroup("IDs")]
        public string AppOpenAdsID;

        [FoldoutGroup("IDs")]
        public string InterID;

        [FoldoutGroup("IDs")]
        public string BannerID;

        [FoldoutGroup("IDs")]
        public string RewardedID;

        [FoldoutGroup("Banner")]
        public MaxSdkBase.BannerPosition BannerPosition = MaxSdkBase.BannerPosition.BottomCenter;

        [FoldoutGroup("Banner")]
        public BannerColor BannerColor = BannerColor.NoColor;

    }
}

public static class AdSavedSettings
{
    public static bool IsSkippedAOA
    {
        get
        {
            return PlayerPrefsExtension.GetBool(AdPlayerPrefsKeys.IsSkippedAOA, false);
        }
        set
        {
            PlayerPrefsExtension.SetBool(AdPlayerPrefsKeys.IsSkippedAOA, value);
        }
    }

    public static bool IsRemovedAds
    {
        get
        {
            return PlayerPrefsExtension.GetBool(AdPlayerPrefsKeys.IsRemovedAds, false);
        }
        set
        {
            PlayerPrefsExtension.SetBool(AdPlayerPrefsKeys.IsRemovedAds, value);
        }
    }
    public static int NumInterShowed
    {
        get
        {
            return PlayerPrefs.GetInt(AdPlayerPrefsKeys.NumInterShowed, 0);
        }
        set
        {
            PlayerPrefs.SetInt(AdPlayerPrefsKeys.NumInterShowed, value);
        }
    }

    public static int NumRewardShowed
    {
        get
        {
            return PlayerPrefs.GetInt(AdPlayerPrefsKeys.NumRewardShowed, 0);
        }
        set
        {
            PlayerPrefs.SetInt(AdPlayerPrefsKeys.NumRewardShowed, value);
        }
    }

    public static DateTime LastTimeRefocusShow
    {
        get
        {
            return PlayerPrefsExtension.GetDateTime(AdPlayerPrefsKeys.LastTimeRefocusShow, DateTime.Now.AddDays(-1));
        }
        set
        {
            PlayerPrefsExtension.SetDateTime(AdPlayerPrefsKeys.LastTimeRefocusShow, value);
        }
    }
}

public static class AdConstants
{
    public const string AndroidFirebaseGroup = "Android Firebase Key";

    public const string IOSFirebaseGroup = "IOS Firebase Key";
}

public static class AdPlayerPrefsKeys
{
    public const string IsSkippedAOA = "IsSkippedAOA";

    public const string IsRemovedAds = "IsRemovedAds";

    public const string NumInterShowed = "NumInterShowed";

    public const string NumRewardShowed = "NumRewardShowed";

    public const string LastTimeRefocusShow = "LastTimeRefocusShow";

    public const string Settings = "Settings";
}