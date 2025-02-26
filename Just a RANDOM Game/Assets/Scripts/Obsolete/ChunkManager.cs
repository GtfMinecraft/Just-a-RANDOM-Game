using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public ChunkData chunkData;
    [System.Serializable]
    public class Chunk
    {
        public bool isUnlocked; 
        public int resources; 
    }
    public class ChunkData
    {
        public List<Chunk> chunks = new List<Chunk>();
    }

    private void InitializeMap()
    {
        chunkData = new ChunkData();
        for (int i = 0; i < 30; i++)
        {
            Chunk newChunk = new Chunk();
            newChunk.isUnlocked = false;
            newChunk.resources = 0;
            chunkData.chunks.Add(newChunk);
        }
        
        chunkData.chunks[0].isUnlocked = true;
        chunkData.chunks[1].isUnlocked = true;
        chunkData.chunks[5].isUnlocked = true;
        chunkData.chunks[6].isUnlocked = true;
    }

    public void UnlockChunk(int index)
    {
        if (index >= 0 && index < chunkData.chunks.Count)
        {
            chunkData.chunks[index].isUnlocked = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

    }

}


    