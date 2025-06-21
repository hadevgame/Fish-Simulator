using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.Common;
using UCExtension.GUI;
using UnityEngine;
namespace UCExtension.GUI
{
    public class TransformFollowerUI : RecyclableObject
    {
        [SerializeField] bool notFollowByTransform;

        RectTransform rect;

        Transform target;

        RectTransform parent;

        Vector2 targetAnchorPos;

        Tween tween;

        Vector3 targetPosition;

        protected virtual void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Start()
        {
            parent = rect.parent.GetComponent<RectTransform>();
        }
        public TransformFollowerUI ResetTransform()
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return this;
        }

        public TransformFollowerUI SetParent(RectTransform parent)
        {
            this.parent = parent;
            return this;
        }

        public TransformFollowerUI SetFollowTarget(Transform target)
        {
            this.target = target;
            this.targetPosition = target.position;
            return this;
        }
        public TransformFollowerUI ResetPos()
        {
            rect.anchoredPosition = GetAnchorPos();
            targetAnchorPos = rect.anchoredPosition;
            return this;
        }

        protected virtual void Update()
        {
        }

        private void LateUpdate()
        {
            Follow();
        }

        void Follow()
        {
            targetAnchorPos = GetAnchorPos();
            rect.anchoredPosition = targetAnchorPos;
        }

        public void ScaleUp()
        {
            tween?.Kill();
            tween = gameObject.transform.DOScale(Vector3.one * 1.3f, 0.2f).SetEase(Ease.InOutSine);
        }

        public void ScaleDown()
        {
            tween?.Kill();
            tween = gameObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutSine);
        }

        Vector2 GetAnchorPos()
        {
            if (notFollowByTransform) return TransformFollowerManager.Ins.GetAnchorPos(parent, targetPosition);
            if (!target) return Vector2.one * 100f;
            return TransformFollowerManager.Ins.GetAnchorPos(parent, target.position);
        }
        public float GetRightZAngle(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle;
        }
    }
}