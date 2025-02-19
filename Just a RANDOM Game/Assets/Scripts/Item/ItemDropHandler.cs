using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropHandler : MonoBehaviour, IDataPersistence
{
    public static ItemDropHandler instance;

    public Transform itemDropParent;
    public float[] dropForce = new float[2];
    public float maxForce;

    [System.Serializable]
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
                    ObjectPoolManager.DestroyPooled(itemObjs[i]);
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

    public void SpawnNewDrop(int itemID, ChunkTypes chunk, Vector3 position, bool random = true, bool towards = true)
    {
        itemDrops.Add(new ItemDrop {
            itemID = itemID,
            chunk = chunk,
            position = new float[] { position.x, position.y, position.z },
            rotation = new float[3]
        });
        itemObjs.Add(null);
        SpawnDrop(itemDrops.Count - 1);

        Vector3 force;
        if (random)
        {
            force = Random.onUnitSphere * Random.Range(dropForce[0], dropForce[1]);
            force.y = Mathf.Abs(force.y);
        }
        else
        {
            force = (PlayerController.instance.transform.position - position) * Random.Range(dropForce[0], dropForce[1]);
            if (!towards)
            {
                force.x = -force.x;
                force.z = -force.z;
            }
            force = Vector3.ClampMagnitude(force, maxForce);
        }
        itemObjs[^1].GetComponent<Rigidbody>().AddForce(force);

        //wait a bit and play add into inventory anim if auto pickup
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
