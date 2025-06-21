using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class HoverGUI : BaseGUI
{
    [SerializeField] private Text nametext;
    [SerializeField] private Text counttext;
    [SerializeField] private Image imageItem;

    public void SetData(GameObject obj)
    {
        FishTankBox ftb = obj.GetComponent<FishTankBox>();
        nametext.text = ftb.tankData.name;
        counttext.text = ftb.fishList.Count.ToString();
        imageItem.sprite = ftb.tankData.image;
    }
}
