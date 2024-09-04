using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(Player player, PlayerMovement movement, MovementStateMachine stateMachine) : base(player, movement, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.onJump += OnJump;
    }

    public override void Exit()
    {
        base.Exit();

        entity.onJump -= OnJump;
    }

    public override void Update()
    {
        base.Update();

        if (!movement.isGrounded)
        {
            stateMachine.ChangeState(PlayerStateMachine.EState.Falling);
        }
    }

    private void OnJump()
    {
        stateMachine.ChangeState(PlayerStateMachine.EState.Jump);
    }
}
