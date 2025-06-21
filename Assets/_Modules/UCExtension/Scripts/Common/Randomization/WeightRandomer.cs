using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    public class WeightRandomer<T>
    {
        List<WeightData<T>> items = new List<WeightData<T>>();

        public void AddItem(T data, int weight)
        {
            var newItem = new WeightData<T>();
            newItem.Data = data;
            newItem.Weight = weight;
            items.Add(newItem);
        }

        public T GetRandomItem()
        {
            List<WeightData<T>> temp = new List<WeightData<T>>();
            int totalWeight = 0;
            foreach (var item in items)
            {
                if (item.Weight > 0)
                {
                    temp.Add(item);
                    totalWeight += item.Weight;
                }
            }
            int randWeight = UnityEngine.Random.Range(0, totalWeight);
            int weight = 0;
            foreach (var item in temp)
            {
                weight += item.Weight;
                if (randWeight <= weight)
                {
                    return item.Data;
                }
            }
            if (temp.Count > 0)
            {
                return temp[0].Data;
            }
            return items.RandomElement().Data;
        }
    }

    public class WeightData<T>
    {
        public T Data;

        public int Weight;
    }
}