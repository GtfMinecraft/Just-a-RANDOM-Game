using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public Item item;

    public override void Interact()
    {
        bool hasPickedUp = InventoryHandler.instance.inventoryList[(int)item.inventoryType].inventory.AddItem(item);
        if (!hasPickedUp)
        {
            //didn't pick up
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
