using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDragHandler : ItemDragHandler
{
    protected override void DropFromSlot()
    {
        //InventoryHandler.instance.inventoryList[inventoryHandler.currentGroup].DropItem(slotIndex);
    }

    public void UpdateItemSlot(Inventory.ItemSlot slot)
    {
        itemSlot = slot;
    }
}
