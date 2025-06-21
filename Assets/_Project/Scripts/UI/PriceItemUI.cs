using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriceItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text priceText;
    private FishData itemdata;
    public void SetData(FishData data)
    {
        itemdata = data;
        iconImage.sprite = data.icon;
        float price = PriceManager.Ins.GetPrice(data);
        priceText.text = price < 0 ? "--" : "$" + price.ToString("F2");
    }

    public FishData GetData() 
    {
        return itemdata;
    }
    private void Start()
    {
        PriceManager.OnPriceUpdated += UpdatePrice;
    }

    private void OnDisable()
    {
        PriceManager.OnPriceUpdated -= UpdatePrice;
    }

    private void UpdatePrice(FishData fishData)
    {
        if (itemdata.fishID == fishData.fishID)
        {
            float price = PriceManager.Ins.GetPrice(itemdata);
            priceText.text = price < 0 ? "--" : "$" + price.ToString("F2");
        }
    }
}
