using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TradingInterface : MonoBehaviour
{
    public static TradingInterface instance;

    [SerializeField]
    private Trade currentTrade;
    private bool isTrade = false;

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

    public void CloseTradingInterface()
    {
        if(InterfaceHandler.instance.currentInterface == Interfaces.trading)
        {
            InterfaceHandler.instance.CloseAllInterface();
            transform.GetComponent<Canvas>().enabled = false;
            isTrade = false;
        }
    }

    public void OpenTradeInterface()
    {
        if(InterfaceHandler.instance.currentInterface == Interfaces.none)
        {
            RefreshTradingInterface();
            InterfaceHandler.instance.OpenInterface(Interfaces.trading, false, false, false);
            transform.GetComponent<Canvas>().enabled = true;
            isTrade = true;
        }
    }

    public void RefreshTradingInterface()
    {
        for(int i = 0; i < currentTrade.offers.Length; i++)
        {
            if (currentTrade.offers[i].show)
            {
                //UI
            }
        }
    }

    public void TradingHandler(InputAction.CallbackContext ctx)
    {

    }
}
