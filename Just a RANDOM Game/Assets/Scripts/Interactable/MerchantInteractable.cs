using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantInteractable : Interactable
{
    public Trade trade;

    public override void Interact()
    {
        //conversation, story, etc.
        TradingInterface.instance.OpenTradeInterface(trade);
    }
}
