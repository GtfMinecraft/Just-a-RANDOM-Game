using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IDropHandler
{
    [HideInInspector]
    public GameObject icon;
    protected int slotIndex;
    private TMP_Text stack;

    protected ItemDatabase database;
    protected Inventory.ItemSlot thisItemSlot;
    protected Inventory.ItemSlot droppedItemSlot;

    protected virtual void Start()
    {
        database = PlayerItemController.instance.database;
        slotIndex = transform.GetSiblingIndex();
        icon = transform.GetChild(0).gameObject;
        stack = icon.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            droppedItemSlot = eventData.pointerDrag.GetComponent<ItemDragHandler>().GetItemSlot();
        }
    }

    protected void HandleItemDrop(PointerEventData eventData)
    {
        if (thisItemSlot.ID != 0)
        {
            int maxStack = PlayerItemController.instance.database.GetItem[thisItemSlot.ID].maxStack;
            if (thisItemSlot.ID == droppedItemSlot.ID && thisItemSlot.currentStack < maxStack)
            {
                if (droppedItemSlot.currentStack <= maxStack - thisItemSlot.currentStack)
                {
                    thisItemSlot.currentStack += droppedItemSlot.currentStack;
                    droppedItemSlot.Clear();
                }
                else
                {
                    int stackDifference = maxStack - thisItemSlot.currentStack;
                    thisItemSlot.currentStack += stackDifference;
                    droppedItemSlot.currentStack -= stackDifference;
                }
            }
            else
            {
                SwapItems(eventData);
            }
        }
        else
        {
            DropOntoEmptySlot();
        }
        InventoryHandler.instance.UpdateInventoryUI();
    }

    private void DropOntoEmptySlot()
    {
        thisItemSlot.ID = droppedItemSlot.ID;
        thisItemSlot.currentStack = droppedItemSlot.currentStack;
        droppedItemSlot.Clear();
    }

    protected virtual void SwapItems(PointerEventData eventData)
    {
        int tempID = thisItemSlot.ID;
        int tempCurrentStack = thisItemSlot.currentStack;

        thisItemSlot.ID = droppedItemSlot.ID;
        thisItemSlot.currentStack = droppedItemSlot.currentStack;

        droppedItemSlot.ID = tempID;
        droppedItemSlot.currentStack = tempCurrentStack;
    }

    protected void RefreshItemIcons()
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
        else
        {
            icon.SetActive(false);
        }
    }
}
