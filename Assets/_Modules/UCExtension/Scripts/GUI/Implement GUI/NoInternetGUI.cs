using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NoInternetGUI : MonoBehaviour
{
    [SerializeField] GameObject popup;

    [SerializeField] float timeDelay=1;

    bool isRequesting;

    void Start()
    {
        popup.gameObject.SetActive(false);
    }

    private void Update()
    {
        PingInternet();
    }

    void PingInternet()
    {
        if (isRequesting || Time.time < 5.5f) return;
        StartCoroutine(IEPingInternet());
    }

    IEnumerator IEPingInternet()
    {
        isRequesting = true;
        string url = @"https://www.google.com.vn/";
        UnityWebRequest request = new UnityWebRequest(url);
        var op = request.SendWebRequest();
        yield return new WaitForSeconds(timeDelay);
        yield return new WaitUntil(() => op.isDone);
        isRequesting = false;
        popup.gameObject.SetActive(request.result == UnityWebRequest.Result.ConnectionError);
    }
}
