using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UCExtension
{
    [Serializable]
    public class DelayCaller : Caller
    {
        [SerializeField] float timeDelay;

        [SerializeField] bool fixedTime;

        float CurrentTime => fixedTime ? Time.fixedTime : Time.time;

        public void SetDelayTime(float time)
        {
            timeDelay = time;
        }

        public void SetTimeDelay(float time)
        {
            timeDelay = time;
        }

        public override void ResetNextTime()
        {
            nextTime = CurrentTime + timeDelay;
        }

        public DelayCaller(float timeDelay)
        {
            this.timeDelay = timeDelay;
        }
    }
}
