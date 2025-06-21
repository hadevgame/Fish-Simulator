using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForProductState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = true;
        Owner.animationCustomer.SetMovement(AnimationCustomer.MovementType.Idle);
        Owner.waitingTime = 0f;
        Owner.Emo.gameObject.SetActive(true);
    }

    public override void FrameUpdate()
    {
        Owner.waitingTime += Time.deltaTime;

        if (Owner.IsProductAvailable())
        {
            Owner.ChangeState<BuyingState>();
            Owner.Emo.gameObject.SetActive(false);
        }
        else if (Owner.waitingTime >= Owner.maxWaitTime)
        {
            Owner.Emo.gameObject.SetActive(false);
            Owner.SetStartPos(CustomerManager.OnExitShop?.Invoke());
            Owner.waitingTime = 0;
            Owner.SetStateSlot();
            Owner.ChangeState<LeavingState>();
        }
    }
}
