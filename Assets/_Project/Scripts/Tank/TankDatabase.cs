using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDatabase : MonoBehaviour
{
    public static TankDatabase Instance;
    public List<ItemData> tankTypes;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
   
    public ItemData GetFishTankByName(string name)
    {
        return tankTypes.Find(ft => ft.name == name);
    }
    public ItemData GetFishTankByID(string id)
    {
        return tankTypes.Find(ft => ft.id == id);
    }
}
