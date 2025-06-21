using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingState : BaseState<CustomerStateMachine>
{
    private float waitTimer = 0f;
    public override void Enter()
    {
        Owner.agent.isStopped = true;
        
        Owner.animationCustomer.SetBool("CarryingIdle", true);
        Owner.waitingTime = 0f;
        waitTimer = 0f;
    }

    public override void FrameUpdate()
    {
        Owner.tank.gameObject.SetActive(true);
        if (Owner.isOrderCompleted) return;

        if (!Owner.tank.isHasFish && Owner.interactTank.IsHasFish())
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= 2f)
            {
                GameObject fish = Owner.interactTank.GetFish();
                if (fish != null)
                {
                    Owner.tank.AddFish(fish);
                    Owner.collectedFish++;

                    if (Owner.order.IsOrderComplete(Owner.collectedFish))
                    {
                        Owner.isOrderCompleted = true;
                        waitTimer = 0;
                        Owner.StartCoroutine(DelayBeforeMovingToCash());
                    }
                    else
                    {
                        Owner.StartCoroutine(WaitForMoreFish());
                    }
                }
            }
        }
    }



    private IEnumerator DelayBeforeMovingToCash()
    {
        yield return new WaitForSeconds(1.5f);
        CashDesk.OnCustomerReadyToQueue?.Invoke(Owner);
        Owner.ChangeState<GoToCashingState>();
        Owner.StopAllCoroutines();
    }

    private IEnumerator WaitForMoreFish()
    {
        yield return new WaitForSeconds(Owner.maxWaitTime);
        waitTimer = 0;
        Owner.StartCoroutine(DelayBeforeMovingToCash());
    }
}
