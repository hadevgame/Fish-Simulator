using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    [DisallowMultipleComponent]
    public class RecyclableObject : MonoBehaviour
    {
        [SerializeField] bool isAutoRecycle;

        [ShowIfGroup("isAutoRecycle")]
        [SerializeField] bool unscaledTime;

        [ShowIfGroup("isAutoRecycle")]
        [SerializeField] float timeAutoRecycle;

        string poolName;

        public bool IsInPool { get; set; }

        public string PoolName
        {
            get
            {
                if (string.IsNullOrEmpty(poolName)) return name;
                return poolName;
            }
        }

        public virtual void OnSpawn()
        {
            if (isAutoRecycle)
            {
                StartCoroutine(IERecycle());
            }
        }

        public void SetPoolName(string originalName)
        {
            poolName = originalName;
        }
        IEnumerator IERecycle()
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(timeAutoRecycle);
            }
            else
            {
                yield return new WaitForSeconds(timeAutoRecycle);
            }
            Recycle();
        }

        public void Recycle()
        {
            if (IsInPool) return;
            PoolObjects.Ins.Destroy(this);
            OnRecycled();
        }

        public virtual void OnRecycled()
        {
        }

    }
}