using API.Ads;
using AppsFlyerSDK;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

namespace API.LogEvent
{
    public class AppsFlyerLogger : Singleton<AppsFlyerLogger>
    {
        [SerializeField] AppsFlyerObjectScript appsFlyerObject;

        private void Start()
        {
            AdManager.OnApplovinAdsRevenuePaid += SendRevenueEvent;
            AdManager.OnInitilized += InitializeAppsFlyer;
        }

        void InitializeAppsFlyer()
        {
            appsFlyerObject.InitSDK();
        }

        public void SendRevenueEvent(MaxSdkBase.AdInfo data)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(AFAdRevenueEvent.COUNTRY, MaxSdk.GetSdkConfiguration().CountryCode);
            dic.Add(AFAdRevenueEvent.AD_UNIT, data.AdUnitIdentifier);
            dic.Add(AFAdRevenueEvent.AD_TYPE, data.AdFormat);
            dic.Add(AFAdRevenueEvent.PLACEMENT, data.Placement);
            AppsFlyerAdRevenue.logAdRevenue(data.NetworkName,
                AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                data.Revenue,
                "USD",
                dic);
        }
    }
}