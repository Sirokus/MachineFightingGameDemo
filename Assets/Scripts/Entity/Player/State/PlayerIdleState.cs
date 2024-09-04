using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerMovement movement, MovementStateMachine stateMachine) : base(player, movement, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.onMove += OnMove;
    }

    public override void Exit()
    {
        base.Exit();

        entity.onMove -= OnMove;
    }

    private void OnMove(Vector2 move)
    {
        if(move.magnitude > 0)
        {
            stateMachine.ChangeState(PlayerStateMachine.EState.Move);
            return;
        }
    }
}
