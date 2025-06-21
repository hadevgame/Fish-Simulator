using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UnityEngine;

namespace UCExtension.GUI
{
    public class TransformFollowerManager : Singleton<TransformFollowerManager>
    {
        [SerializeField] RectTransform worldFollowerParent;

        [SerializeField] Canvas mainCavas;

        [SerializeField] Camera _mCamera;

        public T SpawnControlPositionUI<T>(T prefab, Transform followTarget) where T : TransformFollowerUI
        {
            var UI = PoolObjects.Ins.Spawn<T>(prefab, worldFollowerParent);
            UI.SetFollowTarget(followTarget).SetParent(worldFollowerParent).ResetTransform().ResetPos();
            return UI;
        }

        public Vector2 GetAnchorPos(RectTransform UIElementParent, Vector3 position)
        {
            TryGetCamera();
            var screenPos = _mCamera.WorldToScreenPoint(position);
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIElementParent, screenPos, mainCavas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mCamera, out anchoredPos);
            return anchoredPos;
        }

        public Vector3 GetWorldPos(RectTransform UIElementParent, Vector3 position)
        {
            TryGetCamera();
            var screenPos = _mCamera.WorldToScreenPoint(position);
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UIElementParent, screenPos, mainCavas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mCamera, out anchoredPos);
            return UIElementParent.position + (Vector3)anchoredPos;
        }

        void TryGetCamera()
        {
            if (!_mCamera)
            {
                Debug.Log("Get cam");
                _mCamera = Camera.main;
            }
        }
    }
}
