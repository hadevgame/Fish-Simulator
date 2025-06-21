using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
public class TankCreatureInfo
{
    public FishData fishData;
}
public abstract class BaseTank : MonoBehaviour
{
    [SerializeField] protected int maxCreatureCount = 8;
    protected List<GameObject> creatureList = new List<GameObject>();
    public int MaxCreatureCount => maxCreatureCount;

    [SerializeField] TankPriceUI priceUI;
    [SerializeField] protected Transform addPosition;
    [SerializeField] protected Transform waitPosition;
    [SerializeField] protected float speed;
    [SerializeField] protected float padding;

    public List<TankCreatureInfo> creatureInfoList = new List<TankCreatureInfo>();

    public Text countText;
    public bool isValidSlot;
    protected bool hasCreature = false;
    public ItemSLot tank;
    public Renderer highlightRenderer;
    protected virtual void Start()
    {
        UpdateCountText();
        priceUI.LoadInfo(creatureInfoList);
    }

    protected virtual void Update()
    {
        if (creatureList.Count == 0) hasCreature = false;
    }

    public virtual bool AddCreature(GameObject obj)
    {
        if (creatureList.Count >= maxCreatureCount)
        {
            NotifiGUI.Instance.ShowPopup("The tank is full!");
            return false;
        }

        creatureList.Add(obj);
        obj.transform.SetParent(transform);
        UpdateCountText();

        return true;
    }

    public virtual GameObject RemoveCreature()
    {
        if (creatureList.Count == 0)
        {
            hasCreature = false;
            return null;
        }

        GameObject obj = creatureList[0];
        creatureList.RemoveAt(0);
        obj.transform.SetParent(null);
        UpdateCountText();

        return obj;
    }

    protected void UpdateCountText()
    {
        if (countText != null)
            countText.text = $"{creatureList.Count}/{maxCreatureCount}";
    }

    protected void MoveToAddPosition(GameObject obj, TweenCallback onComplete)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 midPos = (startPos + addPosition.position) / 2 + Vector3.up * 2f;
        Vector3 endPos = addPosition.position;

        obj.transform.DOPath(new Vector3[] { startPos, midPos, endPos }, 1f, PathType.CatmullRom)
            .SetEase(Ease.InOutCubic)
            .OnComplete(onComplete);
    }
    public abstract GameObject GetFish();              
    public abstract bool IsHasFish();                  
    public abstract List<GameObject> GetFishList();   
    public abstract Transform GetAddPos();            
    public abstract Transform GetWaitPos();
}
