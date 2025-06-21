using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Identificator
{
    public string UniqueID;

    [Button]
    public void ResetID()
    {
        UniqueID = Guid.NewGuid().ToString("N");
    }
}
