using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEntity : Entity
{
    public int itemID = 18;

    public override void Kill()
    {
        // TODO: voronoi destroy
        //random 2 to 3
        ItemDropHandler.instance.SpawnNewDrop(itemID, transform.position, ChunkLoadingController.instance.currentChunk);
        //set the chunk for the drop if the axe crossed chunk border

        Destroy(gameObject);
    }

    public override void LoadData(GameData data)
    {
        if (!data.entityData.ContainsKey(entityName))
        {
            Destroy(gameObject);
        }
        else
        {
            base.LoadData(data);
        }
    }

    public override void SaveData(GameData data)
    {
        if (health <= 0)
        {
            data.entityData.Remove(entityName);
        }
        else
        {
            base.SaveData(data);
        }
    }
}
