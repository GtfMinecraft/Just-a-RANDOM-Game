using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePromptController : MonoBehaviour
{
    public static InteractablePromptController instance;

    public Canvas itemPickingCanvas;
    private Canvas currentCanvas;

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
        if(interactableType.GetType() == typeof(ItemInteractable))
        {
            EnableCanvas(itemPickingCanvas);
        }
        else if(interactableType.GetType() == typeof(MerchantInteractable))
        {

        }
    }

    public void ClosePrompt()
    {
        if(currentCanvas != null)
        {
            currentCanvas.enabled = false;
            currentCanvas = null;
        }
    }

    private void EnableCanvas(Canvas canvas)
    {
        if (!canvas.enabled)
        {
            ClosePrompt();
            canvas.enabled = true;
            currentCanvas = canvas;
        }
    }
}
