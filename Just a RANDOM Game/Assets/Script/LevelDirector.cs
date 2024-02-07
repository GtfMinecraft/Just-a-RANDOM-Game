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
    [SerializeField] private string levelPath;

    private LevelInfo level;

    private void Awake()
    {
        // load resources
        LevelInfo.defaultLoggingChunkEnvrionmentPrefab = PrefabUtility.LoadPrefabContents("./Assets/Script/Chunk/Environment/LoggingChunkDefault.prefab");
        LevelInfo.chunkWallPrefab = PrefabUtility.LoadPrefabContents("./Assets/Script/Chunk/Environment/ChunkWallPlaceholoder.prefab");

        // load world
        level = JsonConvert.DeserializeObject<LevelInfo>(File.ReadAllText(levelPath), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        level.chunks[0].environment = LevelInfo.defaultLoggingChunkEnvrionmentPrefab; // TODO: remove when we figure out how to store `GameObject`s
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        foreach (Chunk chunk in level.chunks)
            Instantiate(chunkPrefab, chunk.position, Quaternion.identity).GetComponent<ChunkDirector>().SetChunk(chunk);

        foreach (ChunkWall wall in level.walls)
            Instantiate(wallPrefab, wall.position, wall.direction).GetComponent<ChunkWallDirector>().SetChunkWall(wall);
    }
}
