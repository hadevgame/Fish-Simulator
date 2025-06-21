using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPriceUI : MonoBehaviour
{
    [SerializeField] private PriceItemUI[] priceItems;

    public void LoadInfo(List<TankCreatureInfo> creatureInfos)
    {
        for (int i = 0; i < priceItems.Length; i++)
        {
            var creatureInfo = creatureInfos[i];
            priceItems[i].SetData(creatureInfo.fishData);
        }
    }
}
