using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEngine;

namespace UCExtension.GUI
{
    public class AudioToggle : SettingToggle
    {
        [SerializeField] AudioMixerGroupType mixerType;

        protected override void OnToggle(bool value)
        {
            UCAudioSettings.SetOn(mixerType, value);
        }

        protected override bool StartState => UCAudioSettings.IsOn(mixerType);
    }
}