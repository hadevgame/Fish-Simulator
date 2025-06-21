using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TankData", menuName = "Tank Data")]
public class TankData : ScriptableObject
{
    public string id;
    public GameObject prefab;
}
