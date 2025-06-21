using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrameDelayer
{
    [SerializeField] int delayNumber;

    int count;

    public bool Check()
    {
        if (count >= delayNumber)
        {
            count = 0;
            return true;
        }
        count++;
        return false;
    }
}
