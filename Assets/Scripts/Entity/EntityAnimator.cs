using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    private Animator animator;
    private Movement movement;

    public float velocityToAnimationMultiper = 2.5f;

    public float minAnimSpeed = 0.5f;
    public float maxAnimSpeed = 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //×´Ì¬±äÁ¿°ó¶¨
        movement.stateMachine.GetState(PlayerStateMachine.EState.Idle).onStateChanged += (enter) => animator.SetBool("IsIdle", enter);
        movement.stateMachine.GetState(PlayerStateMachine.EState.Move).onStateChanged += (enter) => animator.SetBool("IsMove", enter);
        movement.stateMachine.GetState(PlayerStateMachine.EState.Jump).onStateChanged += (enter) => animator.SetBool("IsJump", enter);
        movement.stateMachine.GetState(PlayerStateMachine.EState.Falling).onStateChanged += (enter) => animator.SetBool("IsFalling", enter);
    }

    private void Update()
    {
        Vector2 velocity = new Vector2(movement.velocity.x, movement.velocity.z);
        animator.SetFloat("WalkSpeedMultipler", Mathf.Clamp(velocity.magnitude * velocityToAnimationMultiper, minAnimSpeed, maxAnimSpeed));
    }

    public void PlayFootStepSound()
    {
        movement.PlayFootSound();
    }
}
