using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GUISubPathAttribute : Attribute
{
    public string Path { get; }
 
    public GUISubPathAttribute(string path) => Path = path;
}
