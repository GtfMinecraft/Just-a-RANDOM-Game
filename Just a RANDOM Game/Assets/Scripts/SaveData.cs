using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public static SaveData instance;

    private const string key = "2859385910ufqnYdG7X+";

    /*
     *  Things that should be saved
     *  1. inventory status
     *  2. item count
     *  3. NPC dialogue
     *  4. chunk status
     *  
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
    }

    private string EncodeDecode(string data)
    {
        string result = "";
        for(int i = 0 ; i < data.Length; i++)
        {
            result += data[i] ^ key[i % key.Length];
        }
        return result;
    }

    public string ReadFile(string filePath)
    {
        string fullPath = Application.persistentDataPath + filePath;
        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            return EncodeDecode(data);
        }
        return null;
    }

    private void SaveFile(string filePath, string data)
    {
        string fullPath = Application.persistentDataPath + filePath;
        File.WriteAllText(fullPath, EncodeDecode(data));
    }

    //
    public void SaveAll()
    {
        for(int i = 0; i < null; ++i)
        {
            SaveFile(null, null);
        }
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }
}
