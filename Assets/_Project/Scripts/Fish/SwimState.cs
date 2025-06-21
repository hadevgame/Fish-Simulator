using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimState : BaseState<AmphibianStateMachine>
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
        if (Owner.GetWaterPos() != null)
        {
            Owner.SetParentToWater();
            Owner.MoveToTransitionPoint(Owner.GetWaterPos().localPosition, () =>
            {
                StartSwimming();
            });
        }
        else
        {
            Owner.SetParentToWater();
            StartSwimming();
        }
    }

    void StartSwimming()
    {
        stateDuration = Random.Range(8f, 35f); 
        stateTimer = 0f;

        SetNewDirection();
        dirChangeTimer = 0f;

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

        SwimMovement();

        if (stateTimer >= stateDuration)
        {
            Owner.ChangeState<WalkState>();
        }
    }

    void SetNewDirection()
    {
        direction = Random.onUnitSphere;
        dirChangeInterval = Random.Range(2f, 4f);
    }

    void SwimMovement()
    {
        var localPos = Owner.transform.localPosition;
        localPos += direction * Owner.swimSpeed * Time.deltaTime;
        localPos = Clamp(localPos, Owner.GetWater());
        Owner.transform.localPosition = localPos;

        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(Owner.transform.parent.TransformDirection(direction));
            Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, rot, Time.deltaTime * 2f);
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
}
