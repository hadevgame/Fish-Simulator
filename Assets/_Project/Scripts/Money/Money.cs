using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public bool isCoin;
    public float value; // Mệnh giá của tờ tiền
    public MeshFilter moneyMesh; // Renderer để gán material
    public void SetMoney(float amount, Mesh mesh)
    {
        value = amount;
        if (moneyMesh != null && mesh != null)
        {
            moneyMesh.mesh = mesh; 
        }
    }
}
