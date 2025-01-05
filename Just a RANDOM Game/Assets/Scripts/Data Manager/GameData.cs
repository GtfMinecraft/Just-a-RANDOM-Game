using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<InventoryData> inventoryData;
    public MapData mapData;
    public Dictionary<string, LevelData> levelData;

    //TODO: put all data that need to save in this class

    public GameData()
    {
        inventoryData = Enumerable.Repeat(new InventoryData(), 7).ToList();

        mapData = new MapData();

        levelData = new Dictionary<string, LevelData>()
        {
            { "chunk", new("chunk") },
        };
    }

    public class LevelData
    {
        public string levelInfo;
        public string assetList;

        public LevelData() { }

        public LevelData(string levelName)
        {
            switch (levelName)
            {
                case "chunk":
                    levelInfo = "{\"$type\":\"LevelInfo, Assembly-CSharp\",\"chunks\":{\"$type\":\"System.Collections.Generic.List`1[[Chunk, Assembly-CSharp]], mscorlib\",\"$values\":[{\"$type\":\"LoggingChunk, Assembly-CSharp\",\"type\":0,\"position\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":0.0,\"magnitude\":0.0,\"sqrMagnitude\":0.0},\"environmentName\":\"defaultLoggingChunkEnvrionmentPrefab\"}]},\"walls\":{\"$type\":\"System.Collections.Generic.List`1[[ChunkWall, Assembly-CSharp]], mscorlib\",\"$values\":[{\"$type\":\"ItemBasedChunkWall, Assembly-CSharp\",\"position\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":20.0,\"y\":0.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":1.0,\"y\":0.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":20.0,\"sqrMagnitude\":400.0},\"direction\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":0.0,\"magnitude\":0.0,\"sqrMagnitude\":0.0}}},{\"$type\":\"ItemBasedChunkWall, Assembly-CSharp\",\"position\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":-20.0,\"y\":0.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":-1.0,\"y\":0.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":20.0,\"sqrMagnitude\":400.0},\"direction\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":0.0,\"magnitude\":0.0,\"sqrMagnitude\":0.0}}},{\"$type\":\"ItemBasedChunkWall, Assembly-CSharp\",\"position\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":20.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":1.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":20.0,\"sqrMagnitude\":400.0},\"direction\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.707106769,\"z\":0.0,\"w\":0.707106769,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":90.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":1.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":90.0,\"sqrMagnitude\":8100.0},\"normalized\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.7071068,\"z\":0.0,\"w\":0.7071068,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":90.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":1.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":90.0,\"sqrMagnitude\":8100.0}}}},{\"$type\":\"ItemBasedChunkWall, Assembly-CSharp\",\"position\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":-20.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.0,\"z\":-1.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":20.0,\"sqrMagnitude\":400.0},\"direction\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.707106769,\"z\":0.0,\"w\":0.707106769,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":90.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":1.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":90.0,\"sqrMagnitude\":8100.0},\"normalized\":{\"$type\":\"UnityEngine.Quaternion, UnityEngine.CoreModule\",\"x\":0.0,\"y\":0.7071068,\"z\":0.0,\"w\":0.7071068,\"eulerAngles\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":90.0,\"z\":0.0,\"normalized\":{\"$type\":\"UnityEngine.Vector3, UnityEngine.CoreModule\",\"x\":0.0,\"y\":1.0,\"z\":0.0,\"magnitude\":1.0,\"sqrMagnitude\":1.0},\"magnitude\":90.0,\"sqrMagnitude\":8100.0}}}}]}}";
                    assetList = "[\"defaultLoggingChunkEnvrionmentPrefab\", \"chunkWallPrefab\"]";
                    break;
                default:
                    Debug.LogError("Undefined level when creating an instace of GameData.LevelData");
                    break;
            }
        }
    }

    public class MapData
    {
        public bool[] unlockedChunks;
        public List<GameObject> beacons;
        public GameObject selectedBeacon;

        public MapData() 
        { 
            unlockedChunks = new bool[30];
            beacons = new List<GameObject> {};
        }
    }

    public class InventoryData
    {
        public List<int> itemIDs;
        public List<int> currentStacks;

        public InventoryData()
        {
            itemIDs = Enumerable.Repeat(0, 10).ToList();
            currentStacks = Enumerable.Repeat(0, 10).ToList();
        }
    }
}
