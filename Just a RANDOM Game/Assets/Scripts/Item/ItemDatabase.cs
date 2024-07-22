using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("Item names and IDs")]
    public List<ItemAndID> items;
    public Dictionary<int, Item> GetItem = new Dictionary<int, Item>();

    [Serializable]
    public struct ItemAndID
    {
        public Item item;
        public int ID;
    }

    public void OnAfterDeserialize()
    {
        GetItem = new Dictionary<int, Item>();

        if(items == null)
        {
            return;
        }

        for(int i=0; i<items.Count; i++)
        {
            if (items[i].item == null)
            {
                continue;
            }

            var checkItem = GetItem.FirstOrDefault(o => o.Value == items[i].item);
            if (GetItem.TryGetValue(items[i].ID, out Item item))
            {
                Debug.LogError($"Item Database repeated item ID, please set a valid item ID. Item ID: {items[i].ID}");
                continue;
            }
            else if(!checkItem.Equals(default(KeyValuePair<int, Item>)))
            {
                Debug.LogError($"Item Database repeated item, please only set each item once. Item ID: {checkItem.Value.ID} and {items[i].ID}");
                continue;
            }
            else if (item == null)
            {
                GetItem.Add(i, items[i].item);
                items[i].item.ID = items[i].ID;
            }
        }
    }

    public void OnBeforeSerialize()
    {
        
    }
}
