using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Color dayLightColor;
    public Color nightLightColor;

    private bool isNight = false;
    private float skyboxBlend = 0;

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

        if ((inGameTime < 6 || inGameTime > 18))
        {
            RenderSettings.skybox = nightSkybox;
            isNight = true;
            directionalLight.intensity = nightLightIntensity;
            directionalLight.color = nightLightColor;
            skyboxBlend = 1;
            nightSkybox.SetFloat("_Exposure", 1.3f);
            daySkybox.SetFloat("_Exposure", 0.1f);
        }
        else
        {
            RenderSettings.skybox = daySkybox;
            isNight = false;
            directionalLight.intensity = dayLightIntensity;
            directionalLight.color = dayLightColor;
            skyboxBlend = 0;
            nightSkybox.SetFloat("_Exposure", 0.2f);
            daySkybox.SetFloat("_Exposure", 1.3f);
        }
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

        if (isNight && skyboxBlend < 1)
        {
            skyboxBlend += Time.deltaTime / 6;
            if (skyboxBlend <= 0.5)
            {
                directionalLight.intensity = Mathf.Lerp(dayLightIntensity, 0, skyboxBlend * 2);
                float exposure = Mathf.Lerp(1.3f, 0.1f, skyboxBlend * 2);
                daySkybox.SetFloat("_Exposure", exposure);
            }
            else
            {
                directionalLight.intensity = Mathf.Lerp(0, nightLightIntensity, skyboxBlend * 2 - 1);
                if (RenderSettings.skybox != nightSkybox)
                    RenderSettings.skybox = nightSkybox;
                float exposure = Mathf.Lerp(0.2f, 1.3f, skyboxBlend * 2 - 1);
                nightSkybox.SetFloat("_Exposure", exposure);
            }

            directionalLight.color = Color.Lerp(dayLightColor, nightLightColor, skyboxBlend);
        }
        else if (!isNight && skyboxBlend > 0)
        {
            skyboxBlend -= Time.deltaTime / 6;
            if (skyboxBlend > 0.5)
            {
                directionalLight.intensity = Mathf.Lerp(0, nightLightIntensity, skyboxBlend * 2 - 1);
                float exposure = Mathf.Lerp(0.2f, 1.3f, skyboxBlend * 2 - 1);
                nightSkybox.SetFloat("_Exposure", exposure);
            }
            else
            {
                directionalLight.intensity = Mathf.Lerp(dayLightIntensity, 0, skyboxBlend * 2);
                if (RenderSettings.skybox != daySkybox)
                    RenderSettings.skybox = daySkybox;
                float exposure = Mathf.Lerp(1.3f, 0.1f, skyboxBlend * 2);
                daySkybox.SetFloat("_Exposure", exposure);
            }

            directionalLight.color = Color.Lerp(dayLightColor, nightLightColor, skyboxBlend);
        }

        moon.rotation = Camera.main.transform.rotation * Quaternion.Euler(-90, 0, 0);
        float xz = Mathf.Cos((inGameTime + 6) / 12 * Mathf.PI);
        moon.position = new Vector3(xz / 2, Mathf.Sin((inGameTime + 6) / 12 * Mathf.PI), -xz * 1.732051f / 2) * moonDistance;

        if (RenderSettings.skybox == nightSkybox)
            directionalLight.transform.rotation = Quaternion.Euler((inGameTime + 6) * 15, -30, 0);
        else if (RenderSettings.skybox == daySkybox)
            directionalLight.transform.rotation = Quaternion.Euler((inGameTime - 6) * 15, -30, 0);

        if ((inGameTime < 6 || inGameTime > 18) && !isNight)
        {
            isNight = true;
        }
        else if(inGameTime >= 6 && inGameTime <= 18 && isNight)
        {
            isNight = false;
        }
    }

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
