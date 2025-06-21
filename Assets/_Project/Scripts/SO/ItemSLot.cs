using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemSlot", menuName = "ItemSlot")]
public class ItemSLot : BaseData
{
    public string id;
    public string name;
    public Sprite image;
    public float price;
    public float priceOne;
    public GameObject prefab;
    public int unlocklvl;

#if UNITY_EDITOR
    [Button] void CopyInfor() 
    {
        Name = name;
        this.SetDirtyAndSave();
    }
#endif
}


