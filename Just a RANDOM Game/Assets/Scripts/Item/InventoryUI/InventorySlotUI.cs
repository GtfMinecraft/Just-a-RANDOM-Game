using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : SlotUI, IPointerEnterHandler, IPointerExitHandler
{
    private Image hoverTint;

    protected override void Start()
    {
        base.Start();
        icon.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;

        hoverTint = transform.GetChild(1).GetComponent<Image>();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            base.OnDrop(eventData);
            if (eventData.pointerDrag.transform.parent.gameObject == gameObject)
            {
                return;
            }

            HandleItemDrop(eventData);

            if(eventData.pointerDrag.transform.parent.GetSiblingIndex() == InventoryHandler.instance.selectedIndex)
                InventoryHandler.instance.SelectItem(slotIndex);
            else if(slotIndex == InventoryHandler.instance.selectedIndex)
                InventoryHandler.instance.SelectItem(eventData.pointerDrag.transform.parent.GetSiblingIndex());

            InventoryHandler.instance.UpdateInventoryUI();
        }
    }

    public void UpdateInventorySlot(int inv)
    {
        thisItemSlot = InventoryHandler.instance.inventoryList[inv].itemSlots[slotIndex];
        icon.GetComponent<InventoryDragHandler>().UpdateItemSlot(thisItemSlot);

        RefreshItemIcons();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTint.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTint.enabled = false;
    }
}
