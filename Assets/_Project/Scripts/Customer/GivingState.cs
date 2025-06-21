using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        if(Owner == null)
            Debug.LogError("owner null.");
        if (Owner.animationCustomer == null)
        {
            Debug.LogError("animationCustomer is not assigned.");
        }
        Owner.animationCustomer.SetAction(AnimationCustomer.ActionType.Giving);
        if (Owner.order != null)
        {
            if (Owner.order.isCard)
                Owner.cardObj.SetActive(true);
            else
                Owner.moneyObj.SetActive(true);
        }
    }

    public override void FrameUpdate()
    {
        if (CheckOutController.Ins == null || !CheckOutController.Ins.IsActive)
        {
            Owner.animationCustomer.Deaction(AnimationCustomer.ActionType.Giving);
            Owner.moneyObj.SetActive(false);
            Owner.cardObj.SetActive(false);
            Owner.ChangeState<CashingState>();
        }
        else 
        {
            //Owner.ChangeState<GivingState>();
            Owner.HandleTouchMoney();
        }
            
    }
}
