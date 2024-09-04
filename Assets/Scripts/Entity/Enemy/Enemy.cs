using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : Entity
{
    public NavMeshAgent agent;

    protected override void Awake()
    {
        base.Awake();

        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Update()
    {
        base.Update();

        agent.SetDestination(PlayerController.Ins.player.transform.position);
    }
}
