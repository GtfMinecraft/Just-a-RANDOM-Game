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
    protected ItemHolder.ItemSlot itemSlot;
    private Transform originalParent;
    private bool isDragging;

    public ItemHolder.ItemSlot GetItemSlot() { return itemSlot; }

    protected virtual void Start()
    {
        slotIndex = transform.parent.GetSiblingIndex();
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            originalParent = transform.parent;
            transform.SetParent(itemHolderParent);
            GetComponent<Image>().raycastTarget = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            GetComponent<Image>().raycastTarget = true;
        }
    }

    protected virtual void DropFromSlot()
    {

    }
}
