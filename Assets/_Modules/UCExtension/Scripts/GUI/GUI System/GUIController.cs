using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UCExtension.Common;
using System.Reflection;

namespace UCExtension.GUI
{
    public class GUIController : AutoInstantiateSingletonUC<GUIController>
    {
        [SerializeField] bool showLog;

        Dictionary<string, BaseGUI> instantiatedGUIs = new Dictionary<string, BaseGUI>();

        Camera mainCamera;

        string resourcePath = "GUI/";

        public void HideAllGUI()
        {
            foreach (var item in instantiatedGUIs)
            {
                item.Value.Close();
            }
        }

        public T Open<T>() where T : BaseGUI
        {
            T gui = GetGUI<T>();
            gui.Open();
            return gui;
        }

        public T Close<T>() where T : BaseGUI
        {
            T gui = GetGUI<T>();
            gui.Close();
            return gui;
        }

        T GetGUI<T>() where T : BaseGUI
        {
            string guiName = typeof(T).Name;
            return GetGUI<T>(GetSubPath<T>(), guiName);
        }

        string GetSubPath<T>() where T : BaseGUI
        {
            var attribute = typeof(T).GetCustomAttribute<GUISubPathAttribute>();
            return attribute?.Path ?? "";
        }

        T GetGUI<T>(string subPath, string guiName) where T : BaseGUI
        {
            T gui = null;
            if (instantiatedGUIs.ContainsKey(guiName))
            {
                gui = instantiatedGUIs[guiName] as T;
            }
            if (!gui)
            {
                string prefabPath = $"{resourcePath}{subPath}{guiName}";
                T prefabGui = Resources.Load<T>(prefabPath);
                if (prefabGui)
                {
                    LogInfor($"Instantiated GUI: {guiName}");
                    gui = Instantiate(prefabGui, transform);
                    gui.name = guiName;
                    gui.MainCanvas.worldCamera = mainCamera;
                    instantiatedGUIs.Add(guiName, gui);
                }
                else
                {
                    LogError($"Cant find {guiName}. Please check path and prefab name to instantiate this GUI.");
                }
            }
            return gui;
        }

        void LogInfor(string content)
        {
            if (!showLog) return;
            Debug.Log($"[GuiController] {content}");
        }

        void LogError(string content)
        {
            Debug.LogError($"[GuiController] {content}");
        }
    }
}