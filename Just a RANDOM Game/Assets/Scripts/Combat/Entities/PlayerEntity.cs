using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);

        // TODO: play OnDamage animation, belt
    }

    public override void Kill()
    {
        // TODO: death animation, death screen

        transform.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        //wake up anim
    }

    public override void OnSpawn()
    {
        // TODO: play wake up cutscene
    }
}
