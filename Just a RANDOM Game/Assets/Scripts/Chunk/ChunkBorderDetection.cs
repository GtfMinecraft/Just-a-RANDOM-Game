using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBorderDetection : MonoBehaviour
{
    public ChunkTypes chunkType;

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            ChunkLoadingController.instance.ChangeLoadedChunks(chunkType);
        }
    }
}
