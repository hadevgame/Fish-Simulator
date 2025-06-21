using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.GUI
{
    public abstract class ItemView<T> : MonoBehaviour
    {
        public T HoldingData { get; private set; }

        public int Index { get; private set; }

        public virtual void SetData(T data, int index)
        {
            HoldingData = data;
            Index = index;
            ResetView();
        }
        public virtual void ResetView()
        {

        }
    }
}