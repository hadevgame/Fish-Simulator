using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateMachine<T>
{
    public string CurrentStateName;

    Dictionary<string, BaseState<T>> states = new Dictionary<string, BaseState<T>>();

    public BaseState<T> CurrentState { get; private set; }

    public BaseState<T> PrevState { get; private set; }

    public T Owner { get; private set; }

    int handleLogicInterval;

    int frameCount;

    public StateMachine(T owner, int handleLogicInterval)
    {
        this.Owner = owner;
        PrevState = new BaseState<T>();
        CurrentState = new BaseState<T>();
        CurrentState.Enter();
        this.handleLogicInterval = handleLogicInterval;
    }

    public void ChangeState<S>() where S : BaseState<T>
    {
        CurrentStateName = typeof(S).ToString();
        CurrentState.Exit();
        var state = GetState<S>();
        PrevState = CurrentState;
        CurrentState = state;
        state.Enter();
    }

    public void BackToPrevState()
    {
        CurrentState.Exit();
        PrevState.Enter();
        CurrentState = PrevState;
    }

    public BaseState<T> GetState<S>() where S : BaseState<T>
    {
        string id = typeof(S).ToString();
        if (!states.ContainsKey(id))
        {
            var newInstance = Activator.CreateInstance<S>();
            states[id] = newInstance;
            newInstance.Init(this);
            newInstance.OnCreated();
        }
        return states[id];
    }

    public void FrameUpdate()
    {
        CurrentState.FrameUpdate();
        if (frameCount >= handleLogicInterval)
        {
            CurrentState.HandleLogic();
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }
    }
}
