using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoadingController : MonoBehaviour, IDataPersistence
{
    public static ChunkLoadingController instance;

    public bool[] unlockedChunks { get; private set; } = new bool[6] { true, false, false, false, false, false };
    public ChunkTypes currentChunk = ChunkTypes.Logging;
    public bool[] loadedChunks { get; private set; } = new bool[6];

    private Vector3 spawnPos;
    private Quaternion spawnRot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SpawnPlayer();
        if(DataPersistenceManager.firstLoad)
            loadedChunks[(int)currentChunk] = true;
        ChangeLoadedChunks(currentChunk);
    }

    public void UnlockChunk(ChunkTypes chunk)
    {
        if (unlockedChunks[(int)chunk])
        {
            Debug.LogError($"Chunk {chunk} is called to unlock when it's already unlocked.");
        }
        else
        {
            unlockedChunks[(int)chunk] = true;
        }
    }

    //ChunkBorderDetections will be on either side of the door, serving as chunk enter & door closing collider
    //ChunkDoorInteractable depends on unlockedChunks (unlock/open) and it'll auto-close after player leaves its vicinity

    public void ChangeLoadedChunks(ChunkTypes chunk)
    {
        currentChunk = chunk;
        bool[] chunksToLoad = new bool[6];

        if (chunk == ChunkTypes.Logging || chunk == ChunkTypes.Farming)
        {
            chunksToLoad[1] = chunksToLoad[4] = true;
            chunksToLoad[(int)chunk] = true;
        }
        else if (chunk == ChunkTypes.Mining || chunk == ChunkTypes.SpiderCave)
        {
            chunksToLoad[0] = chunksToLoad[3] = true;
            chunksToLoad[(int)chunk] = chunksToLoad[6 - (int)chunk] = true;
        }
        else if(chunk == ChunkTypes.Fishing || chunk == ChunkTypes.SnowField)
        {
            chunksToLoad[(int)chunk] = chunksToLoad[6 - (int)chunk] = true;
        }

        DataPersistenceManager.instance.SaveGame();
        LoadChunks(chunksToLoad);
    }

    private void LoadChunks(bool[] chunks)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (chunks[i] && !loadedChunks[i])
            {
                loadedChunks[i] = true;
                //load the chunk
                ItemDropHandler.instance.LoadChunk((ChunkTypes)i);
            }
            else if (!chunks[i] && loadedChunks[i])
            {
                loadedChunks[i] = false;
                //unload the chunk
                ItemDropHandler.instance.UnloadChunk((ChunkTypes)i);
            }
        }
    }

    public void SpawnPlayer()
    {
        PlayerController player = PlayerController.instance;
        player.transform.position = spawnPos;
        player.playerObj.rotation = spawnRot;
    }

    public void LoadData(GameData data)
    {
        unlockedChunks = data.statisticsData.unlockedChunks;
        currentChunk = data.statisticsData.playerChunk;
        float[] pos = data.statisticsData.playerPos;
        float[] rot = data.statisticsData.playerRot;
        spawnPos = new Vector3(pos[0], pos[1], pos[2]);
        spawnRot = Quaternion.Euler(rot[0], rot[1], rot[2]);
    }

    public void SaveData(GameData data)
    {
        data.statisticsData.unlockedChunks = unlockedChunks;
        data.statisticsData.playerChunk = currentChunk;
        Vector3 pos = PlayerController.instance.transform.position;
        data.statisticsData.playerPos = new float[] { pos.x, pos.y, pos.z };
        Vector3 rot = PlayerController.instance.playerObj.rotation.eulerAngles;
        data.statisticsData.playerRot = new float[] { rot.x, rot.y, rot.z };
    }
}
