using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UnityEngine;

public class Amphibian_ReptileTank : BaseTank
{
    [SerializeField] private Transform waterTransform;
    [SerializeField] private Transform groundTransform;
    [SerializeField] private Transform waterPoint;
    [SerializeField] private Transform groundPoint;

    public override bool AddCreature(GameObject obj)
    {
        if (obj.GetComponent<Fish>().data.tank != "v3")
        {
            NotifiGUI.Instance.ShowPopup("This fish doesn't belong in this tank!");
            return false;
        } 
        if (!base.AddCreature(obj)) return false;

        var fishSwim = obj.GetComponent<FishSwim>();
        var amphibianSM = obj.GetComponent<AmphibianStateMachine>();

        bool isAxolotl = obj.CompareTag(GameConstants.AXOLOTL);

        if (isAxolotl)
        {
            if (fishSwim != null)
            {
                fishSwim.SetUp(waterTransform, speed * 4, padding);
                fishSwim.SetMovingToTank(true);
            }
        }
        else if (amphibianSM != null)
        {
            amphibianSM.SetUp(speed, padding);
            amphibianSM.SetWaterNGround(waterTransform, groundTransform, waterPoint, groundPoint);
        }
        MoveToAddPosition(obj, () =>
        {
            hasCreature = true;
            obj.transform.localScale = obj.transform.localScale * 1.2f;
            if (isAxolotl)
            {
                if (fishSwim != null)
                {
                    fishSwim.SetMovingToTank(false);
                    fishSwim.transform.SetParent(waterTransform);
                }
            }
            else
            {
                if (fishSwim != null) fishSwim.enabled = false;
                if (amphibianSM != null) amphibianSM.BeginState();
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
