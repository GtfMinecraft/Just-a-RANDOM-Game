using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDropHandler : MonoBehaviour, IDataPersistence
{
    public static ItemDropHandler instance;

    public Transform itemDropParent;

    private List<int> itemIDs;
    private List<ChunkTypes> chunks;
    private List<Vector3> position;
    private List<Vector3> rotation;

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
        for (int i = 0; i < itemIDs.Count; i++)
        {
            if (itemObjs[i] != null)
                ObjectPoolManager.DestroyPooled(itemObjs[i]);//gotta give each item their unique reference so that the itemObjs are distinguishable
        }
    }

    public void LoadChunk(ChunkTypes chunk)
    {
        for (int i = 0; i < itemIDs.Count; i++)
        {
            GameObject model = database.GetItem[itemIDs[i]].model;
            itemObjs[i] = ObjectPoolManager.CreatePooled(model, position[i], Quaternion.Euler(rotation[i]));
        }
    }

    public void SpawnDrop(int itemID, Vector3 position)
    {
        //be sure to add ItemInteractable
        //I think we can make every chunk (scene) have one and let PlayerItemController fetch according to name everytime?
        //but when multiple scenes are loaded, they will conflict
        //
        //or just use one instance, but save each scene in deparate files using dictionary?
    }

    public void LoadData(GameData data)
    {
        itemIDs = data.itemDropData.itemIDs;
        position = data.itemDropData.position.Select(o => new Vector3(o[0], o[1], o[2])).ToList();
        rotation = data.itemDropData.rotation.Select(o => new Vector3(o[0], o[1], o[2])).ToList();

        itemObjs = new List<GameObject>(itemIDs.Count);
    }

    public void SaveData(GameData data)
    {
        data.itemDropData.itemIDs = itemIDs;
        data.itemDropData.position = position.Select(o => new float[] {o.x, o.y, o.z}).ToList();
        data.itemDropData.rotation = rotation.Select(o => new float[] { o.x, o.y, o.z }).ToList();
    }
}
