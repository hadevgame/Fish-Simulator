using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerOrder
{
    public int fishCount; 
    public bool isCompleted = false;
    public float payMoney;
    public bool isCard;
    public CustomerOrder()
    {
        fishCount = 0;
    }
    public void GenerateRandomOrder()
    {
        int level = LevelManager.Ins.CurrentLevel;
        int minInt = 0;
        int maxInt = 0;

        if (level <= 3) 
        {
            fishCount = Random.Range(1, 4);
            minInt = Mathf.RoundToInt(fishCount * 5f);
            maxInt = Mathf.RoundToInt(fishCount * 5f + 10f);
        }
        else if (level <= 6)
        {
            fishCount = Random.Range(2, 6);
            minInt = Mathf.RoundToInt(fishCount * 7f);
            maxInt = Mathf.RoundToInt(fishCount * 7f + 10f);
        }
        else 
        {
            fishCount = Random.Range(3, 7);
            minInt = Mathf.RoundToInt(fishCount * 10f);
            maxInt = Mathf.RoundToInt(fishCount * 10f + 10f);
        }

        int integerPart = Random.Range(minInt, maxInt); // phần nguyên
        int decimalPart = Random.Range(0, 100);         // phần thập phân: 0 → 99

        payMoney = integerPart + decimalPart / 100f;
        isCard = Random.value > 0.5f;
    }
    public bool IsOrderComplete(int collectedFish)
    {
        if (collectedFish >= fishCount)
        {
            isCompleted = true; 
            return true;
        }
        return false;
    }

    public void SetFishCount(int count) 
    {
        fishCount = count;
        int level = LevelManager.Ins.CurrentLevel;
        int minInt = 0;
        int maxInt = 0;

        if (level <= 3)
        {
            fishCount = Random.Range(1, 4);
            minInt = Mathf.RoundToInt(fishCount * 5f);
            maxInt = Mathf.RoundToInt(fishCount * 5f + 10f);
        }
        else if (level <= 6)
        {
            fishCount = Random.Range(2, 6);
            minInt = Mathf.RoundToInt(fishCount * 7f);
            maxInt = Mathf.RoundToInt(fishCount * 7f + 10f);
        }
        else
        {
            fishCount = Random.Range(3, 7);
            minInt = Mathf.RoundToInt(fishCount * 10f);
            maxInt = Mathf.RoundToInt(fishCount * 10f + 10f);
        }

        int integerPart = Random.Range(minInt, maxInt); 
        int decimalPart = Random.Range(0, 100);        

        payMoney = integerPart + decimalPart / 100f;
    }
}
