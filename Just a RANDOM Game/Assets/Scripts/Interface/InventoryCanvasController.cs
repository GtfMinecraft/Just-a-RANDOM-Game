using System;
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

    [Header("Inventory (Don't edit currentInventory)")]
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
        storage.enabled = false;
        toolWheel.enabled = false;
        toolWheel.GetComponent<ToolWheelUI>().scrollWheel.enabled = false;
        itemWheel.enabled = false;
    }

    public void ChangeToolInventory(InventoryTypes inv)
    {
        currentInventory = inv;
    }

    public void CloseAllInventory()
    {
        InventoryHandler.instance.ResetDraggingUI();
        storage.enabled = false;
        toolWheel.enabled = false;
        toolWheel.GetComponent<ToolWheelUI>().scrollWheel.enabled = false;
        itemWheel.enabled = false;
        itemWheel.GetComponent<ItemWheelUI>().itemWheelTransform.gameObject.SetActive(false);
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

                string actionName = ctx.action.name.ToLower();
                if (actionName != Enum.GetName(typeof(InventoryTypes), currentInventory))
                {
                    if(Enum.TryParse(actionName, out InventoryTypes inv))
                    {
                        PlayerItemController.instance.ChangeInventory(inv);

                        if (InterfaceHandler.instance.currentInterface == Interfaces.item)
                        {
                            itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
                        }

                        toolWheel.GetComponent<ToolWheelUI>().ScrollToolImage((int)currentInventory);
                    }
                    else
                    {
                        Debug.LogError($"No inventory for actionName {actionName}");
                    }
                }
                else
                {
                    toolWheel.GetComponent<ToolWheelUI>().ScrollToolImage((int)currentInventory);
                }
            }
            else if(ctx.valueType == typeof(Vector2) && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item))
            {
                if(InterfaceHandler.instance.currentInterface == Interfaces.item)
                {
                    itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
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

                if (InterfaceHandler.instance.currentInterface == Interfaces.none)
                {
                    toolWheel.GetComponent<ToolWheelUI>().ScrollToolImage(scroll);
                }
            }
            else if (ctx.valueType == typeof(float) && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item))
            {
                InterfaceHandler.instance.OpenInterface(Interfaces.tool, true, false, true);
                toolWheel.GetComponent<ToolWheelUI>().UpdateToolWheelUI();
                toolWheel.enabled = true;
                toolAnim.SetBool("OpenWheel", true);
            }
            else if (ctx.valueType == typeof(float) && InterfaceHandler.instance.currentInterface == Interfaces.tool)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                toolWheel.GetComponent<ToolWheelUI>().UpdateToolWheelUI();
                toolAnim.SetBool("OpenWheel", true);
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (toolAnim.GetBool("OpenWheel"))
        {
            toolAnim.Play("Close", 0, 0.3f);
        }
        toolAnim.SetBool("OpenWheel", false);

        while (!toolAnim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            if (toolAnim.GetBool("OpenWheel"))
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
        if (ctx.performed && currentInventory != InventoryTypes.storage)
        {
            if(InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool)
            {
                InterfaceHandler.instance.OpenInterface(Interfaces.item, true, false, true);
                itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
                itemWheel.enabled = true;
                itemAnim.SetBool("OpenWheel", true);
            }
            else if(InterfaceHandler.instance.currentInterface == Interfaces.item)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
                itemAnim.SetBool("OpenWheel", true);
            }
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.item)
        {
            itemWheel.GetComponent<ItemWheelUI>().SwapTool();
            ItemWheelAnimAsync();
        }
    }

    private async void ItemWheelAnimAsync()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (itemAnim.GetBool("OpenWheel"))
        {
            itemAnim.Play("Close", 0, 0.3f);
        }
        itemAnim.SetBool("OpenWheel", false);

        while (!itemAnim.GetCurrentAnimatorStateInfo(0).IsName("Hidden"))
        {
            if (itemAnim.GetBool("OpenWheel"))
            {
                return;
            }
            await Task.Delay(100);
        }

        if (InterfaceHandler.instance.currentInterface == Interfaces.item)
            InterfaceHandler.instance.CloseAllInterface();
    }

    public void PlayerInventoryHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.storage, true, false, true);
            InventoryHandler.instance.UpdateInventoryUI();
            storage.enabled = true;
        }
        else if(ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }
}
