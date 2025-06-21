using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UCExtension.GUI
{
    public class ButtonScale : Button
    {
        [ShowInInspector]
        public bool muteClickSound;

        protected bool isPressed = false;

        float zoomInScale = 0.9f;

        float zoomSpeed = 0.5f;

        public static UnityAction<string> OnClickButtonCallback;

        public UnityAction OnPointerDownCallback;

        private enum ButtonScaleType
        {
            ZoomIn,
            ZoomOut
        }

        protected override void OnDisable()
        {
            isPressed = false;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            OnClickButtonCallback?.Invoke(gameObject.name);
            if (!muteClickSound)
            {
                AudioManager.Ins.PlayButtonClickSFX();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable) return;
            OnPointerDownCallback?.Invoke();
            isPressed = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPressed = false;
        }

        private void LateUpdate()
        {
            Vector3 targetScale = Vector3.one * zoomInScale;
            if (isPressed)
            {
                targetScale = Vector3.one * zoomInScale;
            }
            else
            {
                targetScale = Vector3.one;
            }
            if (transition == Transition.ColorTint)
            {
                targetGraphic.transform.localScale = Vector3.Lerp(targetGraphic.transform.localScale, targetScale, zoomSpeed);

            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, zoomSpeed);
            }
        }

#if UNITY_EDITOR
        public void FixGraphic()
        {
            if (!targetGraphic)
            {
                Image img = GetComponent<Image>();
                if (img)
                {
                    targetGraphic = img;
                }
                else
                {
                    Debug.LogError("Cant find Image to fix!");
                    return;
                }
            }
            if (targetGraphic is Image)
            {
                var oldImage = targetGraphic as Image;
                List<Transform> childs = transform.ChildsList();
                GameObject newObject = new GameObject("Graphic");
                var rect = newObject.AddComponent<RectTransform>();
                rect.SetParent(transform);
                rect.localScale = Vector3.one;
                rect.localPosition = Vector3.zero;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 1);
                var newImg = rect.gameObject.AddComponent<Image>();
                newImg.sprite = oldImage.sprite;
                newImg.SetDirtyAndSave();
                oldImage.SetAlpha(0);
                oldImage.SetDirtyAndSave();
                foreach (var item in childs)
                {
                    item.SetParent(rect);
                }
                targetGraphic = newImg;
                this.SetDirtyAndSave();
            }
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ButtonScale))]
public class ButtonScaleCustomEditor : UnityEditor.UI.ButtonEditor
{
    bool show = false;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        var script = target as ButtonScale;
        show = EditorGUILayout.Foldout(show, "Custom");
        if (show)
        {
            script.muteClickSound = GUILayout.Toggle(script.muteClickSound, "Mute click sound");
            if (GUILayout.Button("Fix Graphic"))
            {
                script.FixGraphic();
            }
            EditorUtility.SetDirty(script);
        }
        serializedObject.ApplyModifiedProperties();


    }
}
#endif