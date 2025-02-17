using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class ItemDropHandler : MonoBehaviour, IDataPersistence
{
    public static ItemDropHandler instance;

    public Transform itemDropParent;

    [Serializable]
    public class ItemDrop
    {
        public int itemID;
        public ChunkTypes chunk;
        public Vector3 position;
        public Quaternion rotation;
    }
    private List<ItemDrop> itemDrops;
    private List<GameObject> itemObjs;

    private ItemDatabase database;

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

    // Start is called before the first frame update
    void Start()
    {
        database = PlayerItemController.instance.database;
    }

    public void UnloadChunk(ChunkTypes chunk)
    {
        for (int i = 0; i < itemDrops.Count; i++)
        {
            if (itemDrops[i].chunk == chunk)
            {
                if (itemObjs[i] != null)
                {
                    ObjectPoolManager.DestroyPooled(itemObjs[i]);//gotta give each item their unique reference so that the itemObjs are distinguishable
                    itemObjs[i] = null;
                }
                else
                {
                    itemDrops.RemoveAt(i);
                    itemObjs.RemoveAt(i);
                }
            }
        }
    }

    public void LoadChunk(ChunkTypes chunk)
    {
        for (int i = 0; i < itemDrops.Count; i++)
        {
            if (itemDrops[i].chunk == chunk && itemObjs[i] == null)
            {
                SpawnDrop(i);
            }
        }
    }

    public void SpawnNewDrop(int itemID, ChunkTypes chunk, Vector3 position, Quaternion rotation = default)
    {
        itemDrops.Add(new ItemDrop { itemID = itemID, chunk = chunk, position = position, rotation = rotation });
        itemObjs.Add(null);
        SpawnDrop(-1);
    }

    private void SpawnDrop(int index)
    {
        GameObject model = database.GetItem[itemDrops[index].itemID].model;
        if (model.GetComponent<ItemInteractable>() == null)
            model.AddComponent<ItemInteractable>();
        itemObjs[index] = ObjectPoolManager.CreatePooled(model, itemDrops[index].position, itemDrops[index].rotation);
        itemObjs[index].transform.SetParent(itemDropParent);
        itemObjs[index].GetComponent<ItemInteractable>().itemID = itemDrops[index].itemID;
    }

    public void LoadData(GameData data)
    {
        itemDrops = data.itemDropData.itemDrops;
        itemObjs = new List<GameObject>(itemDrops.Count);
    }

    public void SaveData(GameData data)
    {
        data.itemDropData.itemDrops = itemDrops;
    }
}
