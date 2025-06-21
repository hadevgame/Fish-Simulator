using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UCExtension.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [FoldoutGroup("Neccessary")]
        [SerializeField] AudioMixer mainMixer;

        [FoldoutGroup("Neccessary")]
        [SerializeField] AudioItem audioItemPrefab;


        [SerializeField]
        [BoxGroup("Settings")]
        [Range(0f, 1f)] float defaultBGMusicVolume = 1f;

        [SerializeField]
        [BoxGroup("Settings")]
        [Range(0f, 1f)] float defaultSfxVolume = 1f;

        [BoxGroup("Settings")]
        [SerializeField] AudioClip buttonClickAudio;

        int playingBGMusicID = 0;

        AudioItem bgAudioItem;

        Queue<AudioItem> UnuseAudioItems = new Queue<AudioItem>();

        Dictionary<int, SoundPlayer> soundPlayers = new Dictionary<int, SoundPlayer>();

        private void Start()
        {
            UCAudioSettings.OnChangeAudioState += OnChangeAudioState;
            UCAudioSettings.OnChangeAudioVolumn += OnChangeAudioVolumn;
            UCAudioSettings.SetDefault(AudioMixerGroupType.BgMusic, defaultBGMusicVolume);
            UCAudioSettings.SetDefault(AudioMixerGroupType.SFX, defaultSfxVolume);
            ResetMixerVolumn(AudioMixerGroupType.Master);
            ResetMixerVolumn(AudioMixerGroupType.BgMusic);
            ResetMixerVolumn(AudioMixerGroupType.SFX);
        }

        #region Volumn Settings
        void ResetMixerVolumn(AudioMixerGroupType type)
        {
            SetMixerVolume(type, UCAudioSettings.GetVolumn(type));
        }

        void OnChangeAudioVolumn(AudioMixerGroupType type)
        {
            ResetMixerVolumn(type);
        }

        void OnChangeAudioState(AudioMixerGroupType type)
        {
            ResetMixerVolumn(type);
        }

        AudioMixerGroup GetMixerGroup(AudioMixerGroupType mixerType)
        {
            return mainMixer.FindMatchingGroups(mixerType.GetMixerGroupName())[0];
        }

        public void SetMixerVolume(AudioMixerGroupType mixerType, float value)
        {
            SetMixerVolume(mixerType.GetVolumnParamName(), value);
        }

        public void SetMixerVolume(string key, float value)
        {
            mainMixer.SetFloat(key, AudioUltilities.GetDbValue(value));
        }
        #endregion
        #region Audio play
        public void PlaySFX(List<AudioClip> clips)
        {
            foreach (var item in clips)
            {
                PlaySFX(item);
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (!clip) return;
            var audioItem = PlaySound(clip, AudioMixerGroupType.SFX);
            audioItem.OnCompleted(() =>
            {
                RecycleAudioItem(audioItem);
            });
        }

        public void Play3DSFX(AudioClip clip, Vector3 position, float maxDistance, AudioRolloffMode mode = AudioRolloffMode.Linear)
        {
            if (!clip) return;
            var audioItem = PlaySound(clip, AudioMixerGroupType.SFX);
            audioItem.transform.position = position;
            audioItem.Set3DSound(mode, 0, maxDistance);
            audioItem.OnCompleted(() =>
            {
                audioItem.SetNormalSound();
                RecycleAudioItem(audioItem);
            });
        }

        SoundPlayer GetSoundPlayer(int id)
        {
            if (soundPlayers.ContainsKey(id))
            {
                return soundPlayers[id];
            }
            else
            {
                var soundPlayer = new SoundPlayer(0.1f);
                soundPlayers[id] = soundPlayer;
                return soundPlayer;
            }
        }

        public void PlayDelaySFX(AudioClip clip, float timeDelay)
        {
            if (!clip) return;
            GetSoundPlayer(clip.GetInstanceID()).PlaySound(clip).SetTimeDelay(timeDelay);
        }

        public void PlayButtonClickSFX()
        {
            PlaySFX(buttonClickAudio);
        }

        public void ChangeBGMusic(AudioClip clip, bool overrideClip = false)
        {
            if (!clip) return;
            if (playingBGMusicID == clip.GetInstanceID() && !overrideClip) return;
            playingBGMusicID = clip.GetInstanceID();
            if (!bgAudioItem)
            {
                bgAudioItem = PlaySound(clip, AudioMixerGroupType.BgMusic, true);
            }
            else
            {
                bgAudioItem.PlaySoundLoop(clip);
            }
        }

        public AudioItem PlaySound(AudioClip clip, AudioMixerGroupType mixerType, bool isLoop = false)
        {
            if (!clip)
            {
                return null;
            }
            var soundItem = GetAudioItem(mixerType);
            if (isLoop)
            {
                soundItem.PlaySoundLoop(clip);
            }
            else
            {
                soundItem.PlaySoundOnce(clip);
            }
            return soundItem;
        }

        public void PauseBGMusic()
        {
            if (bgAudioItem)
            {
                bgAudioItem?.PauseImmedietely();
            }
        }
        public void ReplayBGMusic()
        {
            if (bgAudioItem)
            {
                bgAudioItem?.UnpauseImmedietely();
                bgAudioItem?.Play();
            }
        }
        public void UnPauseBGMusic()
        {
            if (bgAudioItem)
            {
                bgAudioItem?.UnpauseImmedietely();
            }
        }

        AudioItem GetAudioItem()
        {
            AudioItem audioItem = null;
            if (UnuseAudioItems.Count > 0)
            {
                audioItem = UnuseAudioItems.Dequeue();
            }
            else
            {
                audioItem = Instantiate(audioItemPrefab, transform);
            }
            return audioItem;
        }

        public AudioItem GetAudioItem(AudioMixerGroupType mixerType)
        {
            AudioItem audioItem = GetAudioItem();
            audioItem.SetMixer(GetMixerGroup(mixerType));
            return audioItem;
        }

        public void RecycleAudioItem(AudioItem item)
        {
            UnuseAudioItems.Enqueue(item);
            item.UnpauseImmedietely();
            item.transform.SetParent(transform);
            item.gameObject.SetActive(false);
        }
        #endregion

    }

    public enum AudioMixerGroupType
    {
        Master,
        BgMusic,
        SFX
    }

    public static class AudioGroupName
    {
        public const string MASTER = "Master";

        public const string BG_MUSIC = "BgMusic";

        public const string SFX = "Sfx";
    }

    public static class AudioVolumnParamName
    {
        public const string MASTER = "MasterVolume";

        public const string SFX = "SfxVolume";

        public const string BG_MUSIC = "BgMusicVolume";
    }

    public static class AudioUltilities
    {
        public static string GetMixerGroupName(this AudioMixerGroupType type)
        {
            switch (type)
            {
                case AudioMixerGroupType.Master:
                    return AudioGroupName.MASTER;
                case AudioMixerGroupType.BgMusic:
                    return AudioGroupName.BG_MUSIC;
                case AudioMixerGroupType.SFX:
                default:
                    return AudioGroupName.SFX;
            }
        }
        public static string GetVolumnParamName(this AudioMixerGroupType type)
        {
            switch (type)
            {
                case AudioMixerGroupType.Master:
                    return AudioVolumnParamName.MASTER;
                case AudioMixerGroupType.BgMusic:
                    return AudioVolumnParamName.BG_MUSIC;
                case AudioMixerGroupType.SFX:
                default:
                    return AudioVolumnParamName.SFX;
            }
        }


        public static float GetDbValue(float value)
        {
            return Mathf.Log10(value + 0.000001f) * 20;
        }
    }
}