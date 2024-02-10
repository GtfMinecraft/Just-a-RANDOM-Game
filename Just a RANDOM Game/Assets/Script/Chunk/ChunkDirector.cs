using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDirector : MonoBehaviour
{
	private Chunk chunk;

	private void Start()
	{
		if (chunk.environment == null)
			chunk.environment = LevelInfo.assets[chunk.environmentName];
		Instantiate(chunk.environment, transform);
	}

	public void SetChunk(Chunk c)
	{
		if (chunk == null)
			chunk = c;
		else
			Debug.Log("chunk is already set");
	}
}
