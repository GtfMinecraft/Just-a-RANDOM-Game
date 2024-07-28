using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storing Config")]
    [SerializeField] private string[] fileName;

    public static DataPersistenceManager instance;

    public bool useEncryption = true;
    [HideInInspector]
    public int saveFileIndex = 0;

    private GameData gameData;
    private FileDataHandler dataHandler;
    private List<IDataPersistence> dataPersistenceObjects;

    /*  
     *  TODO: save file timing, multiple save files
     *  
     *  Things that should be saved
     *  1. inventory status
     *  2. item count
     *  3. NPC dialogue
     *  4. chunk status
     *  5. player status / position
     *  
     *  When to save data
     *  1. Buying tools and upgrading
     *  2. every 5 minutes from the last save
     *  3. chunk unlocking
     *  4. event finishing / before event boss
     *  5. before boss
     *  6. Closing the game
     */

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

        dataHandler = new FileDataHandler(Application.persistentDataPath, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();

        //initialize
        PlayerPrefs.SetString("rightItemsString", "");
        PlayerPrefs.SetString("leftItemsString", "");
        PlayerPrefs.SetInt("selectedTool", 0);
        PlayerPrefs.SetInt("selectedGroup", 0);
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load(fileName);

        if(gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if(gameData == null)
        {
            Debug.Log("Failed to load the game. DataPersistenceManager.instance.gameData was not initialized");
            return;
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        if(gameData == null)
        {
            Debug.Log("Failed to save the game. DataPersistenceManager.instance.gameData was not initialized");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        dataHandler.Save(fileName, gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
