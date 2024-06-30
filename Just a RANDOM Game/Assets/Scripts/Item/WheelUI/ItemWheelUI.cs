using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemWheelUI : WheelUI
{
    public float freeDistance;
    public ItemWheel[] itemWheels = new ItemWheel[6];

    private InventoryTypes inventoryType;

    private int currentItem = 0;//default item if none is selected
    private int currentItem2 = 0;

    private List<Image>[] itemImages = new List<Image>[6];
    private TextMeshProUGUI[] stacks = new TextMeshProUGUI[6];

    [CreateAssetMenu(fileName = "New Item Wheel", menuName = "Inventory/Item Wheel")]
    public class ItemWheel : ScriptableObject
    {
        public List<int> itemID = new List<int>();
        public int[] itemsPerSection = new int[6];

        public int ItemCount()
        {
            int count = 0;
            UDictionaryIntInt resources = InventoryHandler.instance.resources;

            foreach (int item in itemID)
            {
                if (resources.ContainsKey(item))
                {
                    ++count;
                }
            }
            return count;
        }

        public int getSection(int item)
        {
            int section = itemID.IndexOf(item);
            if(section == -1)
            {
                return -1;
            }

            section++;

            for(int i=0; i < itemsPerSection.Length; ++i)
            {
                if (section <= itemsPerSection[i])
                {
                    return i;
                }
                section -= itemsPerSection[i];
            }

            return -1;
        }
    }

    private void Start()
    {
        //for (int i = 0; i < 6; i++)
        //{
        //    for (int j = 0; j < itemImages.Length; j++)
        //    {
        //        itemImages[i] = transform.GetChild(i).GetComponent<Image>();
        //        itemSelected[i] = itemImages[i].transform.GetChild(1).GetComponent<Image>();
        //        itemSelected[i].enabled = false;
        //    }
        //}

        //currentItem = PlayerPrefs.GetInt("selectedTool");
        //if (currentItem != 0)
        //{
        //    itemSelected[currentItem - 1].enabled = true;
        //}
    }

    public void SwapTool()
    {
        //int section = GetSection(-60, 60, 6, freeDistance);
        //if (section == -1)
        //{
        //    return;
        //}
        //if (currentTool != section + 1)
        //{
        //    if (currentTool != 0)
        //    {
        //        toolSelected[currentTool - 1].enabled = false;
        //    }
        //    toolSelected[section].enabled = true;
        //    currentTool = section + 1;
        //    PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        //}
        //else
        //{
        //    toolSelected[section].enabled = false;
        //    currentTool = 0;
        //    PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        //}
    }

    public void RefreshItemWheel(int item = 0)
    {
        if(InterfaceHandler.instance.currentInterface != Interfaces.item)
        {
            return;
        }

        inventoryType = PlayerItemController.instance.currentInventory;

        if (item != 0 && !itemWheels[(int)inventoryType - 1].itemID.Contains(item))
        {
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            
        }
    }
}
