using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UCExtension.Audio;

public class InvertebrateTank : BaseTank
{
    [SerializeField] private List<Transform> starfishPositions = new List<Transform>();
    private Dictionary<Transform, GameObject> starfishSlots = new Dictionary<Transform, GameObject>();
    protected override void Start()
    {
        base.Start();
        foreach (var slot in starfishPositions)
            starfishSlots[slot] = null;
    }

    public override bool AddCreature(GameObject obj)
    {
        if (obj.GetComponent<Fish>().data.tank != "v2") 
        {
            NotifiGUI.Instance.ShowPopup("This fish doesn't belong in this tank!");
            return false;
        }

        if (!base.AddCreature(obj)) return false;

        bool isStarfish = obj.CompareTag(GameConstants.STARFISH);
        bool isCrab = obj.CompareTag(GameConstants.CRAB);

        if (isCrab)
        {
            var crab = obj.GetComponent<CrabMovement>();
            crab.enabled = true;
            crab.tankBoundsCollider = GetComponent<BoxCollider>();
            crab.SetMovingToTank(true);
        }
        else
        {
            var fishSwim = obj.GetComponent<FishSwim>();
            if (fishSwim != null)
            {
                fishSwim.SetUp(transform, speed, padding);
                fishSwim.SetMovingToTank(true);
            }
        }
        MoveToAddPosition(obj, () =>
        {
            hasCreature = true;
            obj.transform.localScale = obj.transform.localScale * 1.5f;
            if (isStarfish)
            {
                AssignStarfishToSlot(obj);
            }
            else if (isCrab)
            {
                var crab = obj.GetComponent<CrabMovement>();
                crab.SetMovingToTank(false);
                crab.Setup();
            }
            else if (!isCrab)
            {
                var fishSwim = obj.GetComponent<FishSwim>();
                fishSwim.speed = 2f;
                fishSwim.SetMovingToTank(false);
            }
        });

        return true;
    }

    public override GameObject RemoveCreature()
    {
        GameObject obj = base.RemoveCreature();
        if (obj != null && obj.CompareTag(GameConstants.STARFISH))
        {
            RemoveStarfishFromSlot(obj);
        }
        return obj;
    }

    private void AssignStarfishToSlot(GameObject starfish)
    {
        foreach (var kvp in starfishSlots)
        {
            if (kvp.Value == null)
            {
                starfishSlots[kvp.Key] = starfish;
                float tankYRotation = transform.eulerAngles.y;
                starfish.transform.localRotation = Quaternion.Euler(-tankYRotation, 0f, -90);
                starfish.transform.DOMove(kvp.Key.position, 1.5f).SetEase(Ease.OutSine);
                break;
            }
        }
    }

    private void RemoveStarfishFromSlot(GameObject starfish)
    {
        foreach (var kvp in starfishSlots)
        {
            if (kvp.Value == starfish)
            {
                starfishSlots[kvp.Key] = null;
                break;
            }
        }
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
        //fish.transform.SetParent(null);
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
