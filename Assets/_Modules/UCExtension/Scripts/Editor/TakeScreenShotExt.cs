using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UCExtension
{
    public class TakeScreenShotExt : MonoBehaviour
    {
        [MenuItem("UCExtension/ TakeScreenShot")]
        public static void TakeScreenShotMenu()
        {
            string path = $"D:/Screenshots";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = path + "/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";
            Debug.Log(fileName);
            ScreenCapture.CaptureScreenshot(fileName);
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}