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

    public Inventory[] inventoryList = new Inventory[(int)InventoryTypes.Food + 1];

    [UDictionary.Split(30, 70)]
    public UDictionaryIntInt resources;

    [Header("UI")]
    //public Transform groupUI;
    public Transform storage;
    public GameObject slotPrefab;
    public Transform armor;
    public Transform itemInfo;
    public ItemWheelUI itemWheel;

    public float dropItemSpeed;
    [HideInInspector]
    public int hoveredSlot = -1;

    //private Image[] groups = new Image[7];
    private InventorySlotUI[] inventorySlots;
    private int slotCount;
    private bool dropAll = false;

    public int currentGroup { get; private set; }
    private ItemDatabase database;

    public int selectedIndex { get; private set; } = -1;
    public Transform crystalSelected;

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
        currentGroup = 0;//for demo

        //groupUI.GetChild(currentGroup).GetComponent<Button>().Select();

        //for(int i=0;i<groups.Length;i++)
        //{
        //    groups[i] = groupUI.GetChild(i).GetComponent<Image>();
        //}

        for (int i = 0; i < slotCount; ++i)
        {
            GameObject slot = Instantiate(slotPrefab, storage);
            int remainder = i % 9;
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(97.7f + (remainder % 5) * 129.6f + (remainder <= 4 ? 0 : 64.8f), 946 - (i / 9) * 162.06f - (remainder <= 4 ? 0 : 81.03f));
        }

        inventorySlots = new InventorySlotUI[storage.childCount];
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = storage.GetChild(i).GetComponent<InventorySlotUI>();
        }
    }

    public void SelectItem(int index)
    {
        itemInfo.gameObject.SetActive(true);
        selectedIndex = index;
        Item item = database.GetItem[inventoryList[currentGroup].itemSlots[index].ID];

        TMP_Text description = itemInfo.GetChild(0).GetComponent<TMP_Text>();
        description.text = item.itemDescription + "\n\n";

        string attackPowerText = $"<color=#FF0000>Attack Power</color>\t----------------\t<color=#FF0000>{item.damage}</color>\n";
        string speedText = $"<color=#00FF00>Speed</color>\t----------------------\t<color=#00FF00>{item.attackSpeed}</color>\n";

        description.text += attackPowerText + speedText;

        itemInfo.GetChild(1).GetComponent<TMP_Text>().text = item.itemName;
        itemInfo.GetChild(2).GetComponent<Image>().sprite = item.icon;

        crystalSelected.gameObject.SetActive(true);
        crystalSelected.position = inventorySlots[index].transform.position;
    }

    public bool AddItem(int itemID, bool addItem = true)
    {
        int invType = (int)database.GetItem[itemID].inventoryType;
        bool canAdd = inventoryList[invType].AddItem(itemID, addItem);

        if (!addItem)
        {
            return canAdd;
        }

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
            InteractablePromptController.instance.AddDrop(itemID);
            PlayerItemController.instance.UpdateHandModel();
            //itemWheel.UpdateItemWheelUI(itemID);

            if(invType == currentGroup)
            {
                UpdateInventoryUI();
            }
            return true;
        }
        return false;
    }

    public bool RemoveItem(int itemID, int count = 1, int slotIndex = -1)
    {
        if (resources[itemID] < count)
        {
            return false;
        }

        resources[itemID] -= count;

        int invType = (int)database.GetItem[itemID].inventoryType;

        if (slotIndex == -1)
            inventoryList[invType].RemoveItem(itemID, count);
        else
        {
            inventoryList[invType].itemSlots[slotIndex].currentStack -= count;
            if(inventoryList[invType].itemSlots[slotIndex].currentStack == 0)
            {
                inventoryList[invType].itemSlots[slotIndex].ID = 0;
            }
        }
        PlayerItemController.instance.UpdateHandModel();
        //itemWheel.UpdateItemWheelUI(itemID);

        if (invType == currentGroup)
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
        if(InterfaceHandler.instance.currentInterface != Interfaces.Storage)
        {
            return;
        }

        for(int i = 0; i < inventorySlots.Length; ++i)
        {
            inventorySlots[i].UpdateInventorySlot(currentGroup);
        }

        if (selectedIndex == -1 || inventoryList[currentGroup].itemSlots[selectedIndex].ID == 0)
        {
            selectedIndex = -1;
            itemInfo.gameObject.SetActive(false);
            crystalSelected.gameObject.SetActive(false);
        }

        // update armor ui
    }

    public void DropItemHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && hoveredSlot != -1)
        {
            if (ctx.action.name == "Drop Item")
            {
                DropItem();
                InvokeRepeating("DropItem", 0.4f, dropItemSpeed);
            }
            else
            {
                CancelInvoke("DropItem");
                dropAll = true;
                DropItem();
            }
        }
        else if (ctx.canceled)
        {
            if(ctx.action.name == "Drop Item")
                CancelInvoke("DropItem");
            dropAll = false;
        }
    }

    private void DropItem()
    {
        if (hoveredSlot == -1 || inventoryList[currentGroup].itemSlots[hoveredSlot].currentStack == 0)
            return;

        int itemID = inventoryList[currentGroup].itemSlots[hoveredSlot].ID;
        int count = dropAll ? inventoryList[currentGroup].itemSlots[hoveredSlot].currentStack : 1;
        RemoveItem(itemID, count, hoveredSlot);
        //vfx
        Vector3 position = PlayerController.instance.transform.position + PlayerController.instance.playerObj.forward;
        for(int i = 0; i < count; i++)
            ItemDropHandler.instance.SpawnNewDrop(itemID, position, ChunkLoadingController.instance.currentChunk, false, false);
    }

    public void LoadData(GameData data)
    {
        slotCount = data.inventoryData[0].itemIDs.Count;// future function to implement: dynamically adjust inventory space to add 1 row

        for (int i = 0; i < data.inventoryData.Count; i++)
        {
            //inventoryList[i].itemSlots = data.inventoryData[i].itemIDs.Zip(
            //    data.inventoryData[i].currentStacks.Zip(data.inventoryData[i].elements, (f1, f2) => new {stacks = f1, elements = f2}), (f1, f2) =>
            //    {
            //        if (resources.ContainsKey(f1))
            //        {
            //            resources[f1] += f2.stacks;
            //        }
            //        else
            //        {
            //            resources[f1] = f2.stacks;
            //        }

            //        int[] elements = f2.elements.ToCharArray().Select(c => c - 48).ToArray();

            //        return new Inventory.ItemSlot(f1, f2.stacks, elements);
            //    }).ToList();

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
                currentStacks = invList.itemSlots.ConvertAll(o => o.currentStack),
            });
        }

        PlayerPrefs.SetInt("selectedTool", (int)PlayerItemController.instance.currentInventory);
        PlayerPrefs.SetInt("selectedGroup", currentGroup);

        //string rightItemsString = "", leftItemsString = "";
        //for (int i = 0; i < PlayerItemController.instance.rightItems.Length; i++)
        //{
        //    rightItemsString += PlayerItemController.instance.rightItems[i];
        //    leftItemsString += PlayerItemController.instance.leftItems[i];
        //}
        //PlayerPrefs.SetString("rightItemsString", rightItemsString);
        //PlayerPrefs.SetString("leftItemsString", leftItemsString);
    }
}
