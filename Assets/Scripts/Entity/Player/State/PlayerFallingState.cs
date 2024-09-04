using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerFallingState : PlayerState
{
    private CharacterController controller;
    private float speed;
    private float airSpeedMultipler;

    public PlayerFallingState(Player player, PlayerMovement movement, MovementStateMachine stateMachine) : base(player, movement, stateMachine)
    {
        controller = movement.characterController;
        speed = movement.speed;
        airSpeedMultipler = movement.airSpeedMultipler;
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

    public override void Update()
    {
        base.Update();

        if (movement.isGrounded)
            stateMachine.ChangeState(PlayerStateMachine.EState.Idle);
    }

    private void OnMove(Vector2 input)
    {
        Vector3 move = PlayerController.GetControllerTransform().right * input.x + PlayerController.GetControllerTransform().forward * input.y;
        move.y = 0;
        move.Normalize();

        controller.Move(move * speed * airSpeedMultipler * Time.deltaTime);
    }
}
