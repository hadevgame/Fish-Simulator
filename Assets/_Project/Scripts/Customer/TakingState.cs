using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakingState : BaseState<CustomerStateMachine>
{
    public override void Enter()
    {
        Owner.animator.SetTrigger("Taking");
        Owner.StartCoroutine(DelayedTaking());
    }

    private IEnumerator DelayedTaking()
    {
        yield return new WaitForSeconds(1.5f);
        Owner.SetStartPos(CustomerManager.OnExitShop?.Invoke());
        Owner.DeCash();
    }
}

