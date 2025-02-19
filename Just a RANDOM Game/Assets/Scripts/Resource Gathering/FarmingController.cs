using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class FarmingController : MonoBehaviour, IDataPersistence
{
    public int farmlandIndex;

    private int cropID = 0;
    private float chance = 0;//attackSpeed is how many minutes to ripe, random tick will check every minute
    private int stage = 0;
    private float timer = 0;
    
    private float plantTime;
    private int plantDay;
    private float duration;

    private GameObject cropObj;

    private void Start()
    {
        Item crop = PlayerItemController.instance.database.GetItem[cropID];

        chance = 2 / crop.attackSpeed;
        if (cropID != 0)
        {
            cropObj = Instantiate(crop.model, transform);

            stage += SampleBinomialFast((int)duration, chance);

            if (stage > 2)
                stage = 2;

            for (int i = 0; i < stage; i++)
                GrowCrop();
        }
    }

    private int SampleBinomialFast(int trials, float probability)
    {
        float mean = trials * probability;
        float stddev = Mathf.Sqrt(trials * probability * (1 - probability));
        return Mathf.Max(0, Mathf.RoundToInt(RandomGaussian(mean, stddev)));
    }

    private float RandomGaussian(float mean, float stddev)
    {
        float u1 = Random.Range(0 - Mathf.Epsilon, 1f);
        float u2 = Random.Range(0 - Mathf.Epsilon, 1f);
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stddev * randStdNormal;
    }

    private void Update()
    {
        if (cropID != 0 && stage < 2)
        {
            timer += Time.deltaTime;
            if (timer >= 60)
            {
                timer = 0;
                if (Random.value <= chance)
                {
                    stage++;
                    GrowCrop();
                }
            }
        }
    }

    public void Harvest(int plantID = 0)
    {
        if(plantID == 0 && cropID != 0)
        {
            if (stage == 2)
            {
                if(Random.value >= 0.6f)
                    ItemDropHandler.instance.SpawnNewDrop(plantID, ChunkTypes.Farming, transform.position);
                ItemDropHandler.instance.SpawnNewDrop(plantID, ChunkTypes.Farming, transform.position);
            }
            ItemDropHandler.instance.SpawnNewDrop(plantID, ChunkTypes.Farming, transform.position);
            cropID = 0;
            Destroy(cropObj);
        }
        else if(plantID != 0 && cropID == 0)
        {
            Item plant = PlayerItemController.instance.database.GetItem[plantID];
            cropObj = Instantiate(plant.model, transform);
            cropID = plantID;
            stage = 0;
            timer = 0;
            plantDay = DayNightCycle.instance.gameDays;
            plantTime = DayNightCycle.instance.inGameTime;
        }
    }

    private void GrowCrop()
    {
        Vector3 pos = cropObj.transform.position;
        cropObj.transform.position = new Vector3(pos.x, pos.y + 0.1f, pos.z);
    }

    public void LoadData(GameData data)
    {
        if (farmlandIndex >= data.farmlandData.Count)
        {
            return;
        }

        cropID = data.farmlandData[farmlandIndex].cropID;
        stage = data.farmlandData[farmlandIndex].stage;

        if(cropID != 0)
        {
            int days = data.statisticsData.gameDays - data.farmlandData[farmlandIndex].gameDay;
            float time = data.statisticsData.inGameTime - data.farmlandData[farmlandIndex].inGameTime;

            duration = days * 24 + time;
        }
    }

    public void SaveData(GameData data)
    {
        while (data.farmlandData.Count <= farmlandIndex)
        {
            data.farmlandData.Add(null);
        }

        data.farmlandData[farmlandIndex] = new GameData.FarmlandData { 
            cropID = cropID,
            gameDay = DayNightCycle.instance.gameDays,
            inGameTime = DayNightCycle.instance.inGameTime,
            stage = stage
        };
    }
}
