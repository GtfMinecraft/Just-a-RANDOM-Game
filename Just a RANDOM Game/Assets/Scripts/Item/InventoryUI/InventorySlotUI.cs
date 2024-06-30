using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : SlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnDrop(eventData);
            if (eventData.pointerDrag.transform.parent == gameObject)
            {
                return;
            }

            HandleItemDrop(eventData);
        }
    }

    public void UpdateInventorySlot(int inv)
    {
        thisItemSlot = InventoryHandler.instance.inventoryList[inv].itemSlots[slotIndex];
        transform.GetChild(0).GetComponent<InventoryDragHandler>().UpdateItemSlot(thisItemSlot);

        RefreshItemIcons();
    }
}
