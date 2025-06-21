using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class WalkState : BaseState<AmphibianStateMachine>
{
    private float stateDuration;
    private float stateTimer;

    private float dirChangeTimer;
    private float dirChangeInterval;
    private Vector3 direction;

    private bool hasStarted = false;

    public override void Enter()
    {
        hasStarted = false;

        if (Owner.GetGroundPos() != null)
        {
            Owner.SetParentToGround();
            Owner.MoveToTransitionPoint(Owner.GetGroundPos().localPosition, () =>
            {
                StartWalking(); 
            });
        }
        else
        {
            Owner.SetParentToGround();
            StartWalking();
        }
    }

    void StartWalking()
    {
        stateDuration = Random.Range(15f, 40f); 
        stateTimer = 0f;

        SetNewDirection();
        dirChangeTimer = 0f;

        Owner.animator.SetBool("Walk", true);

        hasStarted = true;
    }

    public override void FrameUpdate()
    {
        if (!hasStarted) return;

        stateTimer += Time.deltaTime;
        dirChangeTimer += Time.deltaTime;

        if (dirChangeTimer >= dirChangeInterval)
        {
            SetNewDirection();
            dirChangeTimer = 0f;
        }

        WalkMovement();

        if (stateTimer >= stateDuration)
        {
            Owner.ChangeState<SwimState>();
        }
    }

    void SetNewDirection()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        dirChangeInterval = Random.Range(3f, 5f); // Khoảng cách thời gian để đổi hướng tiếp theo
    }

    //void WalkMovement()
    //{
    //    var localPos = Owner.transform.localPosition;
    //    localPos += direction * Owner.walkSpeed * Time.deltaTime;
    //    localPos = Clamp(localPos, Owner.GetGround());
    //    Owner.transform.localPosition = localPos;

    //    if (direction != Vector3.zero)
    //    {
    //        Quaternion rot = Quaternion.LookRotation(Owner.transform.parent.TransformDirection(direction));
    //        Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, rot, Time.deltaTime * 2f);
    //    }
    //}
    void WalkMovement()
    {
        var localPos = Owner.transform.localPosition;
        localPos += direction * Owner.walkSpeed * Time.deltaTime;
        localPos = Clamp(localPos, Owner.GetGround());
        Owner.transform.localPosition = localPos;

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction); 
            Owner.transform.localRotation = Quaternion.Slerp(Owner.transform.localRotation, rot, Time.deltaTime * 2f);
        }
    }

    Vector3 Clamp(Vector3 pos, BoxCollider col)
    {
        Vector3 center = col.center;
        Vector3 size = col.size;
        Vector3 min = center - size * 0.5f;
        Vector3 max = center + size * 0.5f;

        pos.x = Mathf.Clamp(pos.x, min.x + Owner.padding, max.x - Owner.padding);
        pos.y = Mathf.Clamp(pos.y, min.y + Owner.padding, max.y - Owner.padding);
        pos.z = Mathf.Clamp(pos.z, min.z + Owner.padding, max.z - Owner.padding);
        return pos;
    }
    public override void Exit()
    {
        Owner.animator.SetBool("isWalking", false); 
    }

}
