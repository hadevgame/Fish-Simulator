using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class PlacingGUI : BaseGUI
{
    [SerializeField] private Button btnRotateLeft;
    [SerializeField] private Button btnRotateRight;
    [SerializeField] private Button btnCancel;

    private void Start()
    {
        btnRotateLeft.onClick.AddListener(RotateLeft);
        btnRotateRight.onClick.AddListener(RotateRight);
        btnCancel.onClick.AddListener(Cancel);
    }
    public void RotateLeft() 
    {
        InteractionController.Ins.RotateLeft();

    }
    public void RotateRight()
    {
        InteractionController.Ins.RotateRight();
    }
    public void Cancel()
    {
        InteractionController.Ins.CancelPlacing();
        Close();

    }

    /*public override void Open()
    {
        base.Open();
        UCEManager.Ins.ShowClickTutorial(MainCanvas,btnCancel,null);
    }*/
}
