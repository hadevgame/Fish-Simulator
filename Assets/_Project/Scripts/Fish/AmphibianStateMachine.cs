using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmphibianStateMachine : MonoBehaviour
{
    private StateMachine<AmphibianStateMachine> stateMachine;
    public Transform waterTransform;
    public Transform groundTransform;
    public Animator animator;

    private BoxCollider waterCollider;
    private BoxCollider groundCollider;

    private Transform waterPoint;
    private Transform groundPoint;

    public float swimSpeed;
    public float walkSpeed;
    public float padding;

    private bool isActive = false;
    private void Update()
    {
        if (isActive && stateMachine != null)
        {
            stateMachine.FrameUpdate();
        }
    }

    public void BeginState()
    {
        stateMachine = new StateMachine<AmphibianStateMachine>(this, 20);
        stateMachine.ChangeState<SwimState>();
        isActive = true;
    }

    public void StopState() 
    {
        isActive = false;
    }
    public void MoveToTransitionPoint(Vector3 targetPos, System.Action onArrive)
    {
        StartCoroutine(MoveToPointCoroutine(targetPos, onArrive));
    }

    private IEnumerator MoveToPointCoroutine(Vector3 targetLocalPos, System.Action onArrive)
    {
        float speed = 0.3f;

        Vector3 direction = (targetLocalPos - transform.localPosition).normalized;
        if (direction != Vector3.zero)
        {
            //Quaternion targetRot = Quaternion.LookRotation(transform.parent.TransformDirection(direction));
            Quaternion targetRot = Quaternion.LookRotation(direction);
            while (Quaternion.Angle(transform.rotation, targetRot) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
                yield return null;
            }
        }

        while (Vector3.Distance(transform.localPosition, targetLocalPos) > 0.05f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPos, speed * Time.deltaTime);
            yield return null;
        }

        //transform.localPosition = targetLocalPos; 
        onArrive?.Invoke();
    }

    public void SetParentToWater()
    {
        transform.SetParent(waterTransform);
    }

    public void SetParentToGround()
    {
        transform.SetParent(groundTransform);
    }

    public void SetWaterNGround(Transform water, Transform ground , Transform waterpos, Transform groundpos) 
    {
        waterTransform = water;
        groundTransform = ground;
        waterPoint = waterpos;
        groundPoint = groundpos;
        waterCollider = waterTransform.GetComponent<BoxCollider>();
        groundCollider = groundTransform.GetComponent<BoxCollider>();
    }
    public void SetUp(float speed, float tankpadding)
    {
        swimSpeed = speed;
        padding = tankpadding;
    }
    public Transform GetWaterPos() 
    {
        return waterPoint;
    }
    public Transform GetGroundPos()
    {
        return groundPoint;
    }
    public BoxCollider GetWater() 
    {
        if (waterCollider != null) 
        {
            return waterCollider;
        }
        else return null;
    }
    public BoxCollider GetGround()
    {
        if (groundCollider != null)
        {
            return groundCollider;
        }
        else return null;
    }
    public void ChangeState<T>() where T : BaseState<AmphibianStateMachine>, new()
    {
        stateMachine.ChangeState<T>();
    }
}
