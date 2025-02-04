using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour, IDataPersistence
{
    public static DayNightCycle instance;

    public float inGameTime;
    public int gameDays {  get; private set; }

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

    // Update is called once per frame
    void Update()
    {
        inGameTime += Time.deltaTime;

        if (inGameTime >= 1440)
        {
            inGameTime -= 1440;
            ++gameDays;
        }
    }

    //cloud and directional light

    public void LoadData(GameData data)
    {
        gameDays = data.statisticsData.gameDays;
        inGameTime = data.statisticsData.inGameTime;
    }

    public void SaveData(GameData data)
    {
        data.statisticsData.gameDays = gameDays;
        data.statisticsData.inGameTime = inGameTime;
    }
}
