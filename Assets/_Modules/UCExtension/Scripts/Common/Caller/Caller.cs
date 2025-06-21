using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    [Serializable]
    public class Caller
    {
        protected float nextTime;

        public bool CanCall => Time.time > nextTime;

        public bool Call()
        {
            if (CanCall)
            {
                ResetNextTime();
                return true;
            }
            return false;
        }
        public virtual void ResetNextTime()
        {
        }
        public void SetNextTime(float nextTime)
        {
            this.nextTime = nextTime;
        }
        public float GetTimeLeft()
        {
            return nextTime - Time.time;
        }
    }
}
