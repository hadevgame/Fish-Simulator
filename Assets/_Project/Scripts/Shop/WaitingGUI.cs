using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UCExtension.GUI;

public class WaitingGUI : BaseGUI
{
    [SerializeField] private GameObject panelQueue;
    [SerializeField] private Button panelWait;
    [SerializeField] private List<GameObject> slots = new List<GameObject>();

    [SerializeField] private Text waitingNameText;
    [SerializeField] private Text waitingTimeText;
    [SerializeField] private Button btnExit;

    public static Action<string> OnRemoveSlot;
    
    private void Start()
    {
        OnRemoveSlot = RemoveSlot;
        panelWait.onClick.AddListener(OpenWaitingQueue);
        btnExit.onClick.AddListener(ExitWaitingQueue);
    }
    private void OnDisable()
    {
        OnRemoveSlot = null;
    }

    public void SetWaitingPanelActive(bool active)
    {
        panelWait.gameObject.SetActive(active);
    }

    public void UpdateWaitingItemName(string itemName)
    {
        waitingNameText.text = itemName;
    }

    public void UpdateWaitingTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        waitingTimeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void OpenWaitingQueue()
    {
        Vibrator.SoftVibrate();
        panelQueue.SetActive(true);
        UpdateQueue();
    }

    public void ExitWaitingQueue()
    {
        Vibrator.SoftVibrate();
        panelQueue.SetActive(false);
    }

    private void UpdateQueue()
    {
        var queue = ShopController.Ins.GetSpawnQueue();
        int queueIndex = 0;
        HideAllSlots();
        foreach (var item in queue)
        {
            if (queueIndex >= slots.Count) break;

            for (int i = 0; i < slots.Count; i++)
            {
                GameObject obj = slots[i];

                if (obj != null && !obj.activeSelf)
                {
                    obj.SetActive(true);
                    SlotInQueue slotQueue = obj.GetComponent<SlotInQueue>();
                    slotQueue.SetItemInfo(item.itemName, ShopController.Ins.GetSpawnDelay());
                    if (queueIndex == 0)
                    {
                        slotQueue.SetItemInfo(item.itemName, ShopController.Ins.GetCurrentDelay());
                        slotQueue.StartCountdown();
                    }

                    queueIndex++;
                    break;
                }
            }
        }
    }

    private void HideAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.SetActive(false);
            }
        }
    }

    private void RemoveSlot(string name)
    {
        foreach (GameObject slot in slots)
        {
            GameObject firstslot = slots[0];
            SlotInQueue slotQueue = firstslot.GetComponent<SlotInQueue>();

            if (slotQueue != null && slotQueue.GetSlotName() == name && slot.activeSelf)
            {
                slot.SetActive(false);
                slot.transform.SetAsLastSibling();
                slots.RemoveAt(0);
                slots.Add(firstslot);
                UpdateQueue();
                break;
            }
        }
    }
}

