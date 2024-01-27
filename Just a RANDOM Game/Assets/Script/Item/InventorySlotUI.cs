using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlotUI : SlotUI
{
    protected override void Start()
    {
        thisItemSlot = InventoryHandler.instance.currentInventory.itemSlots[slotIndex];
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if(eventData.pointerDrag.transform.parent == gameObject)
        {
            return;
        }

        HandleItemDrop(eventData);
    }
}
