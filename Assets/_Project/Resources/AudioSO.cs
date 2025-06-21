using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AudioSO", menuName = "AudioSO")]
public class AudioSO : ScriptableObject
{
    public List<AudioData> ClipList;
    private Dictionary<string, AudioClip> dictAudio = new Dictionary<string, AudioClip>();

    public AudioClip GetAudioClip(string audioName) 
    {
        if (!dictAudio.ContainsKey(audioName)) 
        {
            foreach (var clip in ClipList) 
            {
                if(audioName == clip.Name) 
                {
                    dictAudio.Add(audioName,clip.AudioClip);
                    break;
                }
            }
        }
        return dictAudio[audioName];
    }
}

[Serializable]
public class AudioData
{
    public string Name;
    public AudioClip AudioClip;
}
