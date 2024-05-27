using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    protected int inventoryType;
    protected GameObject icon;
    protected int slotIndex;
    private TMP_Text stack;

    protected ItemDatabase database;
    protected ItemHolder.ItemSlot thisItemSlot;
    //protected ItemHolder.ItemSlot droppedItemSlot;

    protected virtual void Start()
    {
        database = PlayerItemController.instance.database;
        inventoryType = transform.parent.parent.GetSiblingIndex();
        slotIndex = transform.GetSiblingIndex();
        icon = transform.GetChild(0).gameObject;
        stack = icon.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    //public virtual void OnDrop(PointerEventData eventData)
    //{
    //    if (eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        droppedItemSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().GetItemSlot();
    //    }
    //}

    //protected void HandleItemDrop(PointerEventData eventData)
    //{
    //    if(thisItemSlot.item != null)
    //    {
    //        if(thisItemSlot.item == droppedItemSlot.item && thisItemSlot.currentStack < thisItemSlot.item.maxStack)
    //        {
    //            if(droppedItemSlot.currentStack <= this.thisItemSlot.item.maxStack - thisItemSlot.currentStack)
    //            {
    //                thisItemSlot.currentStack += droppedItemSlot.currentStack;
    //                droppedItemSlot.Clear();
    //            }
    //            else
    //            {
    //                int stackDifference = thisItemSlot.item.maxStack - thisItemSlot.currentStack;
    //                thisItemSlot.currentStack += stackDifference;
    //                droppedItemSlot.currentStack -= stackDifference;
    //            }
    //        }
    //        else
    //        {
    //            SwapItems(eventData);
    //        }
    //    }
    //    else
    //    {
    //        DropOntoEmptySlot();
    //    }
    //    InventoryHandler.instance.UpdateInventoryUI();
    //}

    //private void DropOntoEmptySlot()
    //{
    //    thisItemSlot.item = droppedItemSlot.item;
    //    thisItemSlot.currentStack = droppedItemSlot.currentStack;
    //    droppedItemSlot.Clear();
    //}

    //protected virtual void SwapItems(PointerEventData eventData)
    //{
    //    ItemHolder.ItemSlot tempSlot = new ItemHolder.ItemSlot(thisItemSlot.item, thisItemSlot.currentStack);

    //    thisItemSlot.item = droppedItemSlot.item;
    //    thisItemSlot.currentStack = droppedItemSlot.currentStack;

    //    droppedItemSlot.item = tempSlot.item;
    //    droppedItemSlot.currentStack = tempSlot.currentStack;
    //}

    public void RefreshItemIcons()
    {
        if(thisItemSlot.ID != 0)
        {
            icon.GetComponent<Image>().sprite = database.GetItem[thisItemSlot.ID].icon;
            if(thisItemSlot.currentStack > 1)
            {
                stack.text = thisItemSlot.currentStack.ToString();
            }
            else
            {
                stack.text = "";
            }
            icon.SetActive(true);
        }
        else if (thisItemSlot.defaultID != 0)
        {
            icon.GetComponent<Image>().sprite = database.GetItem[thisItemSlot.defaultID].blackIcon;
            stack.text = "";
            icon.SetActive(true);
        }
        else
        {
            icon.SetActive(false);
        }
    }
}
