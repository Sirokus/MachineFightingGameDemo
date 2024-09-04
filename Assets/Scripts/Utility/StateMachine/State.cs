using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class State
{
    public StateMachine stateMachine;

    public UnityAction onStateEnter;
    public UnityAction onStateExit;
    public UnityAction<bool> onStateChanged;    //½øÈëtrue£¬Àë¿ªfalse

    public State(StateMachine stateMachine)
    {
        this.stateMachine=stateMachine;
    }

    public virtual void Enter()
    {
        onStateEnter?.Invoke();
        onStateChanged?.Invoke(true);
    }

    public virtual void Exit()
    {
        onStateExit?.Invoke();
        onStateChanged?.Invoke(false);
    }

    public virtual void Update()
    {
    }
}
