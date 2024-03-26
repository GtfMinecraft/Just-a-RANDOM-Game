using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public abstract class Chunk
{
	[System.Serializable] public enum ChunkType { Logging, Fishing, Mining, Shop };

	public ChunkType type;
	public Vector3 position;
	public string environmentName; // where is environment stored in `LevelInfo.assets`?
	[System.NonSerialized] public GameObject environment; // the envrionment (terrain, buildings, etc) of the chunk

	[JsonConstructor] public Chunk(ChunkType t, Vector3 pos, string env) { type = t; position = pos; environment = LevelInfo.assets[env]; }
	public Chunk(ChunkType t, Vector3 pos, GameObject env) { type = t; position = pos; environment = env; }
}