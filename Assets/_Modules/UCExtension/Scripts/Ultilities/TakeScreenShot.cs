using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
#endif

namespace UCExtension.Common
{
    public class TakeScreenShot : Singleton<TakeScreenShot>
    {
        public string path = $"D:/Screenshots";
#if UNITY_EDITOR
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                Shot();
            }
        }

        public void Shot()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = path + "/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";
            Debug.Log(fileName);
            ScreenCapture.CaptureScreenshot(fileName);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}