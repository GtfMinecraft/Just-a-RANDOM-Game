using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public class LevelDirector : MonoBehaviour, IDataPersistence
{
    public string levelName;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject loadingScreenPrefab;
    [SerializeField] private GameObject levelLoaderPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform levelLoadingParent;

    private LevelInfo level;
    private LevelLoader levelLoader;

    private void Awake()
    {
        // load world info
        //level = JsonConvert.DeserializeObject<LevelInfo>(File.ReadAllText(Path.Combine(Application.persistentDataPath, levelName + ".dat")), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        // make an instance of `LevelLoader` that generates loading screen and loads assets
        levelLoader = Instantiate(levelLoaderPrefab).GetComponent<LevelLoader>();
        levelLoader.director = this;

        // initialize player entity
        player.AddComponent(typeof(EntityDirector)).GetComponent<EntityDirector>().SetEntity(new PlayerEntity(player));
    }

    public void GenerateLevel()
    {
        foreach (Chunk chunk in level.chunks)
        {
            GameObject temp = Instantiate(chunkPrefab, chunk.position, Quaternion.identity);
            temp.transform.parent = levelLoadingParent;
            temp.GetComponent<ChunkDirector>().SetChunk(chunk);
        }

        foreach (ChunkWall wall in level.walls)
        {
            GameObject temp = Instantiate(wallPrefab, wall.position, wall.direction);
            temp.transform.parent = levelLoadingParent;
            temp.GetComponent<ChunkWallDirector>().SetChunkWall(wall);
        }
    }

    //public void SaveLevel()
    //{
    //    File.WriteAllText(Path.Combine(Application.persistentDataPath, levelName + ".dat"), JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    //}

    public void LoadData(GameData data)
    {
        level = JsonConvert.DeserializeObject<LevelInfo>(data.levelData[levelName].levelInfo, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        levelLoader.startLoading = true;
    }

    public void SaveData(GameData data)
    {
        data.levelData[levelName].levelInfo = JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }
}
