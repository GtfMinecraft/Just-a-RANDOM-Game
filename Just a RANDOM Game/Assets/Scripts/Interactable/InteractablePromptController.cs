using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePromptController : MonoBehaviour
{
    public static InteractablePromptController instance;

    public Canvas itemPickingCanvas;

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
        itemPickingCanvas.enabled = false;
    }

    public void OpenPrompt(Interactable interactableType)
    {
        if(interactableType.GetType() == typeof(ItemInteractable) && !itemPickingCanvas.enabled)
        {
            ClosePrompt();
            itemPickingCanvas.enabled = true;
        }
        else if(interactableType.GetType() == typeof(MerchantInteractable))
        {

        }
    }

    public void ClosePrompt()
    {
        itemPickingCanvas.enabled = false;
    }
}
