using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerJumpState : PlayerState
{
    private CharacterController controller;
    private float speed;
    private float airSpeedMultipler;

    private float jumpHeight;

    private float jumpReadyTime;
    private float jumpReadyTimer;

    public PlayerJumpState(Player player, PlayerMovement movement, MovementStateMachine stateMachine) : base(player, movement, stateMachine)
    {
        controller = movement.characterController;
        speed = movement.speed;
        jumpHeight = movement.jumpHeight;
        jumpReadyTime = movement.jumpReadyTime;
    }

    public override void Enter()
    {
        base.Enter();

        jumpReadyTimer = jumpReadyTime;
    }

    public override void Update()
    {
        base.Update();

        jumpReadyTimer -= Time.deltaTime;

        if(jumpReadyTimer < 0 )
        {
            movement.velocity.y = Mathf.Sqrt(jumpHeight * -2f * movement.gravity) + movement.gravity * Time.deltaTime;

            controller.Move(movement.velocity * Time.deltaTime);

            stateMachine.ChangeState(PlayerStateMachine.EState.Falling);
        }
    }
}
