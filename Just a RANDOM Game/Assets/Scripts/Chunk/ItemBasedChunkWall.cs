using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBasedChunkWall : ChunkWall
{
	public ItemBasedChunkWall(Vector3 pos, Quaternion dir) : base(pos, dir) {}

	public override bool UnlockCriteriaMet()
	{
		// TODO: add check
		return true;
	}
}