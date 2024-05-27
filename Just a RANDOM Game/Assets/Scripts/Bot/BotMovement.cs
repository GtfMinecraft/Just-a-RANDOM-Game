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
        //chance to: roam around / gone when out of bound of unloaded / summon back if gone according to chunks unlocked
        //check spawn chunk is loaded or not when moving to spawn chunk to store item
    }

    //if the scene of the chunk bot's in is about to unload, delete bot and let update summon bot
}