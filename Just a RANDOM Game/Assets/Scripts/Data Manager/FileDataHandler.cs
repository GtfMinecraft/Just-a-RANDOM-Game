using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class FileDataHandler
{
    private string filePath;
    private const string key = "2859385910ufqnYdG7X+";
    private bool useEncryption;

    public FileDataHandler(string filePath, bool useEncryption)
    {
        this.filePath = filePath;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string[] fileName)
    {
        GameData loadData = null;

        foreach (string file in fileName)
        {
            string fullPath = Path.Combine(filePath, file + ".dat");

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    if (dataToLoad[0] != '{' && dataToLoad[0] != '[')
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    loadData ??= new GameData();

                    //TODO: check in the data to load
                    if (file == "InventoryData")
                    {
                        loadData.inventoryData = JsonConvert.DeserializeObject<List<GameData.InventoryData>>(dataToLoad);
                    }
                    else if (file == "MapData")
                    {
                        loadData.mapData = JsonConvert.DeserializeObject<GameData.MapData>(dataToLoad);
                    }
                    else
                    {
                        Debug.LogError($"Loading action undefined for {file} when loading data from file");
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
                }
            }
        }

        return loadData;
    }

    public void Save(string[] fileName, GameData data)
    {
        foreach(string file in fileName)
        {
            string fullPath = Path.Combine(filePath, file + ".dat");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                //TODO: check in the data to save
                string dataToSave = "";

                if (file == "InventoryData")
                {
                    if (data.inventoryData == null)
                    {
                        Debug.Log($"No data for {file} when saving data");
                        continue;
                    }
                    dataToSave = JsonConvert.SerializeObject(data.inventoryData);
                }
                else if (file == "MapData")
                {
                    if (data.mapData == null)
                    {
                        Debug.Log($"No data for {file} when saving data");
                        continue;
                    }
                    dataToSave = JsonConvert.SerializeObject(data.mapData);
                }
                else
                {
                    Debug.LogError($"Saving action undefined for {file} when saving data");
                    continue;
                }

                if (useEncryption)
                {
                    dataToSave = EncryptDecrypt(dataToSave);
                }

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToSave);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred while trying to save data to file: " + fullPath + "\n" + e);
            }
        }
    }

    private string EncryptDecrypt(string data)
    {
        string result = "";
        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ key[i % key.Length]);
        }
        return result;
    }
}