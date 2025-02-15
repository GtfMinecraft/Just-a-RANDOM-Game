using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    [SerializeField]
    private Transform location; // the entity will spawn with this transform
    [SerializeField]
    private GameObject entityPrefab; // the entity this spanwpoint will spawn

    public SpawnPoint(Transform spawnPointLocation, GameObject spawnEntityPrefab)
    {
        location = spawnPointLocation;
        entityPrefab = spawnEntityPrefab;
    }

    // spawns the entity assigned to this spawnpoint and calls the entity's `OnSpawn()` method
    public void SpawnEntity()
    {
        if (entityPrefab == null)
        {
            Debug.Log($"No entity set to spawnpoint {gameObject.name}");
            return;
        }

        GameObject.Instantiate(entityPrefab, location).OnSpawn();
    }
}
