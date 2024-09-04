using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public int flag = -1;

    public PlayerMovement movement { get; private set; }
    public Vector3 velocity => movement.velocity;

    //Move Action
    public UnityAction<Vector2> onMove;
    public UnityAction onJump;

    //Fire Action
    public UnityAction onFireStart, onFireEnd;
    public UnityAction onReload;
    public bool firing;

    //Select Action
    public UnityAction onAllSelect;
    public UnityAction<int> onSelect;
    

    protected virtual void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    
}
