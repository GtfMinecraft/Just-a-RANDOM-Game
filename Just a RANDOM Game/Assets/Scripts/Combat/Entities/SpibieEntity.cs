using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpibieEntity : HostileEntity
{
    [Header("Animation")]
    public float walkAnimThreshold = 1f;

    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim.GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if (agent.velocity.magnitude >= walkAnimThreshold)
        {
            //anim.SetBool("walk", true);
            //walk ground dust vfx
            //walk sound
        }
        else
        {
            //anim.SetBool("walk", false);
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= 10)
        {
            //anim.SetBool("pounce", true);
            //pounce sound
            //calculate damage to player
        }
        else
        {
            //anim.SetBool("pounce", false);
        }
    }
}
