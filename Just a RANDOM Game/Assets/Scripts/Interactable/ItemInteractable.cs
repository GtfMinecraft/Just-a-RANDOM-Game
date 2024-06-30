using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public int itemID;

    public override void Interact()
    {
        bool hasPickedUp = InventoryHandler.instance.AddItem(itemID);
        if (!hasPickedUp)
        {
            Debug.Log("Inventory full");
            //didn't pick up - reasons: ex. inventory is full
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
