using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreEntity : Entity
{
    public int itemID;

    public override void Kill()
    {
        // TODO: voronoi destroy
        //random 2 to 3
        ItemDropHandler.instance.SpawnNewDrop(itemID, transform.position);
    }
}
