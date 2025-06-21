using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UCExtension.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace UCExtension.GUI
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] List<ToggleConfigs> configs;

        Button button;

        bool isActive = false;

        public Action<bool> OnToggle;

        public static UnityAction<string, bool> OnToggleCallback;

        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        public void SetUp(bool isAtive)
        {
            this.isActive = isAtive;
            ResetApearance();
        }
        void OnClick()
        {
            isActive = !isActive;
            OnToggle?.Invoke(isActive);
            OnToggleCallback?.Invoke(gameObject.name, isActive);
            ResetApearance();
        }
        void ResetApearance()
        {
            foreach (var item in configs)
            {
                if (item.MainImage)
                {
                    item.MainImage.sprite = isActive ? item.ActiveSprite : item.DisableSprite;
                    if (item.SetNativeSize)
                    {
                        item.MainImage.SetNativeSize();
                    }
                }
                if (item.TitleText)
                {
                    item.TitleText.text = isActive ? item.ActiveTitle : item.DisableTitle;
                }
            }
        }
    }


    [System.Serializable]
    public class ToggleConfigs
    {
        [FoldoutGroup("Image")]
        public Image MainImage;

        [ShowIf("MainImage")]
        [FoldoutGroup("Image")]
        public bool SetNativeSize;

        [ShowIf("MainImage")]
        [FoldoutGroup("Image")]
        public Sprite DisableSprite;

        [ShowIf("MainImage")]
        [FoldoutGroup("Image")]
        public Sprite ActiveSprite;

        [FoldoutGroup("Title")]
        public Text TitleText;

        [ShowIf("TitleText")]
        [FoldoutGroup("Title")]
        public string DisableTitle;

        [ShowIf("TitleText")]
        [FoldoutGroup("Title")]
        public string ActiveTitle;
    }
}

