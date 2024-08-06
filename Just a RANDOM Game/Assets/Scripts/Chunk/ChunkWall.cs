using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ChunkWall
{
	public static readonly float unlockDistance = 5f;
	public Vector3 position;
	public Quaternion direction;
	[System.NonSerialized] public GameObject wallModel;

	public ChunkWall(Vector3 pos, Quaternion dir) { position = pos; direction = dir; }

	public abstract bool UnlockCriteriaMet();
}