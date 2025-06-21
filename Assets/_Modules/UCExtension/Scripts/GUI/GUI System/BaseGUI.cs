using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UCExtension.Animation;
using UnityEngine.UI;
using System;
using Sirenix.OdinInspector;
using System.Linq;

namespace UCExtension.GUI
{
    [RequireComponent(typeof(GUIAnim))]
    [DisallowMultipleComponent]
    public abstract class BaseGUI : MonoBehaviour
    {
        protected GUIAnim anim;

        Canvas mainCanvas;

        public Canvas MainCanvas
        {
            get
            {
                if (!mainCanvas)
                {
                    mainCanvas = GetComponent<Canvas>();
                }
                return mainCanvas;
            }
        }

        Action CloseCallback;

        public string GuiName => GetType().Name;

        protected virtual void Awake()
        {
            SetUp();
        }

        protected virtual void SetUp()
        {
            anim = GetComponent<GUIAnim>();
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            anim.PlayAnim(GraphicState.Visible, () =>
            {
                OnOpened();
            });
        }

        public virtual void Close()
        {
            CloseCallback?.Invoke();
            CloseCallback = null;
            anim.PlayAnim(GraphicState.Hidden, () =>
            {
                gameObject.SetActive(false);
                OnClosed();
            });
        }

        public void OnClose(Action callback)
        {
            CloseCallback = callback;
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnClosed()
        {
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [HideInInspector]
        bool correctName;

        [OnInspectorInit]
        void OnInspector()
        {
            correctName = gameObject.name == GuiName;
        }

        [HideIfGroup("correctName")]
        [InfoBox("<color=red>This GUI cannot be initialized because the name is wrong. " +
            "Please change the name to use this GUI!</color>")]
        [Button("Rename")]
        public void Rename()
        {
            gameObject.name = GuiName;
            gameObject.SetDirtyAndSave();
        }
#endif
    }
    public static class BaseGUIExtension
    {
        public static List<string> AllImplements()
        {
            List<string> result = new List<string>();
            var type = typeof(BaseGUI);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);
            foreach (var item in types)
            {
                result.Add(item.Name);
            }
            return result;
        }
    }
}
