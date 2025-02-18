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
        public float[] position;
        public float[] rotation;
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
        Vector3 rot = rotation.eulerAngles;
        itemDrops.Add(new ItemDrop {
            itemID = itemID,
            chunk = chunk,
            position = new float[] { position.x, position.y, position.z },
            rotation = new float[] { rot.x, rot.y, rot.z }
        });
        itemObjs.Add(null);
        SpawnDrop(itemDrops.Count - 1);
    }

    private void SpawnDrop(int index)
    {
        GameObject model = database.GetItem[itemDrops[index].itemID].model;
        Vector3 position = new Vector3(itemDrops[index].position[0], itemDrops[index].position[1], itemDrops[index].position[2]);
        Quaternion rotation = Quaternion.Euler(itemDrops[index].rotation[0], itemDrops[index].rotation[1], itemDrops[index].rotation[2]);
        itemObjs[index] = ObjectPoolManager.CreatePooled(model, position, rotation);
        itemObjs[index].transform.SetParent(itemDropParent);
        itemObjs[index].GetComponent<ItemInteractable>().enabled = true;
        itemObjs[index].GetComponent<Rigidbody>().isKinematic = false;
    }

    public void RemoveItem(GameObject itemObj)
    {
        int index = itemObjs.IndexOf(itemObj);
        if (index != -1)
        {
            itemDrops.RemoveAt(index);
            itemObjs.RemoveAt(index);
        }
    }

    public void LoadData(GameData data)
    {
        itemDrops = data.itemDropData.itemDrops;
        itemObjs = Enumerable.Repeat<GameObject>(null, itemDrops.Count).ToList();
    }

    public void SaveData(GameData data)
    {
        for (int i = 0; i < itemDrops.Count; i++)
        {
            if (itemObjs[i] != null)
            {
                Vector3 pos = itemObjs[i].transform.position;
                Vector3 rot = itemObjs[i].transform.rotation.eulerAngles;
                itemDrops[i].position = new float[] { pos.x, pos.y, pos.z };
                itemDrops[i].rotation = new float[] { rot.x, rot.y, rot.z };
            }
        }
        data.itemDropData.itemDrops = itemDrops;
    }
}
