using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotInQueue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timeRemaining;
    private bool isRunning = false;
    
    public void SetItemInfo(string itemName, float waitTime)
    {
        nameText.text = itemName;
        UpdateTimeText();
        timeRemaining = waitTime;
        isRunning = false; 
    }

    public void StartCountdown()
    {
        isRunning = true; 
    }
    public bool IsRunning()
    {
        return isRunning;
    }

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
        }

        UpdateTimeText();
    }
    public string GetSlotName() 
    {
        return nameText.text;
    }

    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}
