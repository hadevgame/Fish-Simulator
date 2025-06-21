using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public class FpsLogger : MonoBehaviour
    {
        [SerializeField] bool showFps;
        [SerializeField] Text fpsText;
        [SerializeField] Color goodFpsColor = new Color(0f, 1, 0f);
        [SerializeField] Color badFpsColor = new Color(1, 0, 0);
        [SerializeField] int goodFps = 55;

        int fpsCount = 0;
        float nextTime = 0;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (showFps)
            {
                fpsCount++;
                if (Time.time > nextTime)
                {
                    if (fpsCount >= goodFps)
                    {
                        fpsText.color = goodFpsColor;
                    }
                    else
                    {

                        fpsText.color = badFpsColor;
                    }
                    fpsText.text = "FPS: " + fpsCount;
                    fpsCount = 0;
                    nextTime = Time.time + 1f;
                }
            }
            else
            {
                fpsText.text = "";
            }
        }
    }
}