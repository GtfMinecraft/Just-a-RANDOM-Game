using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterfaceHandler : MonoBehaviour
{
    public static InterfaceHandler instance;

    public PlayerController player;
    public Canvas EscCanvas;
    public InventoryCanvasController inventoryCanvas;
    //trading
    public Interfaces currentInterface;

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

    public void CloseAllInterface()
    {
        inventoryCanvas.CloseAllInventory();
        //close trading interface
        EscCanvas.enabled = false;

        Time.timeScale = 1f;
        instance.currentInterface = Interfaces.none;
        player.canMove = true;
        player.canRotate = true;
        player.canControl = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenInterface(Interfaces tmp, bool movement = true, bool rotation = true, bool control = true)
    {
        CloseAllInterface();
        instance.currentInterface = tmp;
        if(movement == false)
        {
            player.canMove = false;
        }
        if(rotation == false)
        {
            player.canRotate = false;
        }
        if(control == false)
        {
            player.canControl = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EscHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if(currentInterface != Interfaces.none)
            {
                CloseAllInterface();
            }
            else
            {
                OpenInterface(Interfaces.esc, false, false, false);
                Time.timeScale = 0;
                EscCanvas.enabled = true;
            }
        }
    }
}
