using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMoveState : PlayerGroundState
{
    private CharacterController controller;
    private float speed;

    public PlayerMoveState(Player player, PlayerMovement movement, MovementStateMachine stateMachine) : base(player, movement, stateMachine)
    {
        this.controller = movement.characterController;
        this.speed = movement.speed;
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

    private void OnMove(Vector2 input)
    {
        Vector3 move = PlayerController.GetControllerTransform().right * input.x + PlayerController.GetControllerTransform().forward * input.y;
        move.y = 0;
        move.Normalize();

        float speedMultipler = (Vector3.Dot(new Vector3(move.x, 0, move.z), movement.transform.forward) + 1) / 2;

        controller.Move(move * speed * speedMultipler * Time.deltaTime);

        if (input.magnitude < 0.01)
            stateMachine.ChangeState(PlayerStateMachine.EState.Idle);
    }
}
