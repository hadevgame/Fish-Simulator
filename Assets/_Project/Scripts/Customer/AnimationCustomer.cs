using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCustomer : MonoBehaviour
{
    private Animator animator = null;

    private MovementType type;
    private ActionType action;

    public AnimationCustomer(Animator animator)
    {
        //this.animator = animator;
        type = MovementType.None;
        action = ActionType.None;
    }
    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }
    public void SetMovement(MovementType mType)
    {
        if (type == mType)
            return;

        SetBool(mType.ToString(), true);
        SetBool(type.ToString(), false);
        type = mType;
    }

    public void SetAction(ActionType aType)
    {
        if (action == aType)
            return;

        SetBool(aType.ToString(), true);
        SetBool(action.ToString(), false);
        action = aType;
    }
    public void Deaction(ActionType aType)
    {
        if (action != aType)
            return;
        SetBool(aType.ToString(), false);
        action = ActionType.None;
    }
    public void SetBool(string parameter, bool value)
    {
        if (String.IsNullOrEmpty(parameter))
            return;

        animator.SetBool(parameter, value);
    }
    public enum ActionType
    {
        None,
        Carrying,
        Cashing,
        Giving,
        Taking
    }
    public enum MovementType
    {
        None,
        Idle,
        Walking,
    }
}
