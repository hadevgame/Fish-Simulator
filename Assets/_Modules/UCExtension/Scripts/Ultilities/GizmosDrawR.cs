using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosDrawR : MonoBehaviour
{
    public float size = 1f;

    public float selectedSize = 1f;

    public Color gizmosColor = Color.white;

    public Color selectedGizmosColor = Color.white;

    public bool useWireSphere;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = selectedGizmosColor;
        DrawSphere(selectedSize);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        DrawSphere(size);
    }

    void DrawSphere(float size)
    {
        if (useWireSphere)
        {
            Gizmos.DrawWireSphere(transform.position, size);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, size);
        }
    }
}
