using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public int itemID;
    private float interactTime = 0.3f; // *** picking time has to be in sync with anim
    private bool isPicked = false;

    public override void Interact()
    {
        if (isPicked)
            return;

        bool hasPickedUp = InventoryHandler.instance.AddItem(itemID, false);
        if (!hasPickedUp)
        {
            Debug.Log("Inventory full");
            //didn't pick up - reasons: ex. inventory is full, ring glow red & magic error sound
            return;
        }
        else
        {
            isPicked = true;
            OnInteractionStart();
        }
    }

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        Invoke("OnInteractionEnd", interactTime);
    }

    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        isPicked = false;
        InventoryHandler.instance.AddItem(itemID);
        ItemDropHandler.instance.RemoveItem(gameObject);
    }
}
