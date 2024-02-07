using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Chunk
{
	[System.Serializable] public enum ChunkType { Logging, Fishing, Mining, Shop };

	public ChunkType type;
	public Vector3 position;
	// TODO: come up with a way to save and load this
	[System.NonSerialized] public GameObject environment; // the envrionment (terrain, buildings, etc) of the chunk

	public Chunk(ChunkType t, Vector3 pos, GameObject env) { type = t; position = pos; environment = env; }
}