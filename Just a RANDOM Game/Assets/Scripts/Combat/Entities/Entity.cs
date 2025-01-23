using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an entity is an object used for combat purposes
// entities have health and can be inflicted damage and status effects
abstract public class Entity
{
    public GameObject model; // the GameObject that owns this entity
    public float maxHealth; // max health of this entity
    public float health; // how much health does this entity have?
    public float maxShield; // max shield of thie entity
    public float shield; // shield regenerates after 5 seconds of not taking damage, over 2 seconds (shield gets depleted before health)
    public float speed; // the base (100%) speed of this entity
    public float baseDamage; // the base (100%) damage of this entity
    public float armor; // incoming damage is multiplied by 1 - armor / (100 + abs(armor)), this increases / decreases the damage value by a maximum of 100%
    public SortedSet<StatusEffect> activeEffects = new SortedSet<StatusEffect>(); // the active status effects of this entity
    public string name; // name of this entity

    public Entity(
        string entityName,
        GameObject entityModel,
        float entitySpeed,
        float entityDamage,
        float entityMaxHealth,
        float entityMaxShield = 0,
        float entityArmor = 0
    )
    {
        model = entityModel;
        maxHealth = health = entityMaxHealth;
        maxShield = shield = entityMaxShield;
        speed = entitySpeed;
        baseDamage = entityDamage;
        armor = entityArmor;
        name = entityName;
    }

    public virtual void TakeDamage(Damage instance)
    {
        // reduce health by incoming damage
        ReduceTotalHealth(ApplyArmorReduction(instance).value);

        // apply status effects
        foreach (StatusEffect effect in instance.effects)
            activeEffects.Add(effect);

        if (health < 0f)
            Kill();

        // TODO: play OnDamage animation
    }

    public virtual void Kill()
    {
        GameObject.Destroy(model);
    }

    public virtual void ApplyStatusEffect(StatusEffect instance)
    {
        foreach (StatusEffect effect in activeEffects)
        {
            if (effect.GetType() == instance.GetType())
            {
                effect.source = instance.source;
                effect.stack++;
                return;
            }
        }

        // if the same type of status effect isnt found
        activeEffects.Add(instance);
    }

    protected Damage ApplyArmorReduction(Damage instance)
    {
        return new Damage(
            instance.source,
            instance.value * (1 - armor / (100 + Mathf.Abs(armor))),
            instance.effects
        );
    }

    protected void ReduceTotalHealth(float damageValue)
    {
        if (shield > damageValue) // shield gets depleted first
            shield -= damageValue;
        else // if shield is not enough to absorb damage, start depleting health
        {
            health -= damageValue - shield;
            shield = 0;
        }
    }
}
