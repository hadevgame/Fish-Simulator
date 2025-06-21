using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public FixedTouchField _FixedTouchField;
    public CameraLook _CameraLook;

    void LateUpdate()
    {
        if (_FixedTouchField.TouchDelta.magnitude > 0.1f)
            _CameraLook.LockAxis = _FixedTouchField.TouchDelta;
        else
            _CameraLook.LockAxis = Vector2.zero;
    }
    
}
