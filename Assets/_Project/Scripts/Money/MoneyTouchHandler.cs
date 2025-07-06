using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoneyTouchHandler : MonoBehaviour, IPointerClickHandler
{
    public CustomerStateMachine customer;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Money touched!");

        if (customer != null)
        {
            this.gameObject.SetActive(false);  

            customer.ChangeState<GivingState>();  
        }
    }
}
