using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingController : MonoBehaviour, IDataPersistence
{
    public int farmlandIndex;

    private int planted = 0;
    private float unloadTime = 0;
    private float ripeTime = 0;

    private void Update()
    {
        if (ripeTime > 0)
        {
            ripeTime -= Time.deltaTime;
        }
    }

    public void Harvest(int plantID = 0)
    {
        if(plantID == 0 && planted != 0)
        {
            //Destroy the plant and drop if is ripe
        }
        else if(plantID != 0 && planted == 0)
        {
            //memorize it
            Item plant = PlayerItemController.instance.database.GetItem[plantID];
            Instantiate(plant.model);
            ripeTime = plant.attackSpeed;
            planted = plantID;
            Debug.Log("planting " + plant.name);
        }
    }

    public void LoadData(GameData data)
    {
        //instantiate crop if there are crops and get ripeTime
    }

    public void SaveData(GameData data)
    {
        //save crop type, ripeTime, and unload time
    }
}
