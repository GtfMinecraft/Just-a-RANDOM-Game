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

    public int[] rightItems { get; private set; } = new int[6];
    public int[] leftItems { get; private set; } = new int[6];

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
        UpdateHandModel(database.GetItem[rightItems[(int)currentInventory]].model);
        UpdateHandModel(database.GetItem[leftItems[(int)currentInventory]].model, true);
    }

    public void ChangeInventory(InventoryTypes inv)
    {
        currentInventory = inv;
        InventoryCanvasController.instance.ChangeToolInventory(inv);
        
        //SwapHandItem(database.GetItem[defaultRightItems[(int)inv]], rightHand);
        //SwapHandItem(database.GetItem[defaultLeftItems[(int)inv]], leftHand);
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

            leftItems[(int)currentInventory] = itemID;
            UpdateHandModel(item.model, true);

            leftItems[(int)currentInventory] = temp;
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
}
