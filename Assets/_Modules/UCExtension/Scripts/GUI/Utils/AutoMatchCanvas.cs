using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UCExtension.GUI
{
    public class AutoMatchCanvas : MonoBehaviour
    {
        public float defaultWidth = 720;
        public float defaultHeight = 1280;
        // Start is called before the first frame update
        void Awake()
        {
            float currentRatio = Screen.width * 1f / Screen.height;
            float defaultRatio = defaultWidth * 1f / defaultHeight;
            if (currentRatio > defaultRatio)
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            }
            else
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}