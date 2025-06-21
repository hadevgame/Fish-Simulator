using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = false;
        Owner.MoveToNextWaypoint();
        Owner.animationCustomer.SetMovement(AnimationCustomer.MovementType.Walking);
    }

    public override void FrameUpdate()
    {
        if (Owner.isMoving && Owner.agent.remainingDistance < 0.5f && !Owner.agent.pathPending)
        {
            CustomerManager.OnReleaseCustomerEvent?.Invoke(Owner.gameObject);
        }
    }
}
