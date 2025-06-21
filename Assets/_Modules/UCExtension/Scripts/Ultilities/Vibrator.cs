using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    public static class Vibrator
    {
        public static void Vibrate(HapticTypes type, MonoBehaviour support)
        {
            MMVibrationManager.Haptic(type, false, true, support);
        }

        public static void SoftVibrate()
        {
            if (PlayerDatas.CanVibrate)
            {
                MMVibrationManager.Haptic(HapticTypes.SoftImpact, false, true, UCEManager.Ins);
            }
        }

        public static void MediumVibrate()
        {
            if (PlayerDatas.CanVibrate)
            {
                MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, UCEManager.Ins);
            }
        }
    }
}