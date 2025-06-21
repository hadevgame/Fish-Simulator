using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "AutoShooter/Datas/InventoryData")]
public class InventoryData : BaseData
{
    [SerializeField] int defaultQuantity = 0;

    public static Action<InventoryData, bool> OnChangeQuantity;

    const string INVENTORY_QUANTITIES = "INVENTORY_QUANTITIES";

    public int Quantity
    {
        get
        {
            return PlayerPrefs.GetInt(INVENTORY_QUANTITIES + ID, defaultQuantity);
        }
        protected set
        {
            PlayerPrefs.SetInt(INVENTORY_QUANTITIES + ID, value);

        }
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
        OnChangeQuantity?.Invoke(this, true);
    }
    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
        OnChangeQuantity?.Invoke(this, true);
    }

    public bool TryConsume(int quantity)
    {
        if (Quantity >= quantity)
        {
            Quantity -= quantity;
            OnComsume();
            OnChangeQuantity?.Invoke(this, false);
            return true;
        }
        return false;
    }

    protected virtual void OnComsume()
    {
    }
}

[System.Serializable]
public class QuantityData<T> where T : BaseData
{
    public T Data;

    public int Quantity;
}

[System.Serializable]
public class TemporaryInventory: QuantityData<InventoryData>
{
    public bool HasEnoughQuantity => Data.Quantity >= Quantity;

    public void AddQuantity()
    {
        Data.AddQuantity(Quantity);
    }

    public void SubtractQuantity()
    {
        if (!HasEnoughQuantity) return;
        Data.TryConsume(Quantity);
    }
}

[System.Serializable]
public class ListTemporaryInventory
{
    public List<TemporaryInventory> Temporaries;

    public bool HasEnoughQuantities
    {
        get
        {
            foreach (var item in Temporaries)
            {
                if (!item.HasEnoughQuantity) return false;
            }
            return true;
        }
    }

    public void SubtractQuantity()
    {
        foreach (var item in Temporaries)
        {
            item.SubtractQuantity();
        }
    }
    public void AddQuantity()
    {
        foreach (var item in Temporaries)
        {
            item.AddQuantity();
        }
    }

    public InventoryData GetNotEnoughInventory()
    {
        foreach (var item in Temporaries)
        {
            if (!item.HasEnoughQuantity)
            {
                return item.Data;
            }
        }
        return null;
    }
}