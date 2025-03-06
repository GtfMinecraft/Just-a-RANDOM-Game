using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
    protected Animator anim;

    private void Start()
    {
        anim = PlayerController.instance.anim;
    }

    protected virtual void OnInteractionStart()
    {
        anim.SetInteger("PlayerAction", 2);
    }

    protected virtual void OnInteractionEnd()
    {
        if (anim.GetInteger("PlayerAction") == 2)
            anim.SetInteger("PlayerAction", 0);
    }
}
