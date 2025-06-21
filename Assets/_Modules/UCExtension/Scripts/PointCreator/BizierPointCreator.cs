using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class BizierPointCreator : BasePointCreator
{
    [SerializeField] private Transform[] controlPoints;

    [Range(3, 200)]
    [SerializeField] int numOfPoint = 5;

    public override List<Vector3> GetPath()
    {
        List<Vector3> paths = new List<Vector3>();
        for (float t = 0; t <= 1; t += 1f / (numOfPoint - 1))
        {
            var position = Mathf.Pow(1 - t, 3) * controlPoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position + Mathf.Pow(t, 3) * controlPoints[3].position;
            paths.Add(position);
        }
        return paths;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        foreach (var item in GetPath())
        {
            Gizmos.DrawSphere(item, gizmosSize);
        }
        Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
        Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
    }
}
