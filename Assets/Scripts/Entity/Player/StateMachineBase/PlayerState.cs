using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : MovementState
{
    public PlayerState(Entity entity, PlayerMovement movement, MovementStateMachine stateMachine) : base(entity, movement, stateMachine)
    {
    }
}
