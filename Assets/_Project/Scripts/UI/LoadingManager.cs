using API;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Animation;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    void Start()
    {
        GUIController.Ins.Open<LoadingGUI>().SetWait(() => true).OnClose(() =>
        {
            AudioManager.Ins.ChangeBGMusic(GameManager.Instance.AudioSO.GetAudioClip("BGM"));
        });
    }
    
}
