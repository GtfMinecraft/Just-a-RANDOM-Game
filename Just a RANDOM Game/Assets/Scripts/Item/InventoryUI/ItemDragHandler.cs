using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform itemHolderParent;

    protected int slotIndex;
    protected Inventory.ItemSlot itemSlot;
    [HideInInspector] 
    public Transform originalParent;
    private bool isDragging = false;

    public Inventory.ItemSlot GetItemSlot() { return itemSlot; }

    protected virtual void Start()
    {
        originalParent = transform.parent;
        slotIndex = transform.parent.GetSiblingIndex();
        gameObject.SetActive(false);
        itemHolderParent = InventoryCanvasController.instance.storage.transform;
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (eventData.button == PointerEventData.InputButton.Left && isDragging == false)
        {
            isDragging = true;
            originalParent = transform.parent;
            transform.SetParent(itemHolderParent);
            GetComponent<Image>().raycastTarget = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left && isDragging)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
        else
        {
            ReturnItem();
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            ReturnItem();

            GameObject hoveredObject = eventData.pointerCurrentRaycast.gameObject;
            if (hoveredObject == originalParent.gameObject)
            {
                InventoryHandler.instance.SelectItem(slotIndex);
            }
        }
    }

    public void ReturnItem()
    {
        isDragging = false;
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        GetComponent<Image>().raycastTarget = true;
    }
}
