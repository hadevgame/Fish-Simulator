using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEngine;

public class SoundPlayer
{
    float lastTimePlaySound = 0;

    float delayTime = 0.2f;

    public SoundPlayer(float delayTime)
    {

        this.delayTime = delayTime;
    }

    public SoundPlayer SetTimeDelay(float delayTime)
    {
        this.delayTime = delayTime;
        return this;
    }
    public SoundPlayer PlaySound(AudioClip source)
    {
        if (Time.unscaledTime > delayTime + lastTimePlaySound)
        {
            lastTimePlaySound = Time.unscaledTime;
            AudioManager.Ins.PlaySFX(source);
        }
        return this;
    }
}
