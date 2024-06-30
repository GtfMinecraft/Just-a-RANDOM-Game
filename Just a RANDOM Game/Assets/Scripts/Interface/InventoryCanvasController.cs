using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class InventoryCanvasController : MonoBehaviour
{
    public static InventoryCanvasController instance;
    [Header("Animator")]
    public Animator toolAnim;
    public Animator itemAnim;

    [Header("Inventory")]
    [SerializeField]
    private InventoryTypes currentInventory;

    public Canvas storage;
    public Canvas toolWheel;
    public Canvas itemWheel;

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

    // Start is called before the first frame update
    void Start()
    {
        CloseAllInventory();

        currentInventory = (InventoryTypes)PlayerPrefs.GetInt("selectedTool");
    }

    public void ChangeToolInventory(InventoryTypes inv)
    {
        currentInventory = inv;
    }

    public void CloseAllInventory()
    {
        storage.enabled = false;
        toolWheel.enabled = false;
        itemWheel.enabled = false;
    }

    public void ToolWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if(ctx.action.name != "ToolWheel" && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item || InterfaceHandler.instance.currentInterface == Interfaces.tool))
            {
                if(InterfaceHandler.instance.currentInterface == Interfaces.tool)
                {
                    ToolWheelAnimAsync();
                }

                switch (ctx.action.name)
                {
                    case "Weapon":
                        if(currentInventory != InventoryTypes.weapon)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.weapon);
                        }
                        break;

                    case "Axe":
                        if (currentInventory != InventoryTypes.axe)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.axe);
                        }
                        break;

                    case "Pickaxe":
                        if (currentInventory != InventoryTypes.pickaxe)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.pickaxe);
                        }
                        break;

                    case "Hoe":
                        if (currentInventory != InventoryTypes.hoe)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.hoe);
                        }
                        break;

                    case "Rod":
                        if (currentInventory != InventoryTypes.rod)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.rod);
                        }
                        break;

                    case "Food":
                        if (currentInventory != InventoryTypes.food)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.food);
                        }
                        break;

                    case "Storage":
                        if (currentInventory != InventoryTypes.storage)
                        {
                            if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                            {
                                ItemWheelAnimAsync();
                            }
                            PlayerItemController.instance.ChangeInventory(InventoryTypes.storage);
                        }
                        break;
                }
            }
            else if(ctx.valueType == typeof(Vector2) && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item))
            {
                if(InterfaceHandler.instance.currentInterface == Interfaces.item)
                {
                    ItemWheelAnimAsync();
                }

                int scroll = (int)currentInventory + (int)ctx.ReadValue<Vector2>().normalized.y;
                while(scroll <= 0)
                {
                    scroll += 6;
                }
                while (scroll > 6)
                {
                    scroll -= 6;
                }
                PlayerItemController.instance.ChangeInventory((InventoryTypes)scroll);
            }
            else if (ctx.valueType == typeof(float) && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item))
            {
                InterfaceHandler.instance.OpenInterface(Interfaces.tool, true, false, true);
                toolWheel.enabled = true;
                toolAnim.SetBool("OpenToolWheel", true);
            }
            else if (ctx.valueType == typeof(float) && InterfaceHandler.instance.currentInterface == Interfaces.tool)
            {
                toolAnim.SetBool("OpenToolWheel", true);
            }
        }
        else if (ctx.canceled && ctx.action.name == "ToolWheel" && InterfaceHandler.instance.currentInterface == Interfaces.tool)
        {
            toolWheel.GetComponent<ToolWheelUI>().SwapTool();
            ToolWheelAnimAsync();
        }
    }

    private async void ToolWheelAnimAsync()
    {
        if (toolAnim.GetBool("OpenToolWheel"))
        {
            toolAnim.Play("Close", 0, 0.3f);
        }
        toolAnim.SetBool("OpenToolWheel", false);

        while (!toolAnim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            if (toolAnim.GetBool("OpenToolWheel"))
            {
                return;
            }
            await Task.Delay(100);
        }
        if (InterfaceHandler.instance.currentInterface == Interfaces.tool)
            InterfaceHandler.instance.CloseAllInterface();
    }

    public void ItemWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && currentInventory != InventoryTypes.storage && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.item, true, false, true);
            itemWheel.GetComponent<ItemWheelUI>().RefreshItemWheel();
            itemWheel.enabled = true;
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.item)
        {
            itemWheel.GetComponent<ItemWheelUI>().SwapTool();
            ItemWheelAnimAsync();
        }
    }

    private async void ItemWheelAnimAsync()
    {
        if (InterfaceHandler.instance.currentInterface == Interfaces.item)
            InterfaceHandler.instance.CloseAllInterface();

        await Task.Delay(100);
    }

    public void PlayerInventoryHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.storage, false, false, false);
            InventoryHandler.instance.UpdateInventoryUI();
            storage.enabled = true;
        }
        else if(ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }
}
