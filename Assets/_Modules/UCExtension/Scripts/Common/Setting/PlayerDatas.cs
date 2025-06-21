using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UnityEngine;

namespace UCExtension
{
    public static class PlayerDatas

    {
        public static Action<bool> OnChangeVibrateSettingCallback;

        public static bool CanVibrate
        {
            get
            {
                return PlayerPrefsExtension.GetBool(PlayerPrefsKeys.CAN_VIBRATE, true);
            }
            set
            {
                PlayerPrefsExtension.SetBool(PlayerPrefsKeys.CAN_VIBRATE, value);
                OnChangeVibrateSettingCallback?.Invoke(CanVibrate);
            }
        }

        public static bool IsRatedApp
        {
            get
            {
                return PlayerPrefsExtension.GetBool(PlayerPrefsKeys.IS_RATED_APP, false);
            }
            set
            {
                PlayerPrefsExtension.SetBool(PlayerPrefsKeys.IS_RATED_APP, value);
            }
        }

        public static int RateCount
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.RATE_COUNT, 0);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.RATE_COUNT, value);
            }
        }

        public static DateTime LastTimeLogin
        {
            get
            {
                return PlayerPrefsExtension.GetDateTime(PlayerPrefsKeys.LAST_TIME_LOGIN, new DateTime(1, 1, 1));
            }
            set
            {
                PlayerPrefsExtension.SetDateTime(PlayerPrefsKeys.LAST_TIME_LOGIN, value);
            }
        }

        public static int DayLogin
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.DAY_LOGIN, 0);
            }
            set
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.DAY_LOGIN, value);
            }
        }
    }
}
