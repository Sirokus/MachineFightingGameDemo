using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementState : State
{
    public Entity entity;
    public PlayerMovement movement;
    public new MovementStateMachine stateMachine;

    public MovementState(Entity entity, PlayerMovement movement, MovementStateMachine stateMachine) : base(stateMachine)
    {
        this.entity = entity;
        this.movement = movement;
        this.stateMachine = stateMachine;
    }
}
