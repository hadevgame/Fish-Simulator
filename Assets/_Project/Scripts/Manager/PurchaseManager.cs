using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;
using UnityEngine.UIElements;

public class PurchaseManager : Singleton<PurchaseManager>
{
    [SerializeField] private List<GameObject> purchasedItems = new List<GameObject>(); 
    public static Action<GameObject> OnAddToList = null;
    public static Action<GameObject> OnRemoveFromList = null;

    [SerializeField] private GameObject boxPrefab;

    private void Start()
    {
        OnAddToList = AddItemToList;
        OnRemoveFromList = RemoveItemFromList;
    }
    private void OnDisable()
    {
        OnAddToList = null;
        OnRemoveFromList = null;
    }
    public void AddItemToList(GameObject item)
    {
        purchasedItems.Add(item);
    }
    public void RemoveItemFromList(GameObject item)
    {
        if (purchasedItems.Contains(item))
        {
            purchasedItems.Remove(item);
        }
    }
    public GameObject GetFirstPurchasedItem()
    {
        if ( purchasedItems.Count > 0)
        {
            return purchasedItems[0];
        }
        else return null;
    }
    public void SavePurchasedItems()
    {
        PlayerPrefs.SetInt("PurchasedItemCount", purchasedItems.Count);

        for (int i = 0; i < purchasedItems.Count; i++)
        {
            GameObject item = purchasedItems[i];

            if (item.TryGetComponent(out Box box))
            {
                PlayerPrefs.SetString($"PurchasedItem_{i}_Type", "Box");

                PlayerPrefs.SetFloat($"Box_{i}_PosX", item.transform.position.x);
                PlayerPrefs.SetFloat($"Box_{i}_PosY", item.transform.position.y);
                PlayerPrefs.SetFloat($"Box_{i}_PosZ", item.transform.position.z);

                if (box.itemInside != null)
                {
                    ItemSLot tank = box.itemInside.GetComponent<BaseTank>().tank;
                    if (tank != null)
                    {
                        string tankName = tank.name;
                        PlayerPrefs.SetString($"Tank_{i}_TankName", tankName); 
                    }
                }
            }
            else
            {
                FishTankBox tankBoxScript = item.GetComponent<FishTankBox>();
               
                PlayerPrefs.SetFloat($"FishTankBox_{i}_PosX", item.transform.position.x);
                PlayerPrefs.SetFloat($"FishTankBox_{i}_PosY", item.transform.position.y);
                PlayerPrefs.SetFloat($"FishTankBox_{i}_PosZ", item.transform.position.z);

                ItemSLot tankboxData = tankBoxScript.tankData;
                if (tankboxData != null)
                {
                    string tankID = tankboxData.id;
                    PlayerPrefs.SetString($"FishTankBox_{i}_TankID", tankID); 
                }
                
                Dictionary<int, int> fishCount = new Dictionary<int, int>();

                foreach (var fish in tankBoxScript.fishList)
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
                    string key = $"FishTankBox_{i}_Fish_{fishData.fishID}";
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
        }
        PlayerPrefs.Save();
    }

    public void LoadPurchasedItems()
    {
        int itemCount = PlayerPrefs.GetInt("PurchasedItemCount", 0);

        purchasedItems.Clear();

        for (int i = 0; i < itemCount; i++)
        {
            string itemType = PlayerPrefs.GetString($"PurchasedItem_{i}_Type", "");

            if (itemType == "Box")
            {
                float posX = PlayerPrefs.GetFloat($"Box_{i}_PosX", 0);
                float posY = PlayerPrefs.GetFloat($"Box_{i}_PosY", 0);
                float posZ = PlayerPrefs.GetFloat($"Box_{i}_PosZ", 0);
                Vector3 spawnPosition = new Vector3(posX, posY, posZ);
                GameObject box = Instantiate(boxPrefab, spawnPosition, Quaternion.Euler(0, 0, 0));

                string tankName = PlayerPrefs.GetString($"Tank_{i}_TankName", "0"); 
                if (tankName != "0")
                {
                    foreach (var tankData in TankDatabase.Instance.tankTypes)
                    {
                        if (tankData.name == tankName) 
                        {
                            box.GetComponent<Box>().SetItemInside(tankData.prefab);
                        }
                    }
                }
                AddItemToList(box);
                box.transform.SetParent(transform);
                //purchasedItems.Add(box);
            }
            else 
            {
                float posX = PlayerPrefs.GetFloat($"FishTankBox_{i}_PosX", 0);
                float posY = PlayerPrefs.GetFloat($"FishTankBox_{i}_PosY", 0);
                float posZ = PlayerPrefs.GetFloat($"FishTankBox_{i}_PosZ", 0);
                Vector3 spawnPosition = new Vector3(posX, posY, posZ);

                string tankID = PlayerPrefs.GetString($"FishTankBox_{i}_TankID", "0"); 
                if (tankID != "0")
                {
                    foreach (var tankData in FishTankBoxDatabase.Instance.tankTypes)
                    {
                        if (tankData.id == tankID) 
                        {
                            GameObject tankInstance = Instantiate(tankData.prefab, spawnPosition, Quaternion.identity);
                            FishTankBox fishTankBox = tankInstance.GetComponent<FishTankBox>();
                            fishTankBox.tankData = tankData;

                            foreach (GameObject fish in fishTankBox.fishList)
                            {
                                Destroy(fish);  
                            }

                            fishTankBox.fishList.Clear();

                            foreach (var fishData in FishDatabase.Instance.fishTypes)
                            {
                                int fishCount = PlayerPrefs.GetInt($"FishTankBox_{i}_Fish_{fishData.fishID}", 0);
                                for (int j = 0; j < fishCount; j++)
                                {
                                    //GameObject fishObj = Instantiate(fishData.fishPrefab, fishTankBox.AddPos.position , Quaternion.identity);
                                    //fishTankBox.AddFish(fishObj);
                                    StartCoroutine(DelayAddFish(fishData.fishPrefab, fishTankBox));
                                }
                            }
                            AddItemToList(tankInstance);
                            tankInstance.transform.SetParent(transform);
                            //purchasedItems.Add(tankInstance);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator DelayAddFish(GameObject prefab, FishTankBox fishTankBox) 
    {
        yield return new WaitForSeconds(0.5f);
        GameObject fishObj = Instantiate(prefab, fishTankBox.AddPos.position, Quaternion.identity);
        fishObj.transform.localScale = fishObj.transform.localScale * 1.5f;
        fishTankBox.AddFish(fishObj);
    }
}

