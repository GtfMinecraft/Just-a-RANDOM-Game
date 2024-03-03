using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryCanvasController : MonoBehaviour
{
    [SerializeField]
    private InventoryTypes currentInventory;

    private Canvas[] inventoryList = new Canvas[6];

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0;i<inventoryList.Length; i++)
        {
            inventoryList[i] = transform.GetChild(i).GetComponent<Canvas>();
            inventoryList[i].enabled = false;
        }
    }

    public void ChangeToolInventory(InventoryTypes tmp)
    {
        InterfaceHandler.instance.CloseAllInterface();
        currentInventory = tmp;
    }

    public void SwapHandItem()
    {
        //swap hand item
    }

    public void CloseAllInventory()
    {
        for(int i = 0; i < inventoryList.Length; ++i)
        {
            inventoryList[i].enabled = false;
        }
    }

    public void ItemWheelHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.none)
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.tool, true);
            inventoryList[(int)currentInventory].enabled = true;
        }
        else if (ctx.canceled && InterfaceHandler.instance.currentInterface == Interfaces.tool)
        {
            SwapHandItem();
            InterfaceHandler.instance.CloseAllInterface();
        }
    }

    public void PlayerInventoryHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.none)
        {
            InterfaceHandler.instance.OpenInterface(Interfaces.storage);
            inventoryList[0].enabled = true;
        }
        else if(ctx.performed && InterfaceHandler.instance.currentInterface == Interfaces.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }
}
