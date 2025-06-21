using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDatabase : MonoBehaviour
{
    public static FishDatabase Instance;
    public List<FishData> fishTypes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public FishData GetFishByID(int id)
    {
        return fishTypes.Find(f => f.fishID == id);
    }
}
