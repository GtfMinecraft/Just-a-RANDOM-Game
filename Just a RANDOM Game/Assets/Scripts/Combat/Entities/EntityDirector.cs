using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDirector : MonoBehaviour
{
    [SerializeField]
    private Entity entity;
    private bool isActive;

    private void Start()
    {
        entity.OnSpawn();
    }

    private void Update()
    {
        // ignore if entity is not set or is inactive
        if (entity == null)
        {
            Debug.Log($"EntityDirector attached to {gameObject.name} does not have a valid entity");
            return;
        }
        else if (isActive == false)
            return;

        foreach (StatusEffect effect in entity.activeEffects)
            effect.IncreaseTimer(Time.deltaTime);
    }

    // **THIS MUST BE CALLED WHEN CREATING AN EntityDirector!!!**
    public void SetEntity(Entity target)
    {
        if (entity == null)
            entity = target;
        else
            Debug.Log("Entity already set");
    }

    // When the entity dies, respawn it at `spawnPoint` after `delaySeconds` seconds
    public void SetRespawn(SpawnPointDirector spawnPoint, float delaySeconds = 0)
    {
        entity.respawnPoint = spawnPoint;
        entity.respawnCooldown = delaySeconds;
    }
}
