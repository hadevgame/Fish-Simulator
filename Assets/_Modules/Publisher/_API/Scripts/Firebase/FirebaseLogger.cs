using Firebase.Analytics;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using API.RemoteConfig;
using System.Text.RegularExpressions;
using System;
using UCExtension.Common;
using API.Ads;
using UCExtension;
using System.Collections.Generic;

namespace API.LogEvent
{
    public class FirebaseLogger : Singleton<FirebaseLogger>
    {
        [SerializeField] bool showLog = true;

        public readonly List<int> engagementTimes = new List<int> { 3, 5, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 90, 120, 150 };

        float timeOpen;

        protected override void SetUp()
        {
            base.SetUp();
            StartCoroutine(IEWaitCheckAndFixDependencies());
            AdManager.OnCallShowAds += OnCallShowRewardedVideoAds;
            AdManager.OnShowAdsSucess += OnShowAdsSucess;
            AdManager.OnClickAds += OnClickAds;
            AdManager.OnApplovinAdsRevenuePaid += OnApplovinAdsRevenuePaid;
            AdManager.OnInterShowed += OnInterShow;
            AdManager.OnVideoShowed += OnRewardShow;
            timeOpen = Time.unscaledTime;
        }

        private void Start()
        {
            if (GameExtensions.IsFirstSessionInDay)
            {
                FirebaseLogger.Ins.LogEvent
                    ("player_login", new Parameter("day_login", PlayerDatas.DayLogin.ToString()));
            }
        }

        private void Update()
        {
            CheckEngagement();
        }

        void CheckEngagement()
        {
            if (engagementTimes.Count == 0) return;
            float engagementTime = Time.unscaledTime - timeOpen;
            int engagementValue = engagementTimes[0];
            if (engagementTime > engagementValue * 60)
            {
                engagementTimes.PopFirstElement();
                LogEvent("engagement_each_session", new Parameter("minute", engagementValue.ToString()));
            }
        }

        IEnumerator IEWaitCheckAndFixDependencies()
        {
            yield return new WaitUntil(() => RemoteConfigManager.Ins && RemoteConfigManager.Ins.IsFetchedDatas);
            InitializeFirebase();
        }

        public void InitializeFirebase()
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            // Set the user's sign up method.
            FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
            // Set the user ID.
            FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }

        #region Ads log
        void OnApplovinAdsRevenuePaid(MaxSdkBase.AdInfo adInfo)
        {
            Parameter[] AdParameters = {
                new Parameter("ad_platform", "Applovin"),
                new Parameter("ad_source", adInfo.NetworkName),
                new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
                new Parameter("currency","USD"),
                new Parameter("value", adInfo.Revenue),
                new Parameter("placement", adInfo.Placement),
                new Parameter("country_code", MaxSdk.GetSdkConfiguration().CountryCode),
                new Parameter("ad_format", adInfo.AdFormat)
            };
            LogEvent("ad_impression", AdParameters);
        }

        void OnCallShowRewardedVideoAds(string adType, string placement, bool hasAds)
        {
            EditorLog($"Call show {adType}_ads at: {placement} (Has ads: {hasAds})");
            LogEvent($"show_{adType}_ads", new Parameter[] { new Parameter("has_ads", hasAds.ToString()), new Parameter("placement", placement) });
        }

        void OnShowAdsSucess(string adType, string placement)
        {
            EditorLog($"Show {adType}_ads sucess at: {placement}");
            LogEvent($"show_{adType}_ads_success", new Parameter[] { new Parameter("placement", placement) });
        }

        void OnClickAds(string adType, string placement)
        {
            EditorLog($"Click {adType}_ads at: {placement}");
            LogEvent($"click_{adType}_ads", new Parameter[] { new Parameter("placement", placement) });
        }

        int interShowCount = 0;

        int rewardShowCount = 0;

        void OnInterShow()
        {
            interShowCount++;
            TryLog("impdau_inter_passed", interShowCount, 3);
        }

        void OnRewardShow()
        {
            rewardShowCount++;
            TryLog("impdau_reward_passed", rewardShowCount, 3);
        }

        void TryLog(string eventName, int count, int compareCount)
        {
            if (count == compareCount)
            {
                LogEvent(eventName);
            }
        }
        #endregion
        #region Ultility functions

        public void LogEvent(string eventName)
        {
            WaitForFirebaseInit(() =>
            {
                EditorLog("Log firebase event: " + eventName);
                FirebaseAnalytics.LogEvent(eventName.ToFireBaseEvent());
            });
        }
        public void LogEvent(string eventName, Parameter[] param = null)
        {
            WaitForFirebaseInit(() =>
            {
                EditorLog("Log firebase event with params: " + eventName);
                FirebaseAnalytics.LogEvent(eventName.ToFireBaseEvent(), param);
            });
        }
        public void LogEvent(string eventName, Parameter param = null)
        {
            WaitForFirebaseInit(() =>
            {
                EditorLog($"Log firebase event with param: {eventName}");
                FirebaseAnalytics.LogEvent(eventName.ToFireBaseEvent(), param);
            });
        }
        void WaitForFirebaseInit(UnityAction finish)
        {
            StartCoroutine(IEWaitForInitFirebase(finish));
        }
        IEnumerator IEWaitForInitFirebase(UnityAction finish)
        {
            yield return new WaitUntil(() => RemoteConfigManager.Ins.IsFetchedDatas);
            finish?.Invoke();
        }

        void EditorLog(string content)
        {
            if (showLog)
            {
                UCLogger.Log($"[FirebaseLogger] {content}", Color.green);
            }
        }

        #endregion
    }
}

public static class FirebaseAnaliticsExtension
{
    public static string ToFireBaseEvent(this string eventName)
    {
        var result = Regex.Replace(eventName, @"[^0-9a-zA-Z]+", "_");
        return result.ToLower();
    }
}