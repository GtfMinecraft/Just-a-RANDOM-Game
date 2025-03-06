using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInteractable : Interactable
{
    private float interactTime = 0.05f; // *** bot interface time has to be in sync with anim

    public override void Interact()
    {
        OnInteractionStart();
    }

    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        InterfaceHandler.instance.BotCanvas.GetComponent<BotInterface>().OpenBotInterface();
        PlayerController.instance.forcedInteraction = true;
        //bot anim
        Invoke("OnInteractionEnd", interactTime);
    }

    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
    }
}
