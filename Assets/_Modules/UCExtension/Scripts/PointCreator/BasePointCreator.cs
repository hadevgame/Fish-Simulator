using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public abstract class BasePointCreator : MonoBehaviour
{
    [SerializeField] Transform pointParents;

    [SerializeField] bool reversePath;

    [SerializeField] string pointTag;

    [SerializeField] protected bool drawGizmos;

    [Range(0f, 1)]
    [ShowIf("drawGizmos")]
    [SerializeField] protected float gizmosSize = 0.1f;

    public abstract List<Vector3> GetPath();


#if UNITY_EDITOR

    public void CreatePoint(Vector3 position)
    {
        GameObject newObj = new GameObject($"Point {pointParents.childCount}");
        if (!string.IsNullOrEmpty(pointTag))
        {
            newObj.tag = pointTag;
        }
        newObj.transform.SetParent(pointParents);
        newObj.transform.position = position;
        newObj.transform.SetDirtyAndSave();
        var gizmos = newObj.gameObject.AddComponent<GizmosDrawR>();
        gizmos.gizmosColor = Color.yellow;
        gizmos.size = gizmosSize;
    }
    [Button]
    public void ClearPoints()
    {
        while (pointParents.childCount > 0)
        {
            DestroyImmediate(pointParents.GetChild(0).gameObject);
        }
    }

    [Button]
    void CreatePoints()
    {
        ClearPoints();
        var path = GetPath();
        if (reversePath)
        {
            path.Reverse();
        }
        foreach (var item in path)
        {
            CreatePoint(item);
        }
    }
#endif
}
