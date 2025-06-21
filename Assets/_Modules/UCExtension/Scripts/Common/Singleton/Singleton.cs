using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        
        public static T Ins { get => instance; }

        protected virtual void Awake()
        {
            instance = (T)this;
            SetUp();
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
        protected virtual void SetUp()
        {

        }
    }
}
