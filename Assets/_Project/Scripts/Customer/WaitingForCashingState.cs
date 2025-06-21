using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForCashingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.tank.gameObject.SetActive(true);
        Owner.transform.rotation = Quaternion.Euler(0, -70, 0);
        Owner.agent.isStopped = true;
        Owner.animationCustomer.SetBool("CarryingIdle", true);
        Owner.waitingTime = 0f;
    }

    public override void FrameUpdate()
    {
        if (CashDesk.OnCheckFirstCustomer?.Invoke(Owner) == true)
        {
            CashDesk.Instance.Cash(Owner.tank);
            Owner.ChangeState<CashingState>();
        }
    }
}
