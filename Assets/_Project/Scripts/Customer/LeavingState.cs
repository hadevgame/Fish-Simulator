using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.agent.isStopped = false;
        Owner.MoveToExit();
        //if(Owner.previousState is TakingState)
            //Owner.SpawnBag();
    }

    public override void FrameUpdate()
    {
        if (Owner.isMoving && Owner.agent.remainingDistance < 0.1f && !Owner.agent.pathPending)
        {
            //Owner.ResetBag();
            CustomerManager.OnReleaseCustomerEvent?.Invoke(Owner.gameObject);
        }
    }

    
}
