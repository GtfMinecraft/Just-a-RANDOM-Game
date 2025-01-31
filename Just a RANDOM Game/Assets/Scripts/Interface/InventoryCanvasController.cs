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

    private bool closeToolWheel = false;
    private bool closeItemWheel = false;

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

    private void Update()
    {
        if (closeToolWheel)
        {
            if (toolAnim.GetBool("OpenWheel"))
            {
                closeToolWheel = false;
            }
            else if (toolAnim.GetCurrentAnimatorStateInfo(0).IsName("Hidden") && InterfaceHandler.instance.currentInterface == Interfaces.Tool)
            {
                closeToolWheel = false;
                InterfaceHandler.instance.CloseAllInterface();
            }
        }

        if(closeItemWheel)
        {
            if (itemAnim.GetBool("OpenWheel"))
            {
                closeItemWheel = false;
            }
            else if (itemAnim.GetCurrentAnimatorStateInfo(0).IsName("Hidden") && InterfaceHandler.instance.currentInterface == Interfaces.Item)
            {
                closeItemWheel = false;
                InterfaceHandler.instance.CloseAllInterface();
            }
        }
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
            if(ctx.action.name != "ToolWheel" && (InterfaceHandler.instance.currentInterface == Interfaces.None || InterfaceHandler.instance.currentInterface == Interfaces.Item || InterfaceHandler.instance.currentInterface == Interfaces.Tool))
            {
                if(InterfaceHandler.instance.currentInterface == Interfaces.Tool)
                {
                    CloseToolWheelAnim();
                }

                string actionName = ctx.action.name;
                if (actionName != Enum.GetName(typeof(InventoryTypes), currentInventory))
                {
                    if(Enum.TryParse(actionName, out InventoryTypes inv))
                    {
                        PlayerItemController.instance.ChangeInventory(inv);

                        if (InterfaceHandler.instance.currentInterface == Interfaces.Item)
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
            else if(ctx.valueType == typeof(Vector2) && (InterfaceHandler.instance.currentInterface == Interfaces.None || InterfaceHandler.instance.currentInterface == Interfaces.Item))
            {
                if(InterfaceHandler.instance.currentInterface == Interfaces.Item)
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

                if (InterfaceHandler.instance.currentInterface == Interfaces.None)
                {
                    toolWheel.GetComponent<ToolWheelUI>().ScrollToolImage(scroll);
                }
            }
            else if (ctx.valueType == typeof(float) && (InterfaceHandler.instance.currentInterface == Interfaces.None || InterfaceHandler.instance.currentInterface == Interfaces.Item))
            {
                InterfaceHandler.instance.OpenInterface(Interfaces.Tool, true, false, true);
                toolWheel.GetComponent<ToolWheelUI>().UpdateToolWheelUI();
                toolWheel.enabled = true;
                toolAnim.SetBool("OpenWheel", true);
            }
            else if (ctx.valueType == typeof(float) && InterfaceHandler.instance.currentInterface == Interfaces.Tool)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                toolWheel.GetComponent<ToolWheelUI>().UpdateToolWheelUI();
                toolAnim.SetBool("OpenWheel", true);
            }
        }
        else if (ctx.canceled && ctx.action.name == "ToolWheel" && InterfaceHandler.instance.currentInterface == Interfaces.Tool)
        {
            toolWheel.GetComponent<ToolWheelUI>().SwapTool();
            CloseToolWheelAnim();
        }
    }

    private void CloseToolWheelAnim()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (toolAnim.GetBool("OpenWheel"))
        {
            toolAnim.SetBool("OpenWheel", false);
            toolAnim.Play("Close", 0, 0.3f);
        }

        closeToolWheel = true;
    }

    public void ItemWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && currentInventory != InventoryTypes.Storage)
        {
            if(InterfaceHandler.instance.currentInterface == Interfaces.None || InterfaceHandler.instance.currentInterface == Interfaces.Tool)
            {
                InterfaceHandler.instance.OpenInterface(Interfaces.Item, true, false, true);
                itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
                itemWheel.enabled = true;
                itemAnim.SetBool("OpenWheel", true);
            }
            else if(InterfaceHandler.instance.currentInterface == Interfaces.Item)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                itemWheel.GetComponent<ItemWheelUI>().UpdateItemWheelUI();
                itemAnim.SetBool("OpenWheel", true);
            }
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.Item)
        {
            itemWheel.GetComponent<ItemWheelUI>().SwapTool();
            CloseItemWheelAnim();
        }
    }

    private void CloseItemWheelAnim()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (itemAnim.GetBool("OpenWheel"))
        {
            itemAnim.SetBool("OpenWheel", false);
            itemAnim.Play("Close", 0, 0.3f);
        }

        closeItemWheel = true;
    }

    public void PlayerInventoryHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (InterfaceHandler.instance.currentInterface == Interfaces.None || InterfaceHandler.instance.currentInterface == Interfaces.Tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.Storage, true, false, false);
            InventoryHandler.instance.UpdateInventoryUI();
            storage.enabled = true;
        }
        else if(ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.Storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }
}
