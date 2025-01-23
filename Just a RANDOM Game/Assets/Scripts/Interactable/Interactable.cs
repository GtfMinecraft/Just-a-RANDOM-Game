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
}
