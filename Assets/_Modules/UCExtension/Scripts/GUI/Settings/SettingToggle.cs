using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UCExtension.GUI
{
    [RequireComponent(typeof(ToggleButton))]
    public abstract class SettingToggle : MonoBehaviour
    {
        ToggleButton toggle;

        protected abstract bool StartState { get; }

        protected abstract void OnToggle(bool value);
        private void Awake()
        {
            toggle = GetComponent<ToggleButton>();
            toggle.OnToggle += OnToggle;
        }
        private void OnEnable()
        {
            toggle.SetUp(StartState);
        }
    }

}