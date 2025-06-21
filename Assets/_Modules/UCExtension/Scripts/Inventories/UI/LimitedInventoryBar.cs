using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UCExtension.UI;
using UnityEngine;
using UnityEngine.UI;

public class LimitedInventoryBar : MonoBehaviour
{
    [SerializeField] LimitedInventory inventoryData;

    [SerializeField] Button getButton;

    [SerializeField] Transform spawnPos;

    [SerializeField] Text quantityText;

    [SerializeField] GameObject plusIcon;

    [SerializeField] GetPointBar getPointBar;

    public Timer HeartTimer;

    // Start is called before the first frame update
    void Start()
    {
        CalculateHeart();
        InventoryData.OnChangeQuantity += OnChangeQuantity;
    }

    void OnChangeQuantity(InventoryData data,bool isAdd)
    {
        if (data.HasSameID(inventoryData))
        {
            Debug.Log("Calculate heart");
            CalculateHeart();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            CalculateHeart();
        }
    }

    public void OnAddHeart()
    {
        CalculateHeart();
    }

    public void CalculateHeart()
    {
        int second = inventoryData.Refill();     
        plusIcon.SetActive(!inventoryData.IsFull);
        HeartTimer.gameObject.SetActive(!inventoryData.IsFull);
        getButton.interactable = !inventoryData.IsFull;
        quantityText.gameObject.SetActive(true);
        quantityText.text = $"{inventoryData.Quantity}/{inventoryData.MaxQuantity}";
        if (!inventoryData.IsFull)
        {
            HeartTimer.CountDown(second, () =>
            {
                CalculateHeart();
            });
        }
    }
}
