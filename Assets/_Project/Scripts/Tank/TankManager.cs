using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public List<GameObject> tanks = new List<GameObject>();
    public static Action<GameObject> OnTankAdded;
    public static Action<GameObject> OnTankRemoved;

    public static TankManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        OnTankAdded = AddTank;
        OnTankRemoved = RemoveTank;
    }

    private void OnDisable()
    {
        OnTankAdded = null;
        OnTankRemoved = null;
    }

    private void AddTank(GameObject tank)
    {
        if (!tanks.Contains(tank))
        {
            tanks.Add(tank);
        }
    }

    private void RemoveTank(GameObject tank)
    {
        if (tanks.Contains(tank))
        {
            tanks.Remove(tank);
        }
    }

    public BaseTank HasValidSlot()
    {
        foreach (GameObject tank in tanks)
        {
            BaseTank basetank = tank.GetComponent<BaseTank>();
            if (basetank != null && basetank.isValidSlot)
            {
                basetank.isValidSlot = false;
                return basetank;
            }
        }
        return null;
    }

    
}

