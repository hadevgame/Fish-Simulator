using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FishData", menuName = "Fish")]
public class FishData : ScriptableObject
{
    public int fishID;
    public string name;
    public GameObject fishPrefab;
    public string tank;
    public Sprite icon;
    public float basePrice;
}
