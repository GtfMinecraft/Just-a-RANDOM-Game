using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public ItemDatabase database;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();

    [System.Serializable]
    public class ItemSlot
    {
        public int ID;
        public int currentStack;

        public ItemSlot(int id, int stack)
        {
            ID = id;
            currentStack = stack;
        }

        public void Clear()
        {
            ID = 0;
            currentStack = 0;
        }

        public bool SetNewItem(int setItemID)
        {
            ID = setItemID;
            currentStack = 1;
            return true;
        }
    }

    public bool AddItem(int itemID)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == itemID && itemSlots[i].currentStack < database.GetItem[itemSlots[i].ID].maxStack)
            {
                itemSlots[i].currentStack++;
                return true;
            }
        }
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == 0 && itemSlots[i].SetNewItem(itemID))
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(int itemID, int count)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == itemID)
            {
                count -= itemSlots[i].currentStack;
                if (count <= 0) return;
                else
                {
                    itemSlots[i].ID = 0;
                }
            }
        }
    }

    public void ClearAllItems()
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            itemSlots[i].Clear();
        }
    }
}
