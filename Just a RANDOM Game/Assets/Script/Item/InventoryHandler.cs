using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public static InventoryHandler instance;

    public Inventory currentInventory;

    private GameObject inventoryPanel;
    private InventorySlotUI[] inventorySlotUIs;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        inventorySlotUIs = FindObjectsOfType<InventorySlotUI>();
        inventoryPanel = transform.GetChild(0).gameObject;
    }

    public void UpdateInventoryUI()
    {
        for(int i=0;i<inventorySlotUIs.Length;i++)
        {
            inventorySlotUIs[i].RefreshItemIcons();
        }
    }
}
