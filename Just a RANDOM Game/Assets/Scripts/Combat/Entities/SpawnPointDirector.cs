using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointDirector : MonoBehaviour
{
    [SerializeField]
    private SpawnPoint spawnPoint;
    private float intervalStartTime;
    private float spawnInterval;
    private int remainingSpawns;

    private void Update()
    {
        if (Time.time - intervalStartTime >= spawnInterval && remainingSpawns != 0)
        {
            intervalStartTime = Time.time;
            if (remainingSpawns > 0)
                remainingSpawns--;

            spawnPoint.SpawnEntity();
        }
    }

    public void SpawnEntity()
    {
        spawnPoint.SpawnEntity();
    }

    // Spawns entity once per `intervalSeconds` seconds, up to `amount` times
    // Does not stop spawining if `amount` < 0
    public void SpawnEntityInterval(float intervalSeconds, int amount = -1)
    {
        intervalStartTime = Time.time;
        spawnInterval = intervalSeconds;
        remainingSpawns = amount;
    }
}
