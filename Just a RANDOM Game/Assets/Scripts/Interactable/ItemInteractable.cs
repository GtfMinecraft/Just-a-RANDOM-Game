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
            //didn't pick up - reasons: ex. inventory is full
            return;
        }
        else
        {
            isPicked = true;
            OnInteractionStart();
        }
    }

    private void OnInteractionStart()
    {
        anim.SetInteger("PlayerAction", 2);
        Invoke("OnInteractionEnd", interactTime);
    }

    private void OnInteractionEnd()
    {
        if (anim.GetInteger("PlayerAction") == 2)
            anim.SetInteger("PlayerAction", 0);
        InventoryHandler.instance.AddItem(itemID);
        ItemDropHandler.instance.RemoveItem(gameObject);
        isPicked = false;
        ObjectPoolManager.DestroyPooled(gameObject);
    }
}
