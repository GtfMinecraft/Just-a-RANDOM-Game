using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public static InventoryHandler instance;

    public Transform inventoryCanvas;

    [System.Serializable]
    public class InventoryPack
    {
        public Inventory inventory;
        private Transform inventoryHolder;
        private InventorySlotUI[] inventorySlots;

        public void Initialize(int inventoryType)
        {
            inventory.ClearAllItems();

            //replace clear all items with reading saved inventory data

            inventoryHolder = instance.inventoryCanvas.GetChild(inventoryType).GetChild(0);

            inventorySlots = new InventorySlotUI[inventory.itemSlots.Length];
            for (int i = 0; i < inventory.itemSlots.Length; ++i)
            {
                inventorySlots[i] = inventoryHolder.GetChild(i).GetComponent<InventorySlotUI>();
            }
        }

        public void UpdateInventoryUI()
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].RefreshItemIcons();
            }
        }
    }

    public InventoryPack[] inventoryList = new InventoryPack[6];

    /*
     *  0 storage
     *  1 weapon
     *  2 axe
     *  3 pickaxe
     *  4 hoe
     *  5 rod
    */

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
        for (int i = 0; i < inventoryList.Length; ++i)
        {
            inventoryList[i].Initialize(i);
        }
    }
}
