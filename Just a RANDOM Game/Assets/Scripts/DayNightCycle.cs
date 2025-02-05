using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour, IDataPersistence
{
    public static DayNightCycle instance;

    public float inGameTime;
    public int gameDays {  get; private set; }

    [Header("Sun and Moon")]
    public Light directionalLight;
    public Transform moon;
    public float moonDistance;
    public float moonScale;
    public float dayLightIntensity;
    public float nightLightIntensity;
    public Material daySkybox;
    public Material nightSkybox;

    private bool isNight = false;

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

    void Start()
    {
        moon.localScale = Vector3.one * moonScale;
    }

    // Update is called once per frame
    void Update()
    {
        inGameTime += Time.deltaTime / 60;

        if (inGameTime >= 24)
        {
            inGameTime -= 24;
            ++gameDays;
        }

        moon.rotation = Camera.main.transform.rotation * Quaternion.Euler(-90, 0, 0);
        float xz = Mathf.Cos((inGameTime + 6) / 12 * Mathf.PI);
        moon.position = new Vector3(xz / 2, Mathf.Sin((inGameTime + 6) / 12 * Mathf.PI), -xz * 1.732051f / 2) * moonDistance;

        if((inGameTime < 6 || inGameTime > 18))
        {
            directionalLight.transform.rotation = Quaternion.Euler((inGameTime + 6) * 15, -30, 0);

            if (!isNight)
            {
                directionalLight.intensity = nightLightIntensity;
                isNight = true;
                RenderSettings.skybox = nightSkybox;
            }
        }
        else if(inGameTime >= 6 && inGameTime <= 18)
        {
            directionalLight.transform.rotation = Quaternion.Euler((inGameTime - 6) * 15, -30, 0);

            if (isNight)
            {
                directionalLight.intensity = dayLightIntensity;
                isNight = false;
                RenderSettings.skybox = daySkybox;
            }
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
