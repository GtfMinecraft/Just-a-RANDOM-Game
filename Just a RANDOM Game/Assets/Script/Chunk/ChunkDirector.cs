using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDirector : MonoBehaviour
{
	private Chunk chunk;

	private void Start()
	{
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
