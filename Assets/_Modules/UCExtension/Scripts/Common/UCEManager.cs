using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UCExtension
{
    public class UCEManager : AutoInstantiateSingletonUC<UCEManager>
    {
        int numPaused;

        ClickTutorial tutorialPrefab;

        bool isShowingClickTutorial;

        public bool IsPaused => numPaused > 0;

        public void ShowClickTutorial(Canvas canvas, Button button, Action finish = null)
        {
            if (isShowingClickTutorial) return;
            isShowingClickTutorial = true;
            Pause();
            var clickTutorial = PoolObjects.Ins.Spawn(GetTutorialPrefab(), canvas.transform);
            clickTutorial.Show(button, () =>
            {
                isShowingClickTutorial = false;
                UnPause();
                finish?.Invoke();
            });
        }

        ClickTutorial GetTutorialPrefab()
        {
            tutorialPrefab = Resources.Load<ClickTutorial>("Prefabs/ClickTutorial");
            return tutorialPrefab;
        }

        public void Pause()
        {
            Time.timeScale = 0;
            numPaused++;
        }

        public void UnPause()
        {
            numPaused--;
            if (numPaused <= 0)
            {
                Time.timeScale = 1;
            }
        }
    }
}