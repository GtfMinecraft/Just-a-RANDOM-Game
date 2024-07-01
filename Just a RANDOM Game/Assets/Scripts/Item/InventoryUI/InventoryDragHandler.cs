using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDragHandler : ItemDragHandler
{
    //throw out of inventory + maybe add Q key
    protected override void DropFromSlot()
    {
        //InventoryHandler.instance.inventoryList[inventoryHandler.currentGroup].DropItem(slotIndex);
    }

    public void UpdateItemSlot(Inventory.ItemSlot slot)
    {
        itemSlot = slot;
    }
}
