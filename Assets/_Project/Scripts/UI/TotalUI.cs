using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TotalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalText;
    [SerializeField] private TextMeshProUGUI fishText;

    private float totalPrice = 0f;
    private int fishCount = 0;

    public static Action<Fish> OnUpdateScene = null;

    private void OnEnable()
    {
        OnUpdateScene += UpdateFish;
    }
    private void OnDisable()
    {
        ResetScene();
        OnUpdateScene -= UpdateFish;
    }
    public float GetData()
    {
        return totalPrice;
    }

    public void UpdateFish(Fish fish)
    {
        if (fish != null && fish.data != null)
        {
            totalPrice += fish.data.basePrice;
            fishCount++;

            UpdateTotal();
        }
    }

    public void ResetScene()
    {
        totalPrice = 0f;
        fishCount = 0;
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        totalText.text = $"${totalPrice:F2}";
        fishText.text = fishCount.ToString();
    }
}

