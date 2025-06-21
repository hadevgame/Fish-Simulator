using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private float XMove;
    private float YMove;
    private float XRotation;
    [SerializeField] private Transform PlayerBody;
    public Vector2 LockAxis;
    public float Sensivity = 40f;
    public float smoothingSpeed = 5f; // Tốc độ mượt

    private Vector2 currentRotation;
    private Vector2 rotationVelocity;
    
    void Update()
    {
        if (LockAxis == Vector2.zero)
        {
            XMove = 0;
            YMove = 0;
        }
        else
        {
            float targetXMove = LockAxis.x * Sensivity * Time.deltaTime;
            float targetYMove = LockAxis.y * Sensivity * Time.deltaTime;

            XMove = Mathf.Lerp(XMove, targetXMove, Time.deltaTime * smoothingSpeed);
            YMove = Mathf.Lerp(YMove, targetYMove, Time.deltaTime * smoothingSpeed);
        }

        XRotation -= YMove;
        XRotation = Mathf.Clamp(XRotation, -70f, 70f);

        transform.localRotation = Quaternion.Euler(XRotation, 0, 0);
        PlayerBody.Rotate(Vector3.up * XMove);
    }
}
