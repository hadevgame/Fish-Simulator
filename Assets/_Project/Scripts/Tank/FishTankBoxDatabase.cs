using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankBoxDatabase : MonoBehaviour
{
    public static FishTankBoxDatabase Instance;
    public List<ItemData> tankTypes;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public ItemData GetFishTankByID(string id)
    {
        return tankTypes.Find(ft => ft.id == id);
    }
}
