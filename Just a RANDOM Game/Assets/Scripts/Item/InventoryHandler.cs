using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour, IDataPersistence
{
    public static InventoryHandler instance;

    public Inventory[] inventoryList = new Inventory[(int)InventoryTypes.food + 1];

    [Header("Resource")]
    public int crystals;
    public TMP_Text crystalText;
    [UDictionary.Split(30, 70)]
    public UDictionaryIntInt resources;

    [Header("UI")]
    public Transform groupUI;
    public Transform storage;
    public Transform armor;
    public Transform description;
    public ItemWheelUI itemWheel;

    private Image[] groups = new Image[7];
    private InventorySlotUI[] inventorySlots;

    public int currentGroup { get; private set; }
    private ItemDatabase database;

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

    private void Start()
    {
        database = PlayerItemController.instance.database;
        currentGroup = PlayerPrefs.GetInt("selectedGroup", 0);

        groupUI.GetChild(currentGroup).GetComponent<Button>().Select();

        for(int i=0;i<groups.Length;i++)
        {
            groups[i] = groupUI.GetChild(i).GetComponent<Image>();
        }

        inventorySlots = new InventorySlotUI[storage.childCount];
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = storage.GetChild(i).GetComponent<InventorySlotUI>();
        }
    }

    public bool AddItem(int itemID)
    {
        int invType = (int)database.GetItem[itemID].inventoryType;
        bool canAdd = inventoryList[invType].AddItem(itemID);
        if (canAdd)
        {
            if (resources.ContainsKey(itemID))
            {
                resources[itemID]++;
            }
            else
            {
                resources[itemID] = 1;
            }
            PlayerItemController.instance.SetDefaultItem(itemID);
            itemWheel.UpdateItemWheelUI(itemID);

            if(invType == currentGroup)
            {
                UpdateInventoryUI();
            }
            return true;
        }
        return false;
    }

    public bool RemoveItem(int itemID, int count)
    {
        if (resources[itemID] < count)
        {
            return false;
        }

        resources[itemID] -= count;

        int invType = (int)database.GetItem[itemID].inventoryType;

        inventoryList[invType].RemoveItem(itemID, count);
        itemWheel.UpdateItemWheelUI(itemID);

        if (invType != currentGroup)
        {
            UpdateInventoryUI();
        }
        return true;
    }

    public void SwitchGroup(int group)
    {
        currentGroup = group;
        ResetDraggingUI();
        UpdateInventoryUI();
    }

    public void ResetDraggingUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].icon.GetComponent<InventoryDragHandler>().ReturnItem();
        }
    }

    public void UpdateInventoryUI()
    {
        if(InterfaceHandler.instance.currentInterface != Interfaces.storage)
        {
            return;
        }

        for(int i = 0; i < inventorySlots.Length; ++i)
        {
            inventorySlots[i].UpdateInventorySlot(currentGroup);
        }

        crystalText.text = crystals.ToString();

        // update armor ui
    }

    public void LoadData(GameData data)
    {
        for(int i = 0; i < inventoryList.Length; i++)
        {
            inventoryList[i].itemSlots = data.inventoryData[i].itemIDs.Zip(
                data.inventoryData[i].currentStacks, (f1, f2) => 
                {
                    if (resources.ContainsKey(f1))
                    {
                        resources[f1] += f2;
                    }
                    else
                    {
                        resources[f1] = f2;
                    }
                    return new Inventory.ItemSlot(f1, f2);
                }).ToList();
        }
    }

    public void SaveData(GameData data)
    {
        data.inventoryData.Clear();
        foreach(var invList in inventoryList)
        {
            data.inventoryData.Add(new GameData.InventoryData { 
                itemIDs = invList.itemSlots.ConvertAll(o => o.ID),
                currentStacks = invList.itemSlots.ConvertAll(((o) => o.currentStack)),
            });
        }

        PlayerPrefs.SetInt("selectedTool", (int)PlayerItemController.instance.currentInventory);
        PlayerPrefs.SetInt("selectedGroup", currentGroup);

        string rightItemsString = "", leftItemsString = "";
        for (int i = 0; i < PlayerItemController.instance.rightItems.Length; i++)
        {
            rightItemsString += PlayerItemController.instance.rightItems[i];
            leftItemsString += PlayerItemController.instance.leftItems[i];
        }
        PlayerPrefs.SetString("rightItemsString", rightItemsString);
        PlayerPrefs.SetString("leftItemsString", leftItemsString);
    }
}
