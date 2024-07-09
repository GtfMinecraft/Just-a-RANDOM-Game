using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public static PlayerItemController instance;

    public ItemDatabase database;

    public GameObject magicStone;

    private Animator animator;
    public int rightItem { get; private set; }
    public int leftItem { get; private set; }
    public InventoryTypes currentInventory { get; private set; }

    [HideInInspector]
    public bool canEat = false;

    public GameObject rightHand;
    public GameObject leftHand;

    //TODO: save and load default items (use playerprefs)
    public int[] defaultRightItems;
    public int[] defaultLeftItems;

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
        //get left and right itemID from playprefs
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

    public int HoldItem(int itemID)
    {
        return 0;
    }

    private void SwapHandItem(Item item, GameObject hand)
    {
        //if (item == null)
        //{
        //    UpdateHandModel(null, hand);
        //}
        //else if (item.itemType == ItemTypes.Tool && item.inventoryType == currentInventory)
        //{
        //    rightItem = item;
        //    UpdateHandModel(rightItem.model, rightHand);
        //}
        //else if (item.itemType == ItemTypes.Food)
        //{
        //    leftItem = item;
        //    UpdateHandModel(item.model, leftHand);
        //    canEat = true;
        //}
        //else if (item.itemType == ItemTypes.Potion && item.inventoryType == currentInventory)
        //{
        //    leftItem = item;
        //    UpdateHandModel(item.model, leftHand);
        //    //use potion anim
        //    //potion effect
        //    leftItem = null;
        //    UpdateHandModel(null, leftHand);
        //}
        //else if(item.itemType == ItemTypes.Bait && item.inventoryType == currentInventory)
        //{

        //}
    }

    private void UpdateHandModel(GameObject Item, GameObject hand)
    {
        if(hand == rightHand)
        {
            //right hand backpack anim
        }
        else if(hand == leftHand)
        {
            //left hand magic anim
        }
        //done => return
    }
}
