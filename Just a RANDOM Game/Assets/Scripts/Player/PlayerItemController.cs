using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

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

    public int[] rightItems { get; private set; } = { 0, 1, 3, 4, 7, 8, 9 };// fill in basic tools, and set initial inventory to Storage, then switch to axe after starting cutscene
    public int[] leftItems { get; private set; } = { 0, 2, 0, 6, 22, 0, 10 };

    private int leftHeldItem = 0;
    private int rightHeldItem = 0;

    private UDictionaryIntInt resources;

    public float aimSpeed = 0.3f;
    public float aimTime = 0.5f;
    public GameObject arrowPrefab;
    public Vector3 rightAimOffset;
    public Vector3 leftAimOffset;
    private GameObject arrow;
    public float eatSpeed = 0.3f;
    public Vector3 torchOrigin;
    public Vector3 torchPlacement;
    public float torchPlaceMargin = 0.1f;

    public Coroutine torchCoroutine { get; private set; }

    public bool isFishing { get; private set; }
    private FishingController fishingController;
    private float fishTime;
    private Vector3 bobber;
    private RodTrigger rodTrigger;
    private bool showFishingCanvas = false;
    private bool isRightFish;
    private Coroutine fishCoroutine;

    public bool isAiming { get; private set; }
    private bool isRightAim;
    public bool isEating { get; private set; }
    private bool isRightEat;

    private float[] resetAnimTime = new float[2] { -1, -1 };

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

        if (resources.ContainsKey(5) && resources[5] != 0)
            rightItems[3] = 5;
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

        if (isAiming)
            StopAiming(true);

        if (isFishing)
            StartCoroutine(StopFishing());

        if (isEating)
            StopEating();

        if(torchCoroutine != null)
            StopCoroutine(torchCoroutine);

        CancelInvoke("ResetAnim");
        ResetAnim();
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
            //glowing contour
        }
        if(rightHeldItem == 0 && resources.ContainsKey(item.ID) && resources[item.ID] != 0)
        {
            rightHeldItem = item.ID;
            Instantiate(item.model, rightHandObj.transform);
            //use light orb + glowing contour to instantiate
        }

        item = database.GetItem[leftItems[(int)currentInventory]];
        if (leftHandObj.transform.childCount != 0 && (leftHeldItem != item.ID || !resources.ContainsKey(item.ID) || resources[item.ID] == 0))
        {
            leftHeldItem = 0;
            Destroy(leftHandObj.transform.GetChild(0).gameObject);
            //glowing contour
        }
        if (leftHeldItem == 0 && resources.ContainsKey(item.ID) && resources[item.ID] != 0)
        {
            leftHeldItem = item.ID;
            Instantiate(item.model, leftHandObj.transform);
            //use light orb + glowing contour to instantiate
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
            resetAnimTime[isRight ? 0 : 1] = Time.time;
            ResetAnim();
            return;
        }

        if(item.itemType == ItemTypes.Sword)
        {
            anim.SetInteger("ItemType", 1); //swing anim
            //set another anim integer randomly for three types of attacks

            SwordTrigger swordTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SwordTrigger>();
            swordTrigger.damage = item.damage;
            swordTrigger.detect = true;

            float useTime = toolUseTime[0] * (1 - item.attackSpeed / 100f);
            resetAnimTime[isRight ? 0 : 1] = Time.time + useTime;
            Invoke("ResetAnim", useTime);
            swordTrigger.Invoke("StopDetecting", useTime);
        }
        else if(item.itemType == ItemTypes.Bow)
        {
            if (!isAiming)
            {
                anim.SetInteger("ItemType", 2);
                isRightAim = isRight;
                StartAiming();
            }
        }
        else if (item.itemType == ItemTypes.Axe)
        {
            anim.SetInteger("ItemType", 3); //chop anim

            AxeTrigger axeTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<AxeTrigger>();
            axeTrigger.damage = item.damage;
            axeTrigger.detect = true;

            float useTime = toolUseTime[1] * (1 - item.attackSpeed / 100f);
            resetAnimTime[isRight ? 0 : 1] = Time.time + useTime;
            Invoke("ResetAnim", useTime);
            axeTrigger.Invoke("StopDetecting", useTime);
        }
        else if (item.itemType == ItemTypes.Pickaxe)
        {
            anim.SetInteger("ItemType", 4); //mine anim

            PickaxeTrigger pickaxeTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PickaxeTrigger>();
            pickaxeTrigger.damage = item.damage;
            pickaxeTrigger.detect = true;

            float useTime = toolUseTime[2] * (1 - item.attackSpeed / 100f);
            resetAnimTime[isRight ? 0 : 1] = Time.time + useTime;
            Invoke("ResetAnim", useTime);
            pickaxeTrigger.Invoke("StopDetecting", useTime);
        }
        else if (item.itemType == ItemTypes.Hoe)
        {
            anim.SetInteger("ItemType", 5);

            HoeTrigger hoeTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<HoeTrigger>();
            hoeTrigger.detect = true;
            hoeTrigger.range = item.range;

            float useTime = toolUseTime[3] * (1 - item.attackSpeed / 100f);
            resetAnimTime[isRight ? 0 : 1] = Time.time + useTime;
            Invoke("ResetAnim", useTime);
            hoeTrigger.Invoke("StopDetecting", useTime);
        }
        else if(item.itemType == ItemTypes.Rod)
        {
            if (!isFishing)
            {
                anim.SetInteger("ItemType", 6);
                rodTrigger = rightHandObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<RodTrigger>();
                rodTrigger.detect = true;
                fishTime = item.attackSpeed;
                isRightFish = isRight;
                fishCoroutine = StartCoroutine(StopFishing(toolUseTime[4]));
            }
            else
            {
                StartCoroutine(StopFishing());
            }
        }
        else if (item.itemType == ItemTypes.Food)
        {
            //eat
            if (!isEating)
            {
                anim.SetInteger("ItemType", 7);
                Invoke("Eat", toolUseTime[5]);
                isRightEat = isRight;
                GetComponent<PlayerEntity>().speedMultiplier *= eatSpeed;
            }
        }
        else if(item.itemType == ItemTypes.Crop)
        {
            anim.SetInteger("ItemType", 8); //throw carrot anim

            StartFarming(item.range, item.ID);
            float useTime = toolUseTime[6] * (1 - item.attackSpeed / 100f);
            resetAnimTime[isRight ? 0 : 1] = Time.time + useTime;
            Invoke("ResetAnim", useTime);
        }
        else if(item.itemType == ItemTypes.Torch)
        {
            Transform playerObj = PlayerController.instance.playerObj;
            LayerMask layerMask = 1 << 7;
            if (Physics.Raycast(playerObj.position + torchOrigin, playerObj.rotation * torchPlacement, out RaycastHit wallToPlace, torchPlacement.magnitude, layerMask))
            {
                anim.SetInteger("ItemType", 9);

                GameObject torch = isRight ? rightHandObj.transform.GetChild(0).gameObject : leftHandObj.transform.GetChild(0).gameObject;
                torchCoroutine = StartCoroutine(PlaceTorch(toolUseTime[7], isRight, item.ID, torch, wallToPlace));
            }
            else
            {
                resetAnimTime[isRight ? 0 : 1] = Time.time;
                ResetAnim();
            }
        }
    }

    public void Release(bool isRight = true, bool esc = false)
    {
        if (isEating && isRightEat == isRight)
        {
            if (!esc)
                Eat();
            else
                StopEating();
        }
        else if (isAiming && isRightAim == isRight)
        {
            if(!esc)
                ShootArrow();
            else
                StopAiming(true);
        }
    }

    public void ResetAnim()
    {
        bool isRight = resetAnimTime[0] != -1 && (resetAnimTime[0] <= resetAnimTime[1] || resetAnimTime[1] == -1);
        if (!isRight && resetAnimTime[1] == -1)
            return;
        resetAnimTime[isRight ? 0 : 1] = -1;

        anim.SetInteger("ItemType", 0);
        PlayerController.instance.ResetUseLeftRight(isRight);
    }

    private void StartAiming()
    {
        isAiming = true;
        StopCoroutine("ResetCameraAfterDelay");
        Camera.main.GetComponent<ThirdPersonCam>().SwitchCameraStyle(CameraStyle.Combat);
        GetComponent<PlayerEntity>().speedMultiplier *= aimSpeed;

        float switchTime = Camera.main.GetComponent<ThirdPersonCam>().switchTime;
        Invoke("SummonArrow", Mathf.Max(aimTime, switchTime));

        if (switchTime <= 0)
            InteractablePromptController.instance.EnableCrosshair();
    }

    private void SummonArrow()
    {
        Vector3 arrowDirection = Camera.main.GetComponent<ThirdPersonCam>().combatLookAt.position - Camera.main.transform.position;
        Transform playerObj = PlayerController.instance.playerObj;
        arrow = ObjectPoolManager.CreatePooled(arrowPrefab, leftHandObj.transform.GetChild(0).position, Quaternion.LookRotation(arrowDirection));
        arrow.transform.SetParent(leftHandObj.transform.GetChild(0));
        arrow.transform.localPosition = isRightAim ? rightAimOffset : leftAimOffset;

        InteractablePromptController.instance.EnableCrosshair();
    }

    private void ShootArrow()
    {
        if(arrow != null)
        {
            anim.SetTrigger("Shoot");
            arrow.transform.SetParent(null);
            arrow.GetComponent<Projectile>().Fire();
            arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * database.GetItem[isRightAim ? rightHeldItem : leftHeldItem].attackSpeed;
            arrow.GetComponent<Projectile>().damage = new Damage(database.GetItem[isRightAim ? rightItems[1] : leftItems[1]].damage);
            arrow = null;
        }
        StopAiming();
    }

    public void StopAiming(bool esc = false)
    {
        CancelInvoke("SummonArrow");

        if (arrow != null && !arrow.GetComponent<Projectile>().fired)
        {
            ObjectPoolManager.DestroyPooled(arrow);
            arrow = null;
        }

        isAiming = false;
        GetComponent<PlayerEntity>().speedMultiplier /= aimSpeed;

        resetAnimTime[isRightAim ? 0 : 1] = Time.time + aimTime;
        Invoke("ResetAnim", aimTime);
        if (!esc && Camera.main.GetComponent<ThirdPersonCam>().switchTime <= 0)
            StartCoroutine(ResetCameraAfterDelay(aimTime + 0.01f));
        else
            ResetCamera();
    }

    private IEnumerator ResetCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCamera();
    }

    private void ResetCamera()
    {
        if (!isAiming)
        {
            Camera.main.GetComponent<ThirdPersonCam>().SwitchCameraStyle(CameraStyle.Basic);
            InteractablePromptController.instance.DisableCrosshair();
        }
    }

    private void Eat()
    {
        GetComponent<PlayerEntity>().satiation += database.GetItem[isRightEat?rightItems[6]:leftItems[6]].damage;
        StopEating();
    }

    public void StopEating()
    {
        CancelInvoke("Eat");
        GetComponent<PlayerEntity>().speedMultiplier /= eatSpeed;
        resetAnimTime[isRightEat ? 0 : 1] = Time.time;
        ResetAnim();
    }

    public void StartFarming(Vector3 hoeRange, int plant = 0)
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

    public void StartFishing(FishingController controller, Vector3 bobberPos)
    {
        StopCoroutine(fishCoroutine);
        bobber = bobberPos;
        fishingController = controller;

        fishTime = Random.Range(controller.fishHookTime[0] * (1 - fishTime / 100f), controller.fishHookTime[1] * (1 - fishTime / 100f));
        isFishing = true;
    }

    public IEnumerator StopFishing(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        resetAnimTime[isRightFish ? 0 : 1] = Time.time;
        Invoke("ResetAnim", toolUseTime[4]);

        //reel in anim
        isFishing = false;
        rodTrigger.detect = false;
        showFishingCanvas = false;

        if (fishingController != null)
        {
            fishingController.StopFishing();
            fishingController = null;
        }
    }

    private IEnumerator PlaceTorch(float delay, bool isRight, int itemID, GameObject torch, RaycastHit wallToPlace)
    {
        //use StopCoroutine when changed

        //rotate torch at wallToPlace.transform.position facing off the wall's tangent
        Vector3 pos = wallToPlace.point + wallToPlace.normal * 0.5f;
        Quaternion rot = Quaternion.LookRotation(wallToPlace.normal);

        //set IK and lerp torch rotation

        yield return new WaitForSeconds(delay / 2); // reach out

        if ((torch.transform.position - pos).magnitude >= torchPlaceMargin)
        {
            resetAnimTime[isRight ? 0 : 1] = Time.time;
            ResetAnim();
            yield break;
        }
        
        ItemDropHandler.instance.AddNewDrop(itemID, torch);
        InventoryHandler.instance.RemoveItem(itemID);
        if (isRight) rightHeldItem = 0;
        else leftHeldItem = 0;
        
        yield return new WaitForSeconds(delay / 2); // retract hand

        UpdateHandModel();
        resetAnimTime[isRight ? 0 : 1] = Time.time;
        ResetAnim();
    }

    // visualizing hoe interaction area
    private void OnDrawGizmosSelected()
    {
        CharacterController pos = GetComponent<CharacterController>();
        Transform playerObj = transform.GetChild(0).transform;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + torchOrigin, transform.position + torchOrigin + playerObj.rotation * torchPlacement);

        Item item = database.GetItem[7];
        Vector3 boxCenter = transform.position + pos.center - new Vector3(0, pos.height / 2, 0) + item.range.z / 2 * playerObj.forward;

        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, playerObj.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, item.range);
    }
}
