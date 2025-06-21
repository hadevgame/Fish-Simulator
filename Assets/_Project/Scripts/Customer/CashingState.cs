using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.transform.rotation = Quaternion.Euler(0, -70, 0);
        Owner.agent.isStopped = true;
        Owner.animationCustomer.SetBool("CarryingIdle", false);
        Owner.animationCustomer.Deaction(AnimationCustomer.ActionType.Carrying);
        Owner.animationCustomer.SetMovement(AnimationCustomer.MovementType.Idle);

        //Owner.StartCoroutine(Owner.WaitAndCash());
    }
}
