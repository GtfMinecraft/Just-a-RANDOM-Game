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
        ItemDropHandler.instance.SpawnNewDrop(itemID, transform.position);
        //set the chunk for the drop if the axe crossed chunk border
    }
}
