using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = true;
        Owner.animationCustomer.SetMovement(AnimationCustomer.MovementType.Idle);
    }
}
