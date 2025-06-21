using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LimitedInventory", menuName = "AutoShooter/Datas/LimitedInventory")]
public class LimitedInventory : InventoryData
{
    public int MaxQuantity;

    public int TimeRefill;

    public bool IsFull => Quantity >= MaxQuantity;

    string TIME_START_COUNT_DOWN_KEY => "TIME_START_COUNT_DOWN_KEY" + ID;

    public DateTime TimeStartCountDownHeart
    {
        get
        {
            string dateString = PlayerPrefs.GetString(TIME_START_COUNT_DOWN_KEY);
            return string.IsNullOrEmpty(dateString) ? DateTime.Now : DateTime.Parse(dateString);
        }
        set
        {
            PlayerPrefs.SetString(TIME_START_COUNT_DOWN_KEY, value.ToString());
        }
    }
    protected override void OnComsume()
    {
        base.OnComsume();
        if (Quantity >= MaxQuantity) TimeStartCountDownHeart = DateTime.Now;
    }

    public int Refill()
    {
        if (Quantity >= MaxQuantity) return 0;
        DateTime now = DateTime.Now;
        DateTime timeStartCountDownHeart = TimeStartCountDownHeart;
        int second = (int)(now - timeStartCountDownHeart).TotalSeconds;
        if (now < timeStartCountDownHeart) second = 0;
        int refillQuantity = second / TimeRefill;
        Quantity += refillQuantity;
        Quantity = Mathf.Min(Quantity, MaxQuantity);
        timeStartCountDownHeart = timeStartCountDownHeart.AddSeconds(TimeRefill * refillQuantity);
        TimeStartCountDownHeart = timeStartCountDownHeart;
        int timeCountDown = TimeRefill - second % TimeRefill;
        return timeCountDown > 0 ? timeCountDown : 0;
    }
}
