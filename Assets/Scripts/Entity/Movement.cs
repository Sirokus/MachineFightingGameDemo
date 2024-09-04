#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class Movement : MonoBehaviour
{
    public CharacterController characterController;

    [Header("Movement")]
    public float speed = 12f;
    public float gravity = -10f;
    public float jumpHeight = 2f;
    public float jumpReadyTime = 0.5f;
    public float airSpeedMultipler = 0.5f;
    public float turnSpeed = 4;

    [Header("Collision")]
    public Transform groundCheck;
    public Vector3 groundCheckRange;
    public LayerMask groundMask;

    //self state
    [HideInInspector] public Vector3 velocity;
    private Vector3 lastPosition;
    public bool isGrounded { get; private set; }

    //“Ù–ß
    [SerializeField] private AudioClip[] footStepClips;

    //State Machine
    public MovementStateMachine stateMachine { get; private set; } = new MovementStateMachine();

    protected virtual void Awake()
    {
        lastPosition = transform.position;
        characterController = GetComponent<CharacterController>();
    }

    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        stateMachine.Update();
        HandleGravity();
    }

    void HandleGravity()
    {
        isGrounded = Physics.CheckBox(groundCheck.position, groundCheckRange, Quaternion.identity, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        velocity.x = transform.position.x - lastPosition.x;
        velocity.z = transform.position.z - lastPosition.z;

        lastPosition = transform.position;
    }

    public void PlayFootSound()
    {
        AudioManager.PlayClipAtPoint(footStepClips[Random.Range(0, footStepClips.Length - 1)], transform.position, 1);
    }
}
