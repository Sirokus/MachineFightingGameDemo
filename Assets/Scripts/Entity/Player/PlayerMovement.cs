#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class PlayerMovement : Movement
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
        stateMachine.Init(MovementStateMachine.EState.Idle, new PlayerIdleState(player, this, stateMachine));
        stateMachine.AddState(MovementStateMachine.EState.Move, new PlayerMoveState(player, this, stateMachine));
        stateMachine.AddState(MovementStateMachine.EState.Jump, new PlayerJumpState(player, this, stateMachine));
        stateMachine.AddState(MovementStateMachine.EState.Falling, new PlayerFallingState(player, this, stateMachine));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        HandleRotate();
    }

    void HandleRotate()
    {
        Vector2 input = PlayerController.Ins.move.ReadValue<Vector2>();

        Vector3 move = PlayerController.GetControllerTransform().right * input.x + PlayerController.GetControllerTransform().forward * input.y;
        move.y = 0;
        move.Normalize();

        if (move.magnitude > 0.1)
        {
            Quaternion q = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, turnSpeed * Time.deltaTime);
        }
    }
}
