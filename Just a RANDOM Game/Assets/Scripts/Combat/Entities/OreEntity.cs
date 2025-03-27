using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreEntity : Entity
{
    public int itemID;

    protected override void Start()
    {
        base.Start();
    }

    public override void Kill()
    {
        // TODO: voronoi destroy
        //random 2 to 3
        ItemDropHandler.instance.SpawnNewDrop(itemID, transform.position, ChunkLoadingController.instance.currentChunk);

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
        if(health <= 0)
        {
            data.entityData.Remove(entityName);
        }
        else
        {
            base.SaveData(data);
        }
    }
}
