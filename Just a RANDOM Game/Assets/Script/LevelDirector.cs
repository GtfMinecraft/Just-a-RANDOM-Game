using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject loadingScreenPrefab;
    [SerializeField] private GameObject levelLoaderPrefab;
    [SerializeField] private string levelPath;

    private LevelInfo level;

    private void Awake()
    {
        // load world info
        level = JsonConvert.DeserializeObject<LevelInfo>(File.ReadAllText(levelPath), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        // make an instance of `LevelLoader` that generates loading screen and loads assets
        Instantiate(levelLoaderPrefab).GetComponent<LevelLoader>().director = this;

        SaveLevel();
    }

    public void GenerateLevel()
    {
        foreach (Chunk chunk in level.chunks)
            Instantiate(chunkPrefab, chunk.position, Quaternion.identity).GetComponent<ChunkDirector>().SetChunk(chunk);

        foreach (ChunkWall wall in level.walls)
            Instantiate(wallPrefab, wall.position, wall.direction).GetComponent<ChunkWallDirector>().SetChunkWall(wall);
    }

    public void SaveLevel()
    {
        File.WriteAllText(levelPath, JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }
}
