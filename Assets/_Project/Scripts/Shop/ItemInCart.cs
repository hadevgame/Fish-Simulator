using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class ItemInCart : MonoBehaviour
{
    [SerializeField] private ShopController shopController;
    public TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    public Text countText;
    public string idText;
    private int curCount = 0;
    private float curPrice = 0;
    private int maxCount = 10;
    private float priceOne = 0;
    public GameObject prefab;
    private void SetDefault()
    {
        idText = null;
        curCount = 0;
        curPrice = 0;
        maxCount = 10;
        priceOne = 0;
    }

    public void DisplayInfor(string id ,string name, float priceCount, int count, float price , GameObject obj) 
    {
        idText = id;
        nameText.text = name;
        priceText.text = "$" + priceCount.ToString();
        countText.text = count.ToString();
        curCount = count;
        curPrice = priceCount;
        priceOne = price;
        prefab = obj;
    }

    public void SetCount(int count, float priceCount) 
    {
        if(curCount < maxCount) 
        {
            int actualIncrease = 0;
            curCount += count;
            curPrice += priceCount;
            if (curCount > maxCount) 
            {
                actualIncrease = maxCount - (curCount - count);
                curCount = maxCount;
                curPrice = maxCount * priceOne;
                countText.text = curCount.ToString();
                priceText.text = "$" + curPrice.ToString();
                shopController.IncreaseTotal(actualIncrease * priceOne);
            }
            else 
            {
                countText.text = curCount.ToString();
                priceText.text = "$" + curPrice.ToString();
                shopController.IncreaseTotal(priceCount);
            }
        }
    }

    void UpdatePrice()
    {
        curPrice = curCount * priceOne;
        priceText.text = "$" + curPrice.ToString();
    }
    public void IncreaseAmount()
    {
        if (curCount < 10)
        {
            Vibrator.SoftVibrate();
            curCount++;
            countText.text = curCount.ToString();
            shopController.IncreaseTotal(priceOne);
            UpdatePrice();
        }
    }
    public void DecreaseAmount()
    {
        curCount--;
        if (curCount > 0)
        {
            Vibrator.SoftVibrate();
            countText.text = curCount.ToString();
            shopController.DecreaseTotal(priceOne);
            UpdatePrice();
        }
        if(curCount < 1) 
        {
            shopController.DecreaseTotal(priceOne);
            this.gameObject.SetActive(false);
            SetDefault();
        }
    }

    public void ResetInfor() 
    {
        SetDefault();
    }
}
