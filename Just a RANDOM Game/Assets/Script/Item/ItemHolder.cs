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

        public void SetNewItem(Item setItem)
        {
            item = setItem;
            currentStack = 1;
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
            if (itemSlots[i].item == null)
            {
                itemSlots[i].SetNewItem(item);
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
