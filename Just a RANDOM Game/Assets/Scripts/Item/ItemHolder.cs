using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : ScriptableObject
{
    public ItemDatabase database;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();

    [System.Serializable]
    public class ItemSlot
    {
        public int ID;
        public int defaultID;
        public int currentStack;

        public ItemSlot(int id, int defaultId, int stack) 
        {
            ID = id;
            defaultID = defaultId;
            currentStack = stack;
        }

        public void Clear()
        {
            ID = 0;
            currentStack = 0;
        }

        public bool SetNewItem(int setItemID)
        {
            if(defaultID != 0 && setItemID != defaultID)
            {
                return false;
            }
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
                InventoryHandler.instance.inventoryList[(int)database.GetItem[itemID].inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        for (int i=0; i<itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == 0 && itemSlots[i].SetNewItem(itemID))
            {
                InventoryHandler.instance.inventoryList[(int)database.GetItem[itemID].inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(int itemID, int count)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == itemID && itemSlots[i].currentStack >= count)
            {
                itemSlots[i].currentStack -= count;
                InventoryHandler.instance.inventoryList[(int)database.GetItem[itemID].inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        return false;
    }

    public void ClearAllItems()
    {
        for (int i = 0; i < itemSlots.Count; ++i)
        {
            itemSlots[i].Clear();
        }
    }
}
