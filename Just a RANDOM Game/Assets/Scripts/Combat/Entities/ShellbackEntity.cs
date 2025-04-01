using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShellbackEntity : Entity
{
    private NavMeshAgent agent;
    private Transform player;

    protected override void Start()
    {
        base.Start();
        player = PlayerController.instance.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public override void Kill()
    {
        Destroy(gameObject);
    }
}
