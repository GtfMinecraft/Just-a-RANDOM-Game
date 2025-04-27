using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInteractable : Interactable
{
    public float botMaxDistance = 5;

    private float interactTime = 0f; // *** bot interface time has to be in sync with anim

    private void Update()
    {
        if (InterfaceHandler.instance.currentInterface == Interfaces.Bot && Vector3.Distance(PlayerController.instance.transform.position, transform.position) > botMaxDistance)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }
    }

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
