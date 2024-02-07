using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfo 
{
	public static GameObject defaultLoggingChunkEnvrionmentPrefab;
	public static GameObject chunkWallPrefab;

	public List<Chunk> chunks;
	public List<ChunkWall> walls;

	public LevelInfo() { chunks = new List<Chunk>(); walls = new List<ChunkWall>(); }
	public LevelInfo(List<Chunk> c, List<ChunkWall> w) { chunks = c; walls = w; }

	public void LogInfo() { Debug.Log($"Chunks: {chunks}, Walls: {walls}"); }
}