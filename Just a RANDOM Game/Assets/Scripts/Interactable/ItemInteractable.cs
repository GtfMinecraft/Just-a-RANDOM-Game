using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public int itemID;
    private float interactTime = 0.3f; // *** picking time has to be in sync with anim

    public override void Interact()
    {
        bool hasPickedUp = InventoryHandler.instance.AddItem(itemID, false);
        if (!hasPickedUp)
        {
            Debug.Log("Inventory full");
            //didn't pick up - reasons: ex. inventory is full
            return;
        }
        else
        {
            StartInteraction();
        }
    }

    public void StartInteraction()
    {
        anim.SetInteger("PlayerAction", 2);
        Invoke("OnInterationComplete", interactTime);
    }

    protected override void OnInterationComplete()
    {
        if (anim.GetInteger("PlayerAction") == 2)
            anim.SetInteger("PlayerAction", 0);
        InventoryHandler.instance.AddItem(itemID);
        base.OnInterationComplete();
        Destroy(gameObject);
    }
}
