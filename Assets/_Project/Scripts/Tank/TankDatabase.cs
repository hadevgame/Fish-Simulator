using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDatabase : MonoBehaviour
{
    public static TankDatabase Instance;
    public List<ItemSLot> tankTypes;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
   
    public ItemSLot GetFishTankByName(string name)
    {
        return tankTypes.Find(ft => ft.name == name);
    }
    public ItemSLot GetFishTankByID(string id)
    {
        return tankTypes.Find(ft => ft.id == id);
    }
}
