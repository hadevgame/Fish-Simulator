using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class SheetReader
{
    public static void ReadFromSheet(string apiKey, string sheetID, string sheetName, Action<SheetData> callback)
    {
        string link = GetLink(apiKey, sheetID, sheetName);
        Debug.Log("Request data from: " + link);
        var req = UnityWebRequest.Get(link);
        var op = req.SendWebRequest();
        op.completed += (aop) =>
        {
            SheetData sheetData = JsonConvert.DeserializeObject<SheetData>(req.downloadHandler.text);
            foreach (var item in sheetData.values)
            {
                if (item.Count > 1)
                {
                    UCLogger.Log($"[Sheet value] {item[0]}: {item[1]}", Color.yellow);
                }
            }
            Debug.Log("Requested data from: " + link);
            callback?.Invoke(sheetData);
        };
    }

    public static string GetLink(string apiKey, string sheetID, string sheetName)
    {
        return $@"https://sheets.googleapis.com/v4/spreadsheets/{sheetID}/values/{sheetName}?key={apiKey}";
    }

    public static void OpenLink(string apiKey, string sheetID, string sheetName)
    {
        Application.OpenURL(GetLink(apiKey, sheetID, sheetName));

    }

}
