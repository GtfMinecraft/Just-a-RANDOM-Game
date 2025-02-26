using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfo 
{
	public static Dictionary<string, GameObject> assets = new Dictionary<string, GameObject>();

	public List<Chunk> chunks;
	public List<ChunkWall> walls;

	public LevelInfo() { chunks = new List<Chunk>(); walls = new List<ChunkWall>(); }
	public LevelInfo(List<Chunk> c, List<ChunkWall> w) { chunks = c; walls = w; }
}