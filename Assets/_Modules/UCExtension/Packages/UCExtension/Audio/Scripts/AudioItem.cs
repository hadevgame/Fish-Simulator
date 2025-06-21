using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace UCExtension.Audio
{
    public class AudioItem : MonoBehaviour
    {
        private AudioSource source;
        private AudioSource Source
        {
            get
            {
                if (source == null)
                {
                    source = GetComponent<AudioSource>();
                }
                return source;
            }
        }

        private UnityAction finished;

        IEnumerator ieSmooth;

        public bool IsPlaying
        {
            get
            {
                if (source != null)
                {
                    return source.isPlaying;
                }
                return false;
            }
        }

        public void PlaySoundOnce(AudioClip audioClip)
        {
            if (!audioClip) return;
            gameObject.SetActive(true);
            StartCoroutine(PlaySoundCoroutine(audioClip));
        }

        IEnumerator PlaySoundCoroutine(AudioClip audioClip)
        {
            Source.volume = 1;
            Source.loop = false;
            Source.clip = audioClip;
            Source.time = 0;
            Source.Play();
            float audioLength = audioClip.length;
            yield return new WaitForSecondsRealtime(audioLength + 0.1f);
            finished?.Invoke();
            finished = null;
        }

        public void PlaySoundLoop(AudioClip audioClip)
        {
            gameObject.SetActive(true);
            Source.clip = audioClip;
            Source.loop = true;
            Source.volume = 1;
            Source.Play();
        }
        public void Play()
        {
            gameObject.SetActive(true);
            Source.Play();
        }

        public void StopSound()
        {
            Source.Stop();
        }

        public void PauseImmedietely()
        {
            StopIESmooth();
            source.volume = 0;
            Source.Pause();
        }
        public void UnpauseImmedietely()
        {
            StopIESmooth();
            source.volume = 1;
            Source.UnPause();
        }

        public void UnpauseSmoothy(float speed = 10f)
        {
            SmoothVolumn(1, speed, () =>
            {
                Source.UnPause();
            });
        }

        public void PauseSmoothy(float speed = 10f)
        {
            SmoothVolumn(0, speed, () =>
            {
                Source.Pause();
            });
        }

        void SmoothVolumn(float value, float speed = 10f, UnityAction finish = null)
        {
            StopIESmooth();
            ieSmooth = IESmoothVolumn(value, speed, finish);
            StartCoroutine(ieSmooth);
        }

        void StopIESmooth()
        {
            if (ieSmooth != null)
            {
                StopCoroutine(ieSmooth);
            }
        }

        IEnumerator IESmoothVolumn(float value, float speed = 10f, UnityAction finished = null)
        {
            while (Mathf.Abs(source.volume - value) > 0.01f)
            {
                source.volume = Mathf.Lerp(source.volume, value, speed * Time.unscaledDeltaTime);
                yield return null;
            }
            finished?.Invoke();

        }

        public void OnCompleted(UnityAction callback)
        {
            finished = callback;
        }

        public void Recyle()
        {
            AudioManager.Ins.RecycleAudioItem(this);
        }
        public void SetMixer(AudioMixerGroup mixerGroup)
        {
            Source.outputAudioMixerGroup = mixerGroup;
        }

        public void Set3DSound(AudioRolloffMode mode, float minDistance, float maxDistance)
        {
            Source.spatialBlend = 1;
            Source.rolloffMode = mode;
            Source.minDistance = minDistance;
            Source.maxDistance = maxDistance;
        }

        public void SetNormalSound()
        {
            Source.spatialBlend = 0;
        }
    }

}


public static class AudioExtension
{
    public static void Recyle(this List<AudioItem> items)
    {
        foreach (var item in items)
        {
            item.Recyle();
        }
        items.Clear();
    }
}