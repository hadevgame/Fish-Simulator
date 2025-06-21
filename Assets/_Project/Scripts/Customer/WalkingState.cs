using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = false;
        Owner.MoveToNextWaypoint();
        Owner.animationCustomer.SetMovement(AnimationCustomer.MovementType.Walking);
    }

    public override void FrameUpdate()
    {
        if (Owner.isMoving && Owner.agent.remainingDistance < 0.1f && !Owner.agent.pathPending)
        {
            if (Owner.IsProductAvailable()) 
            {
                Owner.ChangeState<BuyingState>();
            }
            
            else
                Owner.ChangeState<WaitingForProductState>();
        }
    }
}
