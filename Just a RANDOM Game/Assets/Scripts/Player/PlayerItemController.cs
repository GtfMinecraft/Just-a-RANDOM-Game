using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public static PlayerItemController instance;

    public ItemDatabase database;
    public GameObject magicStone;

    private Animator animator;
    public InventoryTypes currentInventory { get; private set; }

    [HideInInspector]
    public bool canEat = false;

    public GameObject rightHandObj;
    public GameObject leftHandObj;

    public int[] rightItems { get; private set; } = new int[7];
    public int[] leftItems { get; private set; } = new int[7];

    /*
     *  0 empty
     *  1 weapon
     *  2 axe
     *  3 pickaxe
     *  4 hoe
     *  5 rod
     *  6 food
    */

    private void Awake()
    {
        if (instance == null)
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
        animator = GetComponent<Animator>();
        ChangeInventory((InventoryTypes)PlayerPrefs.GetInt("selectedTool", 0));

        string rightItemsString = PlayerPrefs.GetString("rightItemsString", "");
        string leftItemsString = PlayerPrefs.GetString("leftItemsString", "");
        for(int i = 0; i < rightItems.Length; i++)
        {
            rightItems[i] = (rightItemsString.Length > i) ? rightItemsString[i] - '0' : 0;
            leftItems[i] = (leftItemsString.Length > i) ? leftItemsString[i] - '0' : 0;
        }
        if(currentInventory != InventoryTypes.Storage)
        {
            UpdateHandModel(database.GetItem[rightItems[(int)currentInventory]].model);
            UpdateHandModel(database.GetItem[leftItems[(int)currentInventory]].model, true);
        }
    }

    public void ChangeInventory(InventoryTypes inv)
    {
        currentInventory = inv;
        InventoryCanvasController.instance.ChangeToolInventory(currentInventory);
        
        if(inv != InventoryTypes.Storage)
        {
            UpdateHandModel(database.GetItem[rightItems[(int)currentInventory]].model);
            UpdateHandModel(database.GetItem[leftItems[(int)currentInventory]].model, true);
        }
        //leftItem = null;
        //UpdateHandModel(magicStone, leftHand);
    }

    public void SwapHandItem(int itemID)
    {
        Item item = database.GetItem[itemID];

        if (item == null)
        {
            UpdateHandModel(null);
        }
        else if (item.itemType == ItemTypes.Tool)
        {
            rightItems[(int)currentInventory] = itemID;
            UpdateHandModel(item.model);
        }
        else if (item.itemType == ItemTypes.Food)
        {
            leftItems[(int)currentInventory] = itemID;
            UpdateHandModel(item.model, true);
            canEat = true;
        }
        else if (item.itemType == ItemTypes.Potion)
        {
            int temp = leftItems[(int)currentInventory];

            UpdateHandModel(item.model, true);
            UpdateHandModel(database.GetItem[temp].model, true);
        }
        else if (item.itemType == ItemTypes.Bait)
        {

        }
    }

    private void UpdateHandModel(GameObject itemModel, bool leftHand = false)
    {
        if(!leftHand)
        {
            if(rightHandObj.transform.childCount != 0)
            {
                Destroy(rightHandObj.transform.GetChild(0).gameObject);
            }
            if(itemModel != null)
            {
                Instantiate(itemModel, rightHandObj.transform);
            }
        }
        else if(leftHand)
        {
            if (leftHandObj.transform.childCount != 0)
            {
                Destroy(leftHandObj.transform.GetChild(0).gameObject);
            }
            if(itemModel != null)
            {
                Instantiate(itemModel, leftHandObj.transform);
            }
        }
    }

    public void SetDefaultItem(int itemID)
    {
        Item item = database.GetItem[itemID];

        if ((item.itemType == ItemTypes.Bait || item.itemType == ItemTypes.Seed) && leftItems[(int)item.inventoryType] == 0)
        {
            leftItems[(int)item.inventoryType] = itemID;
            UpdateHandModel(item.model, true);
        }
        else if(item.itemType == ItemTypes.Tool && rightItems[(int)item.inventoryType] == 0)
        {
            rightItems[(int)item.inventoryType] = itemID;
            UpdateHandModel(item.model);
        }
        //set left / right item to this item if it is one of the necessary items

        //basic tool, tool upgrade, aquire off-hand item
    }
}
