using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public int itemID;

    public override void Interact()
    {
        bool hasPickedUp = InventoryHandler.instance.inventoryList[(int)PlayerItemController.instance.database.GetItem[itemID].inventoryType].inventory.AddItem(itemID);
        if (!hasPickedUp)
        {
            //didn't pick up - inventory is full
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
