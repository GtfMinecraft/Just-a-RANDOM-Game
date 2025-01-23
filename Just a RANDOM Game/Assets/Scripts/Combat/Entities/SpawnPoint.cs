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

    public void SpawnEntity()
    {
        GameObject.Instantiate(entityPrefab, location);
    }
}