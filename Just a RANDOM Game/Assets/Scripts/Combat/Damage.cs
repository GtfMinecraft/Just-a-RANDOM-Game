using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an instance of damage dealt to any entity
// all instances of damage have a source entity
// some instances of damage can inflict status effects
public class Damage
{
    public readonly Entity source; // the source entity of this damage instance
    public readonly Item item; // the item used to inflict this instance of damage (optional)
    public float value; // the actual value of damage
    public SortedSet<StatusEffect> effects = new SortedSet<StatusEffect>(); // the status effects this will inflict

    public Damage(Entity sourceEntity, float damageValue)
    {
        source = sourceEntity;
        value = damageValue;
    }

    public Damage(Entity sourceEntity, float damageValue, StatusEffect statusEffect)
    {
        source = sourceEntity;
        value = damageValue;
        effects.Add(statusEffect);
    }

    public Damage(Entity sourceEntity, float damageValue, SortedSet<StatusEffect> statusEffects)
    {
        source = sourceEntity;
        value = damageValue;
        effects = statusEffects;
    }
}
