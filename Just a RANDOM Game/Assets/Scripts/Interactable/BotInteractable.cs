using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInteractable : Interactable
{
    private float interactTime = 0.3f; // *** bot interface time has to be in sync with anim

    public override void Interact()
    {
        OnInteractionStart();
    }

    private void OnInteractionStart()
    {
        InterfaceHandler.instance.BotCanvas.GetComponent<BotInterface>().OpenBotInterface();
        PlayerController.instance.forcedInteraction = true;
        //bot anim
        Invoke("OnInteractionEnd", interactTime);
    }

    private void OnInteractionEnd()
    {
        //bot animation reset
    }
}
