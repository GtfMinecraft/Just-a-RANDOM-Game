using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayerItemController : MonoBehaviour
{
    public static PlayerItemController instance;

    public GameObject magicStone;

    private Animator animator;
    private Item rightItem;
    private Item leftItem;
    private InventoryTypes currentChunk;

    [HideInInspector] public bool canEat = false;

    public GameObject rightHand;
    public GameObject leftHand;

    public Item[] defaultItems;

    /*
     *  0 empty
     *  1 weapon
     *  2 axe
     *  3 pickaxe
     *  4 hoe
     *  5 rod
     *  6 food
     *  7 bait
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
        leftItem = null;
        UpdateHandModel(magicStone, leftHand);
    }

    public void ChangeChunk(InventoryTypes chunk)
    {
        InterfaceHandler.instance.inventoryCanvas.ChangeToolInventory(chunk);

        currentChunk = chunk;
        SwapHandItem(defaultItems[(int)chunk]);
        leftItem = null;
        UpdateHandModel(magicStone, leftHand);
    }

    public void SwapHandItem(Item item)
    {
        if(item == null)
        {
            UpdateHandModel(null, rightHand);
        }
        else if (item.itemType == ItemTypes.Tool && item.inventoryType == currentChunk)
        {
            rightItem = item;
            UpdateHandModel(rightItem.model, rightHand);
        }
        else if (item.itemType == ItemTypes.Food)
        {
            leftItem = item;
            UpdateHandModel(item.model, leftHand);
            canEat = true;
        }
        else if (item.itemType == ItemTypes.Potion && item.inventoryType == currentChunk)
        {
            leftItem = item;
            UpdateHandModel(item.model, leftHand);
            //use potion anim
            //potion effect
            leftItem = null;
            UpdateHandModel(magicStone, leftHand);
        }
        else if(item.itemType == ItemTypes.Bait && item.inventoryType == currentChunk)
        {

        }
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
