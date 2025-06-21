using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    [Serializable]
    public class RandomDelayCaller : Caller
    {
        [SerializeField] float timeDelayMin;

        [SerializeField] float timeDelayMax;

        [SerializeField] bool fixedTime;

        public float TimeDelayMin => timeDelayMin;

        public float TimeDelayMax => timeDelayMax;

        float CurrentTime => fixedTime ? Time.fixedTime : Time.time;

        public RandomDelayCaller(float timeDelayMin, float timeDelayMax)
        {
            this.timeDelayMin = timeDelayMin;
            this.timeDelayMax = timeDelayMax;
        }

        public RandomDelayCaller(Pair<float> config)
        {
            timeDelayMin = config.First;
            timeDelayMax = config.Second;
        }
        public override void ResetNextTime()
        {
            nextTime = CurrentTime + UnityEngine.Random.Range(timeDelayMin, timeDelayMax);
        }

        public void SetTime(float timeMin, float timeMax)
        {
            timeDelayMin = timeMin;
            timeDelayMax = timeMax;
        }

        public void SetConfig(Pair<float> config)
        {
            SetTime(config.First, config.Second);
        }
    }

    [Serializable]
    public class Pair<T>
    {
        public T First;

        public T Second;
    }
}