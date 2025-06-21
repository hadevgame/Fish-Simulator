using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    [RequireComponent(typeof(Camera))]
    public class AutoSizeCamera : MonoBehaviour
    {
        Camera mainCamera;

        [SerializeField] Vector2 defaultScreen = new Vector2(1920, 1080);

        [SerializeField] AutoSizeMatchType matchType;

        float defaultOrthoSize;

        float defaultFieldOfView;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            defaultOrthoSize = mainCamera.orthographicSize;
            defaultFieldOfView = mainCamera.fieldOfView;
            UpdateSize();
        }

        public void UpdateSize()
        {
            float ratio = CameraExtension.GetSizeRatioByScreen(matchType, defaultScreen); ;
            mainCamera.orthographicSize = defaultOrthoSize * ratio;
            mainCamera.fieldOfView = defaultFieldOfView * ratio;
        }
    }

}