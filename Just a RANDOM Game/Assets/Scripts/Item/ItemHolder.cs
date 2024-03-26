using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : ScriptableObject
{
    public ItemSlot[] itemSlots;

    [System.Serializable]
    public class ItemSlot
    {
        public Item item;
        public Item defaultItem;
        public int currentStack;

        public ItemSlot(Item setItem, int stack) 
        {
            item = setItem;
            currentStack = stack;
        }

        public void Clear()
        {
            item = null;
            currentStack = 0;
        }

        public bool SetNewItem(Item setItem)
        {
            if(defaultItem != null && setItem != defaultItem)
            {
                return false;
            }
            item = setItem;
            currentStack = 1;
            return true;
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item && itemSlots[i].currentStack < itemSlots[i].item.maxStack)
            {
                itemSlots[i].currentStack++;
                InventoryHandler.instance.inventoryList[(int)item.inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        for (int i=0; i<itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null && itemSlots[i].SetNewItem(item))
            {
                InventoryHandler.instance.inventoryList[(int)item.inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Item item, int count)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item && itemSlots[i].currentStack >= count)
            {
                itemSlots[i].currentStack -= count;
                InventoryHandler.instance.inventoryList[(int)item.inventoryType].UpdateInventoryUI();
                return true;
            }
        }
        return false;
    }

    public void ClearAllItems()
    {
        for (int i = 0; i < itemSlots.Length; ++i)
        {
            itemSlots[i].Clear();
        }
    }
}
