using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public Item[] items;
    public Dictionary<int, Item> GetItem = new Dictionary<int, Item>();

    public void OnAfterDeserialize()
    {
        GetItem = new Dictionary<int, Item>();

        for(int i=0; i<items.Length; i++)
        {
            if (items[i] == null)
            {
                continue;
            }

            if (items[i].ID == 0)
            {
                Debug.LogError("Wrong ID, cannot set an item ID to be zero as it is the null item");
                continue;
            }
            var item = GetItem.FirstOrDefault(o => o.Key == items[i].ID).Value;
            if (item != null && item.ID != items[i].ID)
            {
                Debug.LogError("Repeated item ID, please set a valid item ID");
                continue;
            }
            else if(item == null)
            {
                GetItem.Add(items[i].ID, items[i]);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
}
