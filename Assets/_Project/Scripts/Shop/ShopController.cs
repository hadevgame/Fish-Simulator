using API.Ads;
using API.LogEvent;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Ins;
    [Header("Camera & Canvas")]
    [SerializeField] private GameObject shopCamera = null;
    [SerializeField] private GameObject mainCamera = null;

    [SerializeField] private WaitingGUI waitingGUI;

    [Header("Cart System")]
    public List<GameObject> itemCartList = new List<GameObject>();
    private float total = 0;

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnItemPositions; 
    [SerializeField] private float spawnDelay = 30f;
    
    [Header("References")]
    [SerializeField] private GameObject box;
    [SerializeField] private ShopGUI shopGUI;

    private Queue<(string itemid, GameObject obj, string itemName)> spawnQueue = new Queue<(string, GameObject, string)>();
    private bool isProcessingQueue = false;
    private float currentDelayTimer;
    private int currentSpawnIndex = 0;

    public static Func<Queue<(string itemid, GameObject obj, string itemName)>> OnGetSpawnQueue;

    private void Awake()
    {
        if (Ins == null) 
        {
            Ins = this;
        }
    }
    private void OnDisable()
    {
        Ins = null;
    }
    private void Start()
    {
        SetRef();
        OnGetSpawnQueue += GetSpawnQueue;
    }

    void SetRef() 
    {
        waitingGUI = GUIController.Ins.Open<WaitingGUI>();
        shopCamera = CameraManager.Ins.GetCam(2);
        mainCamera = CameraManager.Ins.GetCam(1);
        shopGUI = GUIController.Ins.Open<ShopGUI>();
        spawnItemPositions = GameManager.Instance.GetSpawnPoint();
    }
    public void ExitShop()
    {
        Vibrator.SoftVibrate();
        shopCamera.SetActive(false);
        mainCamera.SetActive(true);
        GUIController.Ins.Open<PlayGUI>();
        GUIController.Ins.Open<ShopGUI>().TogglePanel(false);

    }

    public void BuyItem()
    {
        Vibrator.SoftVibrate();
        if (!MoneyManager.instance.CanAfford(total))
        {
            shopGUI.ShowMessage("Not enough money!",true);
            return;
        }
        if (total == 0) return;

        MoneyManager.instance.SpendMoney(total);
        AddItemsToQueue();

        if (!isProcessingQueue && spawnQueue.Count > 0)
            StartCoroutine(ProcessSpawnQueue());
        shopGUI.ShowMessage("Purchase successful!", false);
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("CASHTRAY"));
        shopGUI.UpdateTotalPrice(0);
        total = 0;
        
    }

    private void AddItemsToQueue()
    {
        foreach (GameObject cartItem in itemCartList)
        {
            if (!cartItem.activeSelf) continue;

            ItemInCart itemInCart = cartItem.GetComponent<ItemInCart>();
            if (itemInCart == null) continue;

            for (int i = 0; i < int.Parse(itemInCart.countText.text); i++)
            {
                spawnQueue.Enqueue((
                    itemInCart.idText,
                    itemInCart.prefab,
                    itemInCart.nameText.text
                ));
            }

            shopGUI.ResetCartItem(cartItem);
            itemInCart.ResetInfor();
        }
    }

    private IEnumerator ProcessSpawnQueue()
    {
        isProcessingQueue = true;
        //waitingQueue.SetWaitingPanelActive(true);
        waitingGUI.SetWaitingPanelActive(true);

        while (spawnQueue.Count > 0)
        {
            var (itemid, obj, itemName) = spawnQueue.Peek();
            currentDelayTimer = spawnDelay;

            waitingGUI.UpdateWaitingItemName(itemName);

            while (currentDelayTimer > 0)
            {
                currentDelayTimer -= Time.deltaTime;
                waitingGUI.UpdateWaitingTimer(currentDelayTimer);
                yield return null;
            }
            spawnQueue.Dequeue();
            WaitingGUI.OnRemoveSlot?.Invoke(itemName);
            SpawnItem(itemid, obj);
            GetNextSpawnPosition(); 
        }

        isProcessingQueue = false;
        waitingGUI.SetWaitingPanelActive(false);
    }
    public Queue<(string itemid, GameObject obj, string itemName)> GetSpawnQueue()
    {
        return spawnQueue;
    }
    public float GetSpawnDelay()
    {
        return spawnDelay;
    }
    public float GetCurrentDelay()
    {
        return currentDelayTimer;
    }

    private void SpawnItem(string itemid, GameObject obj)
    {
        //FirebaseLogger.Ins.LogEvent("buy_item", new Firebase.Analytics.Parameter("id", itemid));
        //FirebaseLogger.Ins.LogEvent($"buy_item", new Parameter[] { new Parameter("id", itemid), new Parameter("level",ShopLevelManager.Ins.CurrentLevel.ToString() ) });
        Transform spawnPos = spawnItemPositions[currentSpawnIndex];
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("ORDER"));
        if (itemid.Contains("Tank"))
        {
            var boxInstance = Instantiate(box, spawnPos.position, Quaternion.identity,spawnPos);
            if (boxInstance.TryGetComponent<Box>(out var boxScript))
            {
                boxScript.SetItemInside(obj);
                PurchaseManager.OnAddToList?.Invoke(boxInstance);
                boxScript.transform.SetParent(PurchaseManager.Ins.gameObject.transform);
            }
        }
        else
        {
            var ftb = Instantiate(obj, spawnPos.position, Quaternion.identity,spawnPos);
            
            ftb.transform.SetParent(PurchaseManager.Ins.gameObject.transform);
            PurchaseManager.OnAddToList?.Invoke(ftb);
        }
    }

    private void GetNextSpawnPosition()
    {
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnItemPositions.Count;
    }

    public void ClearCart()
    {
        Vibrator.SoftVibrate();
        foreach (GameObject cartItem in itemCartList)
        {
            if (cartItem.activeSelf)
            {
                ItemInCart itemInCart = cartItem.GetComponent<ItemInCart>();
                if (itemInCart != null)
                {
                    shopGUI.ResetCartItem(cartItem);
                    itemInCart.ResetInfor();
                }
            }
        }
        total = 0;
        shopGUI.UpdateTotalPrice(total);
    }

    public void IncreaseTotal(float value) => UpdateTotal(total + value);
    public void DecreaseTotal(float value) => UpdateTotal(total - value);

    private void UpdateTotal(float newValue)
    {
        Vibrator.SoftVibrate();
        total = Mathf.Max(0, newValue);
        shopGUI.UpdateTotalPrice(total);
    }

    public void SaveSpawnQueue()
    {
        List<string> saveList = new List<string>();

        // Thêm phần tử đang spawn (delay còn lại)
        if (isProcessingQueue && spawnQueue.Count > 0)
        {
            var (itemid, _, _) = spawnQueue.Peek();
            saveList.Add($"{itemid}|{currentDelayTimer}");
        }

        // Thêm các phần tử còn lại (đặt delay = spawnDelay)
        bool isFirst = true;
        foreach (var (itemid, _, _) in spawnQueue)
        {
            if (isFirst) { isFirst = false; continue; }
            saveList.Add($"{itemid}|{spawnDelay}");
        }

        string data = string.Join(";", saveList);
        PlayerPrefs.SetString("SpawnQueueData", data);
        PlayerPrefs.Save();
    }

    public void LoadSpawnQueue()
    {
        string data = PlayerPrefs.GetString("SpawnQueueData", "");

        if (string.IsNullOrEmpty(data)) return;

        string[] entries = data.Split(';');

        foreach (string entry in entries)
        {
            string[] parts = entry.Split('|');
            if (parts.Length != 2) continue;

            string itemid = parts[0];
            float delay = float.Parse(parts[1]);

            GameObject prefab = null;
            string itemName = "";

            // Ưu tiên tìm trong FishTankBoxDatabase
            var tank = FishTankBoxDatabase.Instance?.GetFishTankByID(itemid);
            if (tank != null)
            {
                prefab = tank.prefab;
                itemName = tank.name;
            }
            else
            {
                var fallback = TankDatabase.Instance?.GetFishTankByID(itemid); 
                if (fallback != null)
                {
                    prefab = fallback.prefab;
                    itemName = fallback.name;
                }
            }

            if (prefab != null)
            {
                spawnQueue.Enqueue((itemid, prefab, itemName));
            }
        }

        if (!isProcessingQueue && spawnQueue.Count > 0)
            StartCoroutine(ProcessSpawnQueue());
    }

}
