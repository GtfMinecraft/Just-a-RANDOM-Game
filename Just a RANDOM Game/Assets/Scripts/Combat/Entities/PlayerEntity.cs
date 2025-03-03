using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public float armor; // one piece (including shield) adds 6 defense

    protected override void Start()
    {
        isSpawn = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(Damage damage)
    {
        damage.value *= (1 - armor / (100 + armor));
        base.TakeDamage(damage);

        // TODO: play OnDamage animation, belt
    }

    public override void Kill()
    {
        // TODO: death animation, death screen

        transform.SetPositionAndRotation(spawnPoints[0].position, spawnPoints[0].rotation);
        //wake up anim
    }

    public override void LoadData(GameData data)
    {
        base.LoadData(data);
        if (data.entityData.ContainsKey(entityName))
            armor = data.entityData[entityName].armor;
    }

    public override void SaveData(GameData data)
    {
        base.SaveData(data);
        data.entityData[entityName].armor = armor;
    }
}
