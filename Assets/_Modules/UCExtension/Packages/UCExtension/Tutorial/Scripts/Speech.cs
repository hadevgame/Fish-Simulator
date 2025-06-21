using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Speech : MonoBehaviour
{
    [SerializeField] Button mainButton;

    [SerializeField] Text speechText;

    [SerializeField] float timeDoText;

    [SerializeField] float maxTimeWait = 1;

    List<string> conversationContents;

    int conversationIndex;

    Action onFinishConversation;

    Tween textingTween;

    bool isTexting;

    float timeFinish;

    bool isTutoring;

    private void Start()
    {
        mainButton.onClick.AddListener(OnClick);
    }

    public void StartConversation(List<string> contents, Action finish)
    {
        this.conversationContents = contents;
        conversationIndex = -1;
        onFinishConversation = finish;
        gameObject.SetActive(true);
        NextSpeech();
    }

    private void Update()
    {
        if (!isTexting && Time.time > timeFinish + maxTimeWait)
        {
            NextSpeech();
        }
    }

    public void NextSpeech()
    {
        conversationIndex++;
        if (conversationIndex >= conversationContents.Count)
        {
            FinishConversation();
            return;
        }
        string content = conversationContents[conversationIndex].Replace("_", "\n");
        isTexting = true;
        textingTween?.Kill();
        speechText.text = "";
        textingTween = speechText.DOText(content, timeDoText * content.Length).SetEase(Ease.Linear).OnComplete(() =>
        {
            isTexting = false;
            timeFinish = Time.time;
        });
    }

    void FinishConversation()
    {
        onFinishConversation?.Invoke();
        gameObject.SetActive(false);
    }

    void OnClick()
    {
        if (isTexting)
        {
            timeFinish = Time.time;
            isTexting = false;
            textingTween?.Kill();
            string content = conversationContents[conversationIndex].Replace("_", "\n");
            speechText.text = content;
        }
        else
        {
            NextSpeech();
        }
    }
}
