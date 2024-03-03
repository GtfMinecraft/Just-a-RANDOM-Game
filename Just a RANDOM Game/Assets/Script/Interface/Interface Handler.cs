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

        instance.currentInterface = Interfaces.none;
        player.canMove = true; 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenInterface(Interfaces tmp, bool playerMovement = false)
    {
        instance.currentInterface = tmp;
        if(playerMovement == false)
        {
            player.canMove = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EscHandler(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (currentInterface == Interfaces.esc)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                instance.currentInterface = Interfaces.none;
                Time.timeScale = 1f;
                player.canMove = true;
                EscCanvas.enabled = false;
            }
            else if(currentInterface != Interfaces.none)
            {
                player.canMove = true;
                CloseAllInterface();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                instance.currentInterface = Interfaces.esc;
                Time.timeScale = 0;
                player.canMove = false;
                EscCanvas.enabled = true;
            }
        }
    }
}
