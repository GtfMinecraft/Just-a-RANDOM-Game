using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkWallDirector : MonoBehaviour
{
    private ChunkWall wall;

    private void Start()
    {
        Instantiate(LevelInfo.assets["chunkWallPrefab"], wall.position, wall.direction, transform);
    }

    public void Unlock() 
    {
        if (!wall.UnlockCriteriaMet())
            return;
        
        // TODO: add effect when wall dies

        Destroy(gameObject);
    }

    public void SetChunkWall(ChunkWall w)
    {
        if (wall == null)
            wall = w;
        else
            Debug.Log("chunkwall already set");
    }
}
