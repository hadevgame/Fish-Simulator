using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject itemInside; 
    private bool isPlacing = false;

    private void Start()
    {
        if (itemInside != null)
        {
            itemInside.SetActive(false); 
        }
    }
    public void SetItemInside(GameObject itemPrefab)
    {
        if (itemInside == null)
        {
            itemInside = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            itemInside.transform.SetParent(transform);
            itemInside.SetActive(false); 
        }
    }

    public GameObject GetItemInside() 
    {
        return itemInside;
    }
    public void StartPlacing()
    {
        isPlacing = true;
        if (itemInside != null)
        {
            itemInside.SetActive(true); 
            itemInside.transform.SetParent(null);
        }
        gameObject.SetActive(false);
        PurchaseManager.OnRemoveFromList?.Invoke(gameObject);
    }

    public void CancelPlacing()
    {
        isPlacing = false;
        if (itemInside != null)
        {
            itemInside.transform.SetParent(transform); 
            itemInside.SetActive(false); 
        }
        gameObject.SetActive(true);
        PurchaseManager.OnAddToList?.Invoke(gameObject);
    }
}
