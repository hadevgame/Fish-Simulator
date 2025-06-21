using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

namespace UCExtension
{
    public class UniquePopList<T>
    {
        List<T> source = new List<T>();

        List<T> clone = new List<T>();

        public UniquePopList(List<T> source)
        {
            this.source = source;
            clone.Clear();
        }

        public void SetSource(List<T> source)
        {
            this.source = source;
            clone.Clear();
        }

        public T Pop()
        {
            if (clone.Count == 0)
            {
                clone = source.Clone();
            }
            return clone.PopRandomElement();
        }
    }
}