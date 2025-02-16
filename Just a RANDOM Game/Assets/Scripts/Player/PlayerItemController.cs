using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    public static PlayerItemController instance;

    public ItemDatabase database;

    public Animator anim;
    public InventoryTypes currentInventory { get; private set; }

    //[HideInInspector]
    //public bool canEat = false;

    public GameObject rightHandObj;
    public GameObject leftHandObj;

    public float[] toolUseTime = { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 0.8f, 2.5f };

    public int[] rightItems { get; private set; } = { 0, 1, 2, 0, 3, 4, 0 };// fill in basic tools, and set initial inventory to Storage, then switch to axe after starting cutscene
    public int[] leftItems { get; private set; } = { 0, 0, 0, 0, 0, 0, 0 };

    private int leftHeldItem = 0;
    private int rightHeldItem = 0;

    private UDictionaryIntInt resources;

    public bool isFarming { get; private set; }
    private HoeTrigger hoeTrigger;
    private Vector3 hoeRange;

    public bool isFishing { get; private set; }
    private FishingController fishingController;
    private float fishTime;
    private Vector3 bobber;
    private RodTrigger rodTrigger;
    private bool showFishingCanvas = false;

    public bool isAiming { get; private set; }
    public bool isEating { get; private set; }

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
        resources = InventoryHandler.instance.resources;
        ChangeInventory((InventoryTypes)PlayerPrefs.GetInt("selectedTool", 0));

        //string rightItemsString = PlayerPrefs.GetString("rightItemsString", "");
        //string leftItemsString = PlayerPrefs.GetString("leftItemsString", "");
        //for(int i = 0; i < rightItems.Length; i++)
        //{
        //    rightItems[i] = (rightItemsString.Length > i) ? rightItemsString[i] - '0' : 0;
        //    leftItems[i] = (leftItemsString.Length > i) ? leftItemsString[i] - '0' : 0;
        //}
        //if(currentInventory != InventoryTypes.Storage)
        //{
        //    UpdateHandModel(database.GetItem[rightItems[(int)currentInventory]].model);
        //    UpdateHandModel(database.GetItem[leftItems[(int)currentInventory]].model, true);
        //}
    }

    private void Update()
    {
        if (isFishing)
        {
            if(fishTime > 0)
            {
                fishTime -= Time.deltaTime;
            }
            else if(!showFishingCanvas)
            {
                showFishingCanvas = true;
                fishingController.ShowFishCanvas(bobber);
            }
        }
    }

    public void ChangeInventory(InventoryTypes inv)
    {
        currentInventory = inv;
        InventoryCanvasController.instance.ChangeToolInventory(currentInventory);

        if (isFarming)
        {
            CancelInvoke("StopFarming");
            StopFarming();
        }

        if (isFishing)
        {
            CancelInvoke("StopFishing");
            StopFishing();
        }

        CancelInvoke("ResetAnim");
        ResetAnim();

        //change tool anim

        UpdateHandModel();
    }

    //public void SwapHandItem(int itemID)
    //{
    //    Item item = database.GetItem[itemID];

    //    if (item == null)
    //    {
    //        UpdateHandModel(null);
    //    }
    //    else if (item.itemType == ItemTypes.Tool)
    //    {
    //        rightItems[(int)currentInventory] = itemID;
    //        UpdateHandModel(item.model);
    //    }
    //    else if (item.itemType == ItemTypes.Food)
    //    {
    //        leftItems[(int)currentInventory] = itemID;
    //        UpdateHandModel(item.model, true);
    //        canEat = true;
    //    }
    //    else if (item.itemType == ItemTypes.Potion)
    //    {
    //        int temp = leftItems[(int)currentInventory];

    //        UpdateHandModel(item.model, true);
    //        UpdateHandModel(database.GetItem[temp].model, true);
    //    }
    //    else if (item.itemType == ItemTypes.Bait)
    //    {

    //    }
    //}

    public void UpdateHandModel()
    {
        //if change is from item wheel, stop item anim first
        //also, add left or right hand update

        Item item = database.GetItem[rightItems[(int)currentInventory]];
        if(rightHandObj.transform.childCount != 0 && (rightHeldItem != item.ID || !resources.ContainsKey(item.ID) || resources[item.ID] == 0))
        {
            rightHeldItem = 0;
            Destroy(rightHandObj.transform.GetChild(0).gameObject);
        }
        if(rightHeldItem == 0 && resources.ContainsKey(item.ID) && resources[item.ID] != 0)
        {
            rightHeldItem = item.ID;
            Instantiate(item.model, rightHandObj.transform);
        }

        item = database.GetItem[leftItems[(int)currentInventory]];
        if (leftHandObj.transform.childCount != 0 && (leftHeldItem != item.ID || !resources.ContainsKey(item.ID) || resources[item.ID] == 0))
        {
            leftHeldItem = 0;
            Destroy(leftHandObj.transform.GetChild(0).gameObject);
        }
        if (leftHeldItem == 0 && resources.ContainsKey(item.ID) && resources[item.ID] != 0)
        {
            leftHeldItem = item.ID;
            Instantiate(item.model, leftHandObj.transform);
        }
    }

    //public void SetDefaultItem(int itemID, bool isRight = true)
    //{
    //    Item item = database.GetItem[itemID];

    //    if ((item.itemType == ItemTypes.Crop || item.itemType == ItemTypes.Bow || item.itemType == ItemTypes.Food) && leftItems[(int)item.inventoryType] == 0)
    //    {
    //        leftItems[(int)item.inventoryType] = itemID;
    //        UpdateHandModel();
    //    }
    //    else if ((item.itemType == ItemTypes.Sword || item.itemType == ItemTypes.Axe || item.itemType == ItemTypes.Pickaxe || 
    //        item.itemType == ItemTypes.Rod || item.itemType == ItemTypes.Food) && rightItems[(int)item.inventoryType] == 0)
    //    {
    //        rightItems[(int)item.inventoryType] = itemID;
    //        UpdateHandModel();
    //    }
    //    //set left / right item according to ItemWheel's selection
    //}

    public void UseItem(bool isRight = true)
    {
        Item item;
        item = isRight ? database.GetItem[rightItems[(int)currentInventory]] : database.GetItem[leftItems[(int)currentInventory]];

        if ((isRight ? rightHeldItem : leftHeldItem) != item.ID || !resources.ContainsKey(item.ID) || resources[item.ID] == 0)
        {
            ResetAnim();
            return;
        }

        if(item.itemType == ItemTypes.Sword)
        {
            anim.SetInteger("ItemType", 1); //swing anim
            Invoke("ResetAnim", toolUseTime[0] * (1 - item.attackSpeed / 100f));
        }
        else if(item.itemType == ItemTypes.Bow)
        {
            //aim
        }
        else if (item.itemType == ItemTypes.Axe)
        {
            //chop anim
            Invoke("ResetAnim", toolUseTime[2] * (1 - item.attackSpeed / 100f));
        }
        else if (item.itemType == ItemTypes.Pickaxe)
        {
            //mine anim
            Invoke("ResetAnim", toolUseTime[3] * (1 - item.attackSpeed / 100f));
        }
        else if (item.itemType == ItemTypes.Hoe)
        {
            anim.SetInteger("ItemType", 5);

            hoeRange = item.range;
            hoeTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<HoeTrigger>();
            hoeTrigger.detect = true;

            Invoke("ResetAnim", toolUseTime[4] * (1 - item.attackSpeed / 100f));
        }
        else if(item.itemType == ItemTypes.Rod)
        {
            if (!isFishing)
            {
                anim.SetInteger("ItemType", 6);
                rodTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RodTrigger>();
                rodTrigger.detect = true;
                fishTime = item.attackSpeed;
                Invoke("StopFishing", toolUseTime[5]);
            }
            else
            {
                StopFishing();
            }
        }
        else if (item.itemType == ItemTypes.Food)
        {
            //eat
        }
        else if(item.itemType == ItemTypes.Crop)
        {
            //throw carrot anim

            hoeRange = new Vector3 (2.5f, 1f, 2.5f);
            StartFarming(item.ID);
            Invoke("ResetAnim", toolUseTime[7] * (1 - item.attackSpeed / 100f));
        }
        else
        {
            ResetAnim();
        }
    }

    public void Release(bool isRight = true)
    {
        if (isEating || isAiming)
        {
            ResetAnim();
        }
    }

    public void ResetAnim()
    {
        anim.SetInteger("ItemType", 0);
        PlayerController.instance.ResetUseLeftRight();
    }

    public void StartFarming(int plant = 0)
    {
        CharacterController pos = GetComponent<CharacterController>();
        Transform playerObj = PlayerController.instance.playerObj;
        Collider[] hits;
        hits = Physics.OverlapBox(transform.position + pos.center - new Vector3(0, pos.height / 2, 0) + hoeRange.z / 2 * playerObj.forward, hoeRange / 2, playerObj.rotation);

        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<FarmingController>() != null)
            {
                hit.GetComponent<FarmingController>().Harvest(plant);
            }
        }
    }

    private void StopFarming()
    {
        hoeTrigger.detect = false;
        ResetAnim();
    }

    public void StartFishing(FishingController controller, Vector3 bobberPos)
    {
        CancelInvoke("StopFishing");
        bobber = bobberPos;
        fishingController = controller;

        fishTime = Random.Range(controller.fishHookTime[0] * (1 - fishTime / 100f), controller.fishHookTime[1] * (1 - fishTime / 100f));
        isFishing = true;
    }

    public void StopFishing()
    {
        //reel in anim
        Invoke("ResetAnim", toolUseTime[5]);

        isFishing = false;
        rodTrigger.detect = false;
        showFishingCanvas = false;

        if (fishingController != null)
        {
            fishingController.StopFishing();
            fishingController = null;
        }
    }

    // visualizing hoe interaction area
    private void OnDrawGizmosSelected()
    {
        Item item = database.GetItem[3];

        CharacterController pos = GetComponent<CharacterController>();
        Transform playerObj = transform.GetChild(0).transform;

        // Calculate the center position of the box
        Vector3 boxCenter = transform.position + pos.center - new Vector3(0, pos.height / 2, 0) + item.range.z / 2 * playerObj.forward;

        // Draw the box using Gizmos.DrawWireCube (half the range since it's the full box size)
        Gizmos.color = Color.green;  // You can change the color here
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerObj.rotation, Vector3.one); // Apply rotation of the player

        Gizmos.DrawWireCube(Vector3.zero, item.range);
    }
}
