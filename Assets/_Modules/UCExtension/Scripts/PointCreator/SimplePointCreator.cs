using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class SimplePointCreator : BasePointCreator
{
    [SerializeField] int numPoint;

    [SerializeField] List<Transform> points;

    public override List<Vector3> GetPath()
    {
        return points.ToVectors().Split(numPoint);
    }
}
