using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UCExtension.Audio;

public class FishTank : BaseTank
{
    public override bool AddCreature(GameObject obj)
    {
        if (obj.GetComponent<Fish>().data.tank != "v1") 
        {
            NotifiGUI.Instance.ShowPopup("This fish doesn't belong in this tank!");
            return false;
        } 

        if (!base.AddCreature(obj)) return false;

        var fishSwim = obj.GetComponent<FishSwim>();
        fishSwim.enabled = true;
        if (fishSwim != null)
        {
            fishSwim.SetUp(transform, speed, padding);
            fishSwim.SetMovingToTank(true);
        }
        
        MoveToAddPosition(obj, () =>
        {
            hasCreature = true;
            if (fishSwim != null)
            {
                fishSwim.speed = 2f;
                obj.transform.localScale = obj.transform.localScale * 2;
                fishSwim.SetMovingToTank(false);
            }
        });

        return true;
    }
    public override GameObject GetFish()
    {
        if (creatureList.Count == 0)
        {
            Debug.Log("Không có cá trong bể để lấy ra!");
            return null;
        }

        GameObject fish = creatureList[0];
        creatureList.RemoveAt(0);
        fish.transform.SetParent(null);
        UpdateCountText();
        return fish;
    }
    public override bool IsHasFish()
    {
        return hasCreature;
    }

    public override List<GameObject> GetFishList()
    {
        return new List<GameObject>(creatureList);
    }

    public override Transform GetAddPos()
    {
        return addPosition;
    }

    public override Transform GetWaitPos()
    {
        return waitPosition;
    }
}


