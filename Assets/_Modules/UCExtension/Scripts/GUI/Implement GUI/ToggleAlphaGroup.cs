using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAlphaGroup : MonoBehaviour
{
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.U) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            canvasGroup.alpha = 0;
        }
        if (Input.GetKey(KeyCode.U) && Input.GetKeyDown(KeyCode.Alpha2))
        {
            canvasGroup.alpha = 1;
        }
    }
}
