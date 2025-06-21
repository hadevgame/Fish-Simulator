using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    public class AutoInstantiateSingletonUC<T> : MonoBehaviour where T : AutoInstantiateSingletonUC<T>
    {
        private static T instance;

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
        public static T Ins
        {
            get
            {
                if (instance == null)
                {
                    GameObject gObject = new GameObject(string.Format("[UC Extension] {0}", typeof(T).Name));
                    instance = gObject.AddComponent<T>();
                    DontDestroyOnLoad(gObject);
                }
                return instance;
            }
        }
    }
}