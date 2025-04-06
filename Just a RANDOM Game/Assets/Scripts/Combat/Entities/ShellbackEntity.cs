using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShellbackEntity : HostileEntity
{
    [Header("Animation")]
    public float walkAnimSpeed = 1f;

    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = entityObj.GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if(agent.hasPath)
        {
            Vector3 ratio = agent.transform.InverseTransformDirection(agent.velocity).normalized;
            anim.SetBool("Walk", true);
            anim.SetFloat("Speed",  Mathf.Max(ratio.x, ratio.z) * walkAnimSpeed);
            //walk ground dust vfx
            //walk sound
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= 1)
        {
            anim.SetBool("Charge", true);
            //pounce sound
            //calculate damage to player
        }
        else
        {
            anim.SetBool("Charge", false);
        }
    }

    public override void Kill()
    {
        Destroy(gameObject);
    }
}
