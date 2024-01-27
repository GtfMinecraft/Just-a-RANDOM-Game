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

    private float xInterval = -1520;
    private float yInterval = -530;

    public ItemHolder.ItemSlot GetItemSlot() { return itemSlot; }

    protected virtual void Start()
    {
        slotIndex = transform.parent.GetSiblingIndex();
    }

    public void OnPointerDown(PointerEventData eventData) 
    {
        isDragging = true;
        originalParent = transform.parent;
        transform.SetParent(itemHolderParent);
        GetComponent<Image>().raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.localPosition = new Vector3(Input.mousePosition.x+xInterval, Input.mousePosition.y+yInterval, 0);
        }
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            GetComponent<Image>().raycastTarget = false;
        }
    }

    protected virtual void DropFromSlot()
    {

    }
}
