using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCExtension;
using System;
public class PriceManager : Singleton<PriceManager>
{
    [System.Serializable]
    public class PriceEntry
    {
        public FishData fishData;
        public float price;
    }

    [SerializeField] private List<PriceEntry> priceList;

    private Dictionary<int, float> priceDict = new Dictionary<int, float>();
    public static Action<FishData> OnPriceUpdated = null;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        priceDict.Clear();
        foreach (var entry in priceList)
        {
            if (entry.fishData != null)
                priceDict[entry.fishData.fishID] = entry.price;
        }
    }

    public float GetPrice(FishData data)
    {
        if (data == null) return 0f;
        if (priceDict.TryGetValue(data.fishID, out float value))
            return value;

        return 0f;
    }
    public void SetPrice(FishData data, float newPrice)
    {
        if (data == null) return;

        // Cập nhật giá trong priceDict
        if (priceDict.ContainsKey(data.fishID))
        {
            priceDict[data.fishID] = newPrice;
            Debug.Log("set");
        }
        else
        {
            priceDict.Add(data.fishID, newPrice);
        }
        OnPriceUpdated?.Invoke(data);
    }

}
