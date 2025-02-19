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
        //public int[] elements = new int[5];

        //public ItemSlot(int id, int stack, int[] elements)
        //{
        //    ID = id;
        //    currentStack = stack;
        //    this.elements = elements;
        //}

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

        public void SetNewItem(int setItemID)
        {
            ID = setItemID;
            currentStack = 1;
        }
    }

    public bool AddItem(int itemID, bool addItem = true)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == itemID && itemSlots[i].currentStack < database.GetItem[itemSlots[i].ID].maxStack)
            {
                if(addItem)
                    itemSlots[i].currentStack++;
                return true;
            }
        }
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].ID == 0)
            {
                if (addItem)
                    itemSlots[i].SetNewItem(itemID);
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
                if (itemSlots[i].currentStack > count)
                {
                    itemSlots[i].currentStack -= count;
                    return;
                }
                else
                {
                    count -= itemSlots[i].currentStack;
                    itemSlots[i].ID = itemSlots[i].currentStack = 0;
                    if (count == 0) return;
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
