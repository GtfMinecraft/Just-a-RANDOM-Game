using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChunkInteraction : MonoBehaviour
{
    public static PlayerChunkInteraction instance;

    public InventoryTypes currentChunk;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        InventoryTypes chunkTemp = currentChunk;
        //detect chunk
        if(currentChunk != chunkTemp)
        {
            currentChunk = chunkTemp;
            PlayerItemController.instance.ChangeChunk(currentChunk);
        }
    }

    public void UpdatelDefaultItem(InventoryTypes types)
    {
        //update item to strongest item/last holding item
        if (PlayerItemController.instance.defaultItems[(int)types] == null)
        {
            //set to strongest item
        }
    }
}
