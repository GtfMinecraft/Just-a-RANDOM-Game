using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDragHandler : ItemDragHandler
{
    public void UpdateItemSlot(Inventory.ItemSlot slot)
    {
        itemSlot = slot;
    }
}
