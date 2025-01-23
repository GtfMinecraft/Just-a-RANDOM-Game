using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterfaceHandler : MonoBehaviour
{
    public static InterfaceHandler instance;

    private PlayerController player;
    public Canvas EscCanvas;
    private InventoryCanvasController inventoryCanvas;
    private TradingInterface trading;
    public BotInterface BotCanvas;

    public Interfaces currentInterface { get; private set; }

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
        player = PlayerController.instance;
        inventoryCanvas = InventoryCanvasController.instance;
        trading = TradingInterface.instance;
    }

    public void CloseAllInterface()
    {
        inventoryCanvas.CloseAllInventory();
        trading.CloseTradingInterface();
        EscCanvas.enabled = false;
        BotCanvas.CloseBotInterface();

        Time.timeScale = 1f;
        currentInterface = Interfaces.None;
        player.canMove = true;
        player.canRotate = true;
        player.canControl = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenInterface(Interfaces tmp, bool movement = true, bool rotation = true, bool control = true)
    {
        CloseAllInterface();
        currentInterface = tmp;
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
            if(currentInterface != Interfaces.None)
            {
                CloseAllInterface();
            }
            else
            {
                OpenInterface(Interfaces.Esc, false, false, false);
                Time.timeScale = 0;
                EscCanvas.enabled = true;
            }
        }
    }
}
