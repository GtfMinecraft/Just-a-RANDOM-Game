using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDragHandler : ItemDragHandler
{
    private InventoryHandler inventoryHandler;

    protected override void Start()
    {
        base.Start();
        inventoryHandler = InventoryHandler.instance;
    }

    //throw out of inventory + maybe add Q key
    protected override void DropFromSlot()
    {
        //inventoryHandler.inventoryList[inventoryHandler.currentGroup].DropItem(slotIndex);
    }

    public void UpdateItemSlot(Inventory.ItemSlot slot)
    {
        itemSlot = slot;
    }
}
