using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState<T>
{
    protected StateMachine<T> stateMachine { get; private set; }

    public T Owner => stateMachine.Owner;

    public void Init(StateMachine<T> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void OnCreated()
    {
    }

    public virtual void Enter()
    {
    }

    public virtual void HandleLogic()
    {

    }

    public virtual void FrameUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {
    }
}
