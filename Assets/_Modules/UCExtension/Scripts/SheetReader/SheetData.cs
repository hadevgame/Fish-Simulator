using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetData
{
    public string range;

    public string majorDimension;

    public List<List<string>> values;

    public string GetValue(string key)
    {
        foreach (var item in values)
        {
            if (item.Count >= 2)
            {
                if (item[0].Trim().Equals(key))
                {
                    return item[1].Trim();
                }
            }
        }
        return "";
    }

    public List<string> GetValues(string key)
    {
        var result = new List<string>();
        foreach (var item in values)
        {
            if (item.Count > 2)
            {
                if (item[0].Trim().Equals(key))
                {
                    for (int i = 1; i < item.Count; i++)
                    {
                        result.Add(item[i].Trim());
                    }
                }
            }
        }
        return result;
    }
}