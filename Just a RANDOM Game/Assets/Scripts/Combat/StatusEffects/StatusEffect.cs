using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// status effects are effects that can be applied to entities
// they can be instantaneous or have an effect over time
// isConstant effects **only use StartEffect and EndEffect!!!**
abstract public class StatusEffect
{
    public readonly bool isInstant; // whether this status effect is triggered and deleted instantly (overrides isConstant)
    public readonly bool isConstant; // whether this status effect is applied constantly (overridden by isInstant)
	public float timeToLiveSeconds; // how long does this effect last?
	public bool stackable; // can this effect be stacked?
	public bool refreshable; // does adding another stack of this effect refresh its time to live?
    public float cooldownSeconds; // the cooldown of this status effect (only matters if isConstant and isInstant are both false)
    public int stack; // how many stacks (instances) of this effect is active?
    public Entity source; // the source of this status effect
	public Entity target; // the entity this effect is affecting

    private float cooldownLeftSeconds; // how much cooldown is left until next tick?
	private float lifeTImeLeftSeconds; // time until this effect dies

    protected virtual void StartEffect(Entity target) { }

    protected virtual void TickEffect(Entity target) { }

    protected virtual void EndEffect(Entity target) { }

	public void IncreaseTimer(float timeSeconds)
	{		
		// update timers
		cooldownLeftSeconds -= timeSeconds;
		lifeTImeLeftSeconds -= timeSeconds;

		// trigger effect if conditions are met
		if (!isConstant)
		{
			while (cooldownLeftSeconds <= 0f)
			{
				TickEffect(target);
				cooldownLeftSeconds += cooldownSeconds;
			}
		}
		if (lifeTImeLeftSeconds <= 0f)
		{
			EndEffect(target);
			Remove();
		}
	}

	public void AddStack(int stackAmount)
	{
		if (!stackable)
			return;
			
		if (refreshable)
			lifeTImeLeftSeconds = timeToLiveSeconds;
		stack += stackAmount;
	}

	public void Remove()
	{
		target.activeEffects.Remove(this);
	}

    public StatusEffect(Entity sourceEntity, Entity targetEntity, float cooldownTimeSeconds = 0f, bool isInstantEffect = false, bool isConstantEffect = false, int stackAmount = 1)
    {
        source = sourceEntity;
		target = targetEntity;
        cooldownSeconds = cooldownTimeSeconds;
		isInstant = isInstantEffect;
		isConstant = isConstantEffect;
		stack = stackAmount;

		StartEffect(target);

		if (isInstant) {
			EndEffect(target);
			Remove();
		}
    }
}
