using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.Audio;
using UnityEngine;

public class FishTankBox : MonoBehaviour
{
    public List<GameObject> fishList = new List<GameObject>();
    public ItemData tankData;
    [SerializeField] private Transform addPos;
    public Transform AddPos => addPos;
    [SerializeField] private Collider colliderPos;
    public bool isHasFish = false;

    private Transform originalParent;
    private Vector3 originalPosition;
    private float speed = 0.05f;
    private float padding = 0.004f;
    public void TakeFishFromBoxToTank(GameObject desTank)
    {
        if (fishList.Count > 0)
        {
            BaseTank ft = desTank.GetComponent<BaseTank>();

            if (ft.GetFishList().Count >= ft.MaxCreatureCount)
            {
                NotifiGUI.Instance.ShowPopup("The fish tank is full!");
                return;
            }
            GameObject fish = fishList[0];
            if (ft.AddCreature(fish)) 
            {
                AudioManager.Ins.PlaySFX(StoreManager.Instance.AudioSO.GetAudioClip("SPLASH"));
                fishList.RemoveAt(0);
                Debug.Log("Cá đã được chuyển vào bể!");
            }
            else
            {
                Debug.Log("Thêm cá thất bại!");
            }
        }
        if (fishList.Count == 0)
        {
            NotifiGUI.Instance.ShowPopup("Tank out of fish!");
        }
    }

    public void AddFish(GameObject obj) 
    {
        fishList.Add(obj);
        obj.transform.SetParent(transform);
        obj.transform.localScale = obj.transform.localScale / 2;
        bool isStarfish = obj.CompareTag(GameConstants.STARFISH);
        bool isCrab = obj.CompareTag(GameConstants.CRAB);
        var fishSwim = obj.GetComponent<FishSwim>();
        var amphibian = obj.GetComponent<AmphibianStateMachine>();
        if (isCrab)
        {
            obj.GetComponent<CrabMovement>().enabled = false;
           
        }
        else if(fishSwim != null && amphibian != null)
        {
            amphibian.StopState();
            amphibian.enabled = false;

            fishSwim.enabled = false;

        }
        else if (fishSwim != null)
        {
            fishSwim.SetUp(transform, speed, padding);
            fishSwim.SetMovingToTank(true);
        }

        Vector3 startPos = obj.transform.position; 
        Vector3 midPos = (startPos + addPos.position) / 2 + Vector3.up * 2f; 
        Vector3 endPos = addPos.position;
        obj.transform.DOPath(new Vector3[] { startPos, midPos, endPos }, 1f, PathType.CatmullRom)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
            {
                obj.transform.localPosition = addPos.localPosition;
                fishSwim.SetMovingToTank(false);
                Debug.Log("Cá đã vào bể!");
            });
        obj.transform.localPosition = addPos.localPosition;
    }

    public List<GameObject> GetAllFish() 
    {
        return fishList;
    }

    public Transform GetAddPos() 
    {
        return AddPos;
    }
    public void ClearAll()
    {
        foreach (var fish in fishList)
        {
            if (fish != null)
            {
                Destroy(fish);
            }
        }
        fishList.Clear();
        isHasFish = false;
    }
}
