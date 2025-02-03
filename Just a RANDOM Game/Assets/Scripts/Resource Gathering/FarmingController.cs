using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingController : MonoBehaviour
{
    private ItemDatabase database;
    private int planted = 0;

    private float unloadTime = 0;

    private void Start()
    {
        database = PlayerItemController.instance.database;
    }

    public void Harvest(int plantID = 0)
    {
        if(plantID == 0 && planted != 0)
        {
            //check if is ripe
        }
        else if(plantID != 0 && planted == 0)
        {
            //memorize it
            Item plant = database.GetItem[plantID];
            Instantiate(plant.model);
            Debug.Log("planting " + plant.name);
        }
    }
}
