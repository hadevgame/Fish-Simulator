using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UCExtension;
using UCExtension.GUI;
using UnityEditor;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.UI;

namespace UCExtension
{
    public static class GameExtensions
    {
        public static string ProductNameSimplified
        {
            get
            {
                return Application.productName.ToLower().Replace(" ", "_");
            }
        }

        public static bool IsFirstSessionInDay;

        public static float GetTotalSquareMagnitude(this EdgeCollider2D edgeCollider)
        {
            float total = 0;
            for (int i = 0; i < edgeCollider.points.Length - 1; i++)
            {
                float value = (edgeCollider.points[i] - edgeCollider.points[i + 1]).sqrMagnitude;
                total += value;
            }
            return total;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void OnGameStart()
        {
            UCLogger.Log("UCExtension init game", Color.green);
            SetMobileFPS();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            CheckFirstSession();
        }

        static void CheckFirstSession()
        {
            var now = DateTime.Now;
            IsFirstSessionInDay = now.IsDayAfter(PlayerDatas.LastTimeLogin);
            if (IsFirstSessionInDay)
            {
                PlayerDatas.DayLogin++;
            }
            PlayerDatas.LastTimeLogin = now;
        }

        public static void RateApp(Action finish = null)
        {
            PlayerDatas.RateCount++;
            if (!CanRate() || PlayerDatas.IsRatedApp)
            {
                finish?.Invoke();
                return;
            }
#if UNITY_EDITOR||UNITY_ANDROID
            GUIController.Ins.Open<RateGUI>().OnClose(finish);
#elif UNITY_IOS
            PlayerDatas.IsRatedApp = true;
            Device.RequestStoreReview();
            finish?.Invoke();
#endif
        }

        static bool CanRate()
        {
            return PlayerDatas.RateCount == 1 || PlayerDatas.RateCount == 5;
        }

        public static void OpenGoogleStore()
        {
            PlayerDatas.IsRatedApp = true;
            Application.OpenURL(@$"https://play.google.com/store/apps/details?id={Application.identifier}");
        }

        public static void SetMobileFPS()
        {
#if !UNITY_EDITOR
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
#endif
        }
    }
}