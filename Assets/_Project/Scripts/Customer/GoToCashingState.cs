using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToCashingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = false;
        Owner.animationCustomer.SetBool("CarryingIdle", false);
        Owner.animationCustomer.SetAction(AnimationCustomer.ActionType.Carrying);
    }

    public override void FrameUpdate()
    {
        if (Owner.isMoving && Owner.agent.remainingDistance < 0.01f && !Owner.agent.pathPending)
        {
            Owner.ChangeState<WaitingForCashingState>();
        }
    }
}
