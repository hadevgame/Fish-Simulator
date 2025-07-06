using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotDisplay : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text priceAddToCartText;
    [SerializeField] private Text quantityText;
    [SerializeField] private Image imageItem;
    [SerializeField] private Button increaseButton;
    [SerializeField] private Button decreaseButton;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private Text unlockText;

    [SerializeField] private ShopController shopController;
    private int curAmount = 1;
    private float curPrice;
    private string itemID;
    private GameObject itemPrefab;
    private bool lockitem;

    public static Action<int> OnUnlockItem = null;
    void Start()
    {
        OnUnlockItem += TryUnlock;

        itemID = item.id;
        nameText.text = item.name;
        imageItem.sprite = item.image;
        priceText.text = "$" + item.priceOne.ToString();
        priceAddToCartText.text = "$" + item.price.ToString();
        curPrice = float.Parse(priceAddToCartText.text.Replace("$", ""));
        itemPrefab = item.prefab;

        int lvl = LevelManager.Ins.CurrentLevel;
        if (lvl < item.unlocklvl)
        {
            LockItem();
        }
        else
        {
            UnlockItem();
        }
    }
    private void OnDisable()
    {
        OnUnlockItem -= null;
    }
    void LockItem()
    {
        lockitem = true;
        lockedPanel.SetActive(true);
        unlockText.text = $"UNLOCK AT LEVEL {item.unlocklvl}";
    }
    void UnlockItem()
    {
        lockitem= false;
        lockedPanel.SetActive(false);
    }
    public void TryUnlock(int currentLevel)
    {
        if (lockitem && currentLevel >= item.unlocklvl)
        {
            UnlockItem();
        }
    }
    void UpdatePrice() 
    {
        curPrice = curAmount * item.price;
        priceAddToCartText.text = "$" + curPrice.ToString();
    }
    public void IncreaseAmount()
    {
        if (curAmount < 10) 
        {
            Vibrator.SoftVibrate();
            curAmount++;
            quantityText.text = curAmount.ToString();
            UpdatePrice();
        }
    }
    public void DecreaseAmount() 
    {
        if (curAmount > 1) 
        {
            Vibrator.SoftVibrate();
            curAmount--;
            quantityText.text = curAmount.ToString();
            UpdatePrice();
        }
    }

    public void DisplayItemInCart() 
    {
        if (lockitem) return;

        for (int i = 0; i < shopController.itemCartList.Count; i++)
        {
            GameObject cartItem = shopController.itemCartList[i];
            ItemInCart itemInCart = cartItem.GetComponent<ItemInCart>();

            if (itemInCart.idText == this.itemID) 
            {
                itemInCart.SetCount(int.Parse(quantityText.text), float.Parse(priceAddToCartText.text.Replace("$", "")));
                return;
            }

            if(itemInCart.idText != this.itemID) 
            {
                if (item != null && !cartItem.activeSelf) 
                {
                    Vibrator.SoftVibrate();
                    cartItem.SetActive(true); 
                    itemInCart.DisplayInfor(itemID, this.nameText.text, float.Parse(priceAddToCartText.text.Replace("$", "")), int.Parse(quantityText.text), float.Parse(item.price.ToString()), itemPrefab);
                    shopController.IncreaseTotal(curPrice);
                    return; 
                }
            }
            
        }
    }
    
}
