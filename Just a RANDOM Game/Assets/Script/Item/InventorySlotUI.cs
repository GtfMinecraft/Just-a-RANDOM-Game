using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : SlotUI
{
    protected override void Start()
    {
        base.Start();
        thisItemSlot = InventoryHandler.instance.inventoryList[inventoryType].inventory.itemSlots[slotIndex];
    }

    //public override void OnDrop(PointerEventData eventData)
    //{
    //    if (eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        base.OnDrop(eventData);
    //        if (eventData.pointerDrag.transform.parent == gameObject)
    //        {
    //            return;
    //        }

    //        HandleItemDrop(eventData);
    //    }
    //}
}
