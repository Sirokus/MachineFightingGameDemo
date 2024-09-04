using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateMachine : StateMachine
{
    public enum EState
    {
        Idle,
        Move,
        Jump,
        Falling
    }

    private Dictionary<EState, MovementState> states = new Dictionary<EState, MovementState>();

    public virtual void Init(EState type, MovementState state)
    {
        AddState(type, state);

        base.Init(state);
    }

    public override void Update()
    {
        base.Update();
    }

    public virtual void ChangeState(EState type)
    {
        base.ChangeState(GetState(type));
    }

    public State GetState(EState type)
    {
        return states[type];
    }

    public void AddState(EState type, MovementState state)
    {
        if(!states.ContainsKey(type))
        {
            states.Add(type, state);
        }
    }
}
