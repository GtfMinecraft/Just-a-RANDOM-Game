using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryCanvasController : MonoBehaviour
{
    public static InventoryCanvasController instance;

    [SerializeField]
    private InventoryTypes currentInventory;

    private Canvas[] inventoryList;

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
        inventoryList = new Canvas[transform.childCount];

        for (int i=0;i<inventoryList.Length; i++)
        {
            inventoryList[i] = transform.GetChild(i).GetComponent<Canvas>();
            inventoryList[i].enabled = false;
        }
        currentInventory = (InventoryTypes)PlayerPrefs.GetInt("selectedTool");
    }

    public void ChangeToolInventory(InventoryTypes inv)
    {
        InterfaceHandler.instance.CloseAllInterface();
        currentInventory = inv;
    }

    public void CloseAllInventory()
    {
        for(int i = 0; i < inventoryList.Length; ++i)
        {
            inventoryList[i].enabled = false;
        }
    }

    public void ToolWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if(ctx.valueType == typeof(Vector2) && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.item))
            {
                InterfaceHandler.instance.CloseAllInterface();
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
                InterfaceHandler.instance.OpenInterface(Interfaces.tool);
                inventoryList[inventoryList.Length-1].enabled = true;
            }
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.tool)
        {
            inventoryList[inventoryList.Length - 1].GetComponent<ToolWheelUI>().SwapTool();
            InterfaceHandler.instance.CloseAllInterface();
        }
    }

    public void ItemWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && currentInventory != InventoryTypes.storage && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.item);
            inventoryList[(int)currentInventory].enabled = true;
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.item)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }

    public void PlayerInventoryHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && (InterfaceHandler.instance.currentInterface == Interfaces.none || InterfaceHandler.instance.currentInterface == Interfaces.tool))
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.storage, true, false, true);
            inventoryList[(int)InventoryTypes.storage].enabled = true;
        }
        else if(ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }
}
