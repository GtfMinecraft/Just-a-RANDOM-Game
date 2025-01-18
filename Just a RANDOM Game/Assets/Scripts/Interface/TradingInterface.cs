using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TradingInterface : MonoBehaviour
{
    public static TradingInterface instance;

    [SerializeField]
    private Trade currentTrade;
    private bool isTrading = false;

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
        if(InterfaceHandler.instance.currentInterface == Interfaces.Trading)
        {
            InterfaceHandler.instance.CloseAllInterface();
            transform.GetComponent<Canvas>().enabled = false;
            isTrading = false;
        }
    }

    public void OpenTradeInterface(Trade trade)
    {
        if(InterfaceHandler.instance.currentInterface == Interfaces.None)
        {
            currentTrade = trade;
            InterfaceHandler.instance.OpenInterface(Interfaces.Trading, false, false, false);
            RefreshTradingInterface();
            transform.GetComponent<Canvas>().enabled = true;
            isTrading = true;
        }
    }

    public void RefreshTradingInterface()
    {
        if (InterfaceHandler.instance.currentInterface != Interfaces.Trading)
        {
            return;
        }

        for (int i = 0; i < currentTrade.offers.Length; i++)
        {
            if (currentTrade.offers[i].show)
            {
                //UI
            }
        }
    }
}
