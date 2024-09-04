using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachine
{
    public State curState;

    public UnityAction onStateChanged;

    public virtual void Init(State state)
    {
        curState = state;
        curState.Enter();
    }

    public virtual void Update()
    {
        curState.Update();
    }

    public virtual void ChangeState(State newState)
    {
        curState.Exit();
        curState = newState;
        curState.Enter();

        onStateChanged?.Invoke();
    }
}
