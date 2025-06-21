using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Audio
{
    public static class UCAudioSettings

    {
        static List<float> defaultVolums = new List<float>();

        public static Action<AudioMixerGroupType> OnChangeAudioState;

        public static Action<AudioMixerGroupType> OnChangeAudioVolumn;

        public static bool IsOn(AudioMixerGroupType type)
        {
            return PlayerPrefsExtension.GetBool(PlayerPrefsKeys.IS_AUDIO_ON + type, true);
        }

        public static void SetOn(AudioMixerGroupType type, bool value)
        {
            PlayerPrefsExtension.SetBool(PlayerPrefsKeys.IS_AUDIO_ON + type, value);
            OnChangeAudioState?.Invoke(type);
        }

        public static void Toggle(AudioMixerGroupType type)
        {
            bool newValue = !IsOn(type);
            SetOn(type, newValue);
        }

        public static void SetVolumn(AudioMixerGroupType type, float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.AUDIO_VOLUMN + type, value);
            OnChangeAudioVolumn?.Invoke(type);
        }

        public static float GetVolumn(AudioMixerGroupType type)
        {
            if (!IsOn(type)) return 0;
            InitDefault();
            return PlayerPrefs.GetFloat(PlayerPrefsKeys.AUDIO_VOLUMN + type, GetDefault(type));
        }

        public static void SetDefault(AudioMixerGroupType type, float value)
        {
            InitDefault();
            defaultVolums[(int)type] = value;
        }

        static float GetDefault(AudioMixerGroupType type)
        {
            return defaultVolums[(int)type];
        }

        static void InitDefault()
        {
            if (defaultVolums.Count > 0) return;
            defaultVolums.Clear();
            for (int i = 0; i < Enum.GetNames(typeof(AudioMixerGroupType)).Length; i++)
            {
                defaultVolums.Add(1);
            }
        }
    }
}