using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    [SerializeField] private PurchaseManager purchaseManager;
    [SerializeField] private LevelManager shopLevelManager;
    [SerializeField] private CashDesk cashDesk;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private void Start()
    {
        LoadGameData();
        purchaseManager.LoadPurchasedItems();
        shopLevelManager = GUIController.Ins.Open<SceneGUI>().GetLevel();
        shopLevelManager.LoadData();
        GameTimeManager.Instance.LoadGameTime();
        ShopController.Ins.LoadSpawnQueue();
        cashDesk.LoadCustomerQueueData();
    }

    
    public void SaveGameData()
    {
        purchaseManager.SavePurchasedItems();
        shopLevelManager.SaveData();
        GameTimeManager.Instance.SaveGameTime();
        ShopController.Ins.SaveSpawnQueue();
        ;

        if (MoneyManager.instance != null )
        {
            PlayerPrefs.SetFloat("Money", MoneyManager.instance.CurrentMoney);
            PlayerPrefs.SetFloat("Income", MoneyManager.instance.Income);
            PlayerPrefs.SetFloat("Expense", MoneyManager.instance.Expense);
        }

        if (StoreManager.Instance != null )
        {
            PlayerPrefs.SetInt("IsShopOpen", StoreManager.Instance.GetState() ? 1 : 0);
        }

        // Lưu số lượng bể cá
        PlayerPrefs.SetInt("TankCount", TankManager.Instance.tanks.Count);
        for (int i = 0; i < TankManager.Instance.tanks.Count; i++)
        {
            GameObject tank = TankManager.Instance.tanks[i];

            BaseTank fishTank = tank.GetComponent<BaseTank>();
            if (fishTank != null && fishTank.tank != null)
            {
                string tankID = fishTank.tank.id;
                PlayerPrefs.SetString($"Tank_{i}_ID", tankID); // Lưu ID của bể
            }

            PlayerPrefs.SetFloat($"Tank_{i}_PosX", tank.transform.position.x);
            PlayerPrefs.SetFloat($"Tank_{i}_PosY", tank.transform.position.y);
            PlayerPrefs.SetFloat($"Tank_{i}_PosZ", tank.transform.position.z);
            PlayerPrefs.SetFloat($"Tank_{i}_RotY", tank.transform.eulerAngles.y);
        }

        // Lưu cá trong từng bể
        for (int i = 0; i < TankManager.Instance.tanks.Count; i++)
        {
            BaseTank fishTank = TankManager.Instance.tanks[i].GetComponent<BaseTank>();
            Dictionary<int, int> fishCount = new Dictionary<int, int>();

            foreach (var fish in fishTank.GetFishList())
            {
                FishData fishData = fish.GetComponent<Fish>().data;
                if (fishData != null)
                {
                    if (!fishCount.ContainsKey(fishData.fishID))
                        fishCount[fishData.fishID] = 0;

                    fishCount[fishData.fishID]++;
                }
            }

            foreach (var fishData in FishDatabase.Instance.fishTypes)
            {
                string key = $"Tank_{i}_Fish_{fishData.fishID}";

                if (fishCount.ContainsKey(fishData.fishID))
                {
                    PlayerPrefs.SetInt(key, fishCount[fishData.fishID]);
                }
                else if (PlayerPrefs.HasKey(key))
                {
                    PlayerPrefs.DeleteKey(key);
                }
            }
        }
        cashDesk.SaveCustomerQueueData();
        PlayerPrefs.Save();
    }

    public void LoadGameData()
    {
        if (MoneyManager.instance != null)
        {
            float money = PlayerPrefs.GetFloat("Money", 200);
            float income = PlayerPrefs.GetFloat("Income", 0);
            float expense = PlayerPrefs.GetFloat("Expense", 0);
            MoneyManager.instance.SetMoney(money, income, expense);
        }

        if (StoreManager.Instance != null)
        {
            bool isShopOpen = PlayerPrefs.GetInt("IsShopOpen", 0) == 1;
            StoreManager.Instance.SetStoreStatus(isShopOpen);
        }

        if (TankManager.Instance != null)
        {
            bool isFirstTime = PlayerPrefs.GetInt("HasSeenTutorial", 0) == 0;

            if (!isFirstTime)
            {
                // Xóa toàn bộ bể cũ
                foreach (var tank in TankManager.Instance.tanks)
                {
                    Destroy(tank);
                }
                TankManager.Instance.tanks.Clear();

                // Load lại bể từ dữ liệu lưu
                int tankCount = PlayerPrefs.GetInt("TankCount", 0);
                for (int i = 0; i < tankCount; i++)
                {
                    string tankID = PlayerPrefs.GetString($"Tank_{i}_ID", "");
                    float posX = PlayerPrefs.GetFloat($"Tank_{i}_PosX", 0);
                    float posY = PlayerPrefs.GetFloat($"Tank_{i}_PosY", 0);
                    float posZ = PlayerPrefs.GetFloat($"Tank_{i}_PosZ", 0);
                    float rotY = PlayerPrefs.GetFloat($"Tank_{i}_RotY", 0);

                    ItemData tankSlot = TankDatabase.Instance.tankTypes.Find(t => t.id == tankID);
                    if (tankSlot != null)
                    {
                        GameObject prefab = tankSlot.prefab;
                        GameObject newTank = Instantiate(prefab, new Vector3(posX, posY, posZ), Quaternion.Euler(0, rotY, 0));
                        TankManager.Instance.tanks.Add(newTank);
                    }
                }
            }

            // Load cá cho từng bể hiện có
            for (int i = 0; i < TankManager.Instance.tanks.Count; i++)
            {
                BaseTank fishTank = TankManager.Instance.tanks[i].GetComponent<BaseTank>();
                if (fishTank != null)
                {
                    fishTank.GetFishList().Clear();
                    foreach (var fishData in FishDatabase.Instance.fishTypes)
                    {
                        int fishCount = PlayerPrefs.GetInt($"Tank_{i}_Fish_{fishData.fishID}", 0);
                        for (int j = 0; j < fishCount; j++)
                        {
                            GameObject fishInstance = Instantiate(fishData.fishPrefab, fishTank.GetAddPos().position, Quaternion.identity);
                            
                            fishInstance.GetComponent<Fish>().data = fishData;
                            fishTank.AddCreature(fishInstance);
                        }
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameData();
        }
    }
}



