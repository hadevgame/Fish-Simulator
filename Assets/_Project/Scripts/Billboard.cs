using UnityEngine;

public class Billboard : MonoBehaviour
{
    public static System.Action<Billboard> OnAddEvent = null;

    private void OnEnable()
    {
        BillboardManager.Ins.Add(this);
    }
    public void LookAtCamera(Camera mainCamera)
    {
        if (mainCamera == null) return;

        transform.rotation = mainCamera.transform.rotation;
    }

    private void OnDisable()
    {
        BillboardManager.Ins?.Remove(this);
    }
   
}