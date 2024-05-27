using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryHandler : MonoBehaviour, IDataPersistence
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
            inventoryHolder = instance.inventoryCanvas.GetChild(inventoryType).GetChild(0);

            inventorySlots = new InventorySlotUI[inventory.itemSlots.Count];
            for (int i = 0; i < inventory.itemSlots.Count; ++i)
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

    public InventoryPack[] inventoryList = new InventoryPack[(int)InventoryTypes.food + 1];

    /*
     *  0 storage
     *  1 weapon
     *  2 axe
     *  3 pickaxe
     *  4 hoe
     *  5 rod
     *  6 food
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

    public void LoadData(GameData data)
    {
        for(int i = 0; i < inventoryList.Length; i++)
        {
            inventoryList[i].inventory.itemSlots = data.inventoryData[i].itemIDs.Zip(
                data.inventoryData[i].defaultItemIDs, (f1, f2) => new { f1, f2 })
                .Zip(data.inventoryData[i].currentStacks, (f12, f3) => 
                new ItemHolder.ItemSlot(f12.f1, f12.f2, f3)).ToList();
            inventoryList[i].Initialize(i);
        }
    }

    public void SaveData(GameData data)
    {
        data.inventoryData.Clear();
        foreach(var invList in inventoryList)
        {
            data.inventoryData.Add(new GameData.InventoryData { 
                itemIDs = invList.inventory.itemSlots.ConvertAll(o => o.ID), 
                defaultItemIDs = invList.inventory.itemSlots.ConvertAll((o) => o.defaultID),
                currentStacks = invList.inventory.itemSlots.ConvertAll(((o) => o.currentStack)),
            });
        }
    }
}
