using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropHandler : MonoBehaviour, IDataPersistence
{
    public static ItemDropHandler instance;

    public Transform itemDropParent;

    private List<int> itemIDs;
    private List<ChunkTypes> chunks;
    private List<Vector3> positions;

    private List<GameObject> itemObjs;

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
        //load items in loaded chunks
    }

    public void UnloadChunk(ChunkTypes chunk)
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i] == chunk)
            {
                Destroy(itemObjs[i]);//gotta give each item their unique reference so that the itemObjs are distinguishable
            }
        }
    }

    public void LoadChunk(ChunkTypes chunk)
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i] == chunk)
            {
                GameObject itemObj = Instantiate(itemObjs[i], itemDropParent);
                itemObjs[i] = itemObj;
            }
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
        chunks = data.itemDropData.chunks;
    }

    public void SaveData(GameData data)
    {
        data.itemDropData.itemIDs = itemIDs;
        data.itemDropData.chunks = chunks;
    }
}
