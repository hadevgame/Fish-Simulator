using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class PlayGUI : BaseGUI
{
    [SerializeField] private Button  buttonDrop, buttonPlacing;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private FixedTouchField touchField;
    public Button buttonInteract;
    private void Start()
    {
        InteractionController.UpdatePlacingState += UpdatePlacingButtonInteractivity;
        InteractionController.UpdatePlacingState += UpdateInteractButtonInteractivity;
        
    }

    public List<Button> GetButton() 
    {
        return new List<Button> { buttonInteract, buttonDrop, buttonPlacing };
    }
    public FixedJoystick GetJoyStick() 
    {
        return joystick;
    }
    public FixedTouchField GetTouchField()
    {
        return touchField;
    }
    public void UpdatePlacingButtonInteractivity(bool isInteractable)
    {
        buttonPlacing.interactable = isInteractable;
    }

    public void UpdateInteractButtonInteractivity(bool isInteractable)
    {
        buttonInteract.interactable = isInteractable;
    }
    public void ResetUIForNewObject()
    {
        ColorBlock cb = buttonInteract.colors;
        cb.normalColor = cb.highlightedColor = Color.white;
        buttonInteract.colors = cb;
    }

}
