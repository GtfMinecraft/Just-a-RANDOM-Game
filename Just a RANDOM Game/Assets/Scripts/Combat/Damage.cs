using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an instance of damage dealt to any entity
// all instances of damage have a source entity
// some instances of damage can inflict status effects
public class Damage
{
    public readonly Item item; // the item used to inflict this instance of damage (optional)
    public float value; // the actual value of damage
    public SortedSet<StatusEffect> effects = new SortedSet<StatusEffect>(); // the status effects this will inflict

    public Damage(float damageValue)
    {
        value = damageValue;
    }

    public Damage(float damageValue, StatusEffect statusEffect)
    {
        value = damageValue;
        effects.Add(statusEffect);
    }

    public Damage(float damageValue, SortedSet<StatusEffect> statusEffects)
    {
        value = damageValue;
        effects = statusEffects;
    }
}
