using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    public GameObject player;
    public void Call()
    {
        GetComponent<NavMeshAgent>().destination = player.transform.position;
    }

    private void Update()
    {
        //roam around
    }
}