using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    [SerializeField]
    private Transform respawnLocation;

    public PlayerEntity(
        GameObject playerModel,
        float playerSpeed = 10f,
        float playerDamage = 20f,
        float playerMaxHealth = 100f,
        float playerMaxShield = 0f,
        float playerArmor = 0f
    )
        : base(
            "Player",
            playerModel,
            playerSpeed,
            playerDamage,
            playerMaxHealth,
            playerMaxShield,
            playerArmor
        )
    {
        base.respawnPoint = respawnPoint;
        base.respawnCooldown = float.PositiveInfinity;
    }

    public override void TakeDamage(Damage instance)
    {
        ReduceTotalHealth(ApplyArmorReduction(instance).value);

        if (health < 0f)
        {
            Debug.Log($"You were killed by {instance.source.name}"); // TODO: better death message
            Kill();
        }

        // TODO: play OnDamage animation
    }

    public override void Kill()
    {
        // TODO: death animation, death screen

        base.respawnPoint.SpawnEntity();
        GameObject.Destroy(base.model);
    }
}
