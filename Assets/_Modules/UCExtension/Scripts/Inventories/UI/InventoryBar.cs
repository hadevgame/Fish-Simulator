using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UCExtension.Audio;
using UCExtension.GUI;
using UCExtension.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryBar : MonoBehaviour
{
    [SerializeField] InventoryData data;

    [SerializeField] Text quantityText;

    [SerializeField] GetPointBar getPointBar;

    [SerializeField] AudioClip getSound;

    private void Start()
    {
        UpdateQuantities();
        InventoryData.OnChangeQuantity += OnItemQuantitiesChange;
    }

    void OnItemQuantitiesChange(InventoryData inventory,bool isAdd)
    {
        if (inventory.HasSameID(inventory))
        {
            UpdateQuantities();
        }
    }

    public void UpdateQuantities()
    {
        quantityText.text = $"{data.Quantity}";
    }


    public void Get(Vector3 position, int quantities, UnityAction complete = null)
    {
        int step = 5;
        int coinPerStep = quantities / step;
        int totalCoin = quantities;
        getPointBar.Get(step, position, () =>
        {
            totalCoin -= coinPerStep;
            if (totalCoin < coinPerStep)
            {
                quantities += totalCoin;
            }
            else
            {
                quantities += coinPerStep;
            }
            //quantityText.text = $"{quantity}";
        }, () =>
        {
            AudioManager.Ins.PlaySFX(getSound);
            complete?.Invoke();
            UpdateQuantities();
        });
    }
}
