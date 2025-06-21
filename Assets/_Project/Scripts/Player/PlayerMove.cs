using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public FixedJoystick joystick;
    public float moveSpeed = 5f;
    private CharacterController controller;
    private float verticalVelocity = 0f;
    private float gravity = 9.81f;

    public bool isPCControl;

    private bool canmove = true;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        joystick = GUIController.Ins.Open<PlayGUI>().GetJoyStick();
        GetComponent<TouchController>()._FixedTouchField = GUIController.Ins.Open<PlayGUI>().GetTouchField();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (canmove == false) return;
        Vector3 move = Vector3.zero;
        if (isPCControl)
        {
            float horizontalInput = Input.GetAxis("Horizontal"); 
            float verticalInput = Input.GetAxis("Vertical");     

            move = transform.right * horizontalInput + transform.forward * verticalInput;
        }
        else
        {
            float joystickHorizontal = joystick.Horizontal;
            float joystickVertical = joystick.Vertical;     
            move = transform.right * joystickHorizontal + transform.forward * joystickVertical;

        }

        move = move.normalized * moveSpeed * Time.deltaTime;
        
        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime; 
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; 
        }

        move.y = verticalVelocity;
        
        controller.Move(move);
    }

    public void SetMove(bool ismove) 
    {
        canmove = ismove;
    }
}
