using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDirector : MonoBehaviour {
	[SerializeField] private Entity entity;

	private void Update()
	{
		// ignore if entity is not set
		if (entity == null)
			return;

		foreach (StatusEffect effect in entity.activeEffects)
			effect.IncreaseTimer(Time.deltaTime);
	}

	// **THIS MUST BE CALLED WHEN CREATING AN EntityDirector!!!**
	public void SetEntity(Entity target) {
		if (entity == null)
			entity = target;
		else
			Debug.Log("Entity already set");
	}
}