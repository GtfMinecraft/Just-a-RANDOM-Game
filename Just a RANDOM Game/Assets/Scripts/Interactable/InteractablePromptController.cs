using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class InteractablePromptController : MonoBehaviour
{
    public static InteractablePromptController instance;

    public Image prompt;

    [Header("Prompt Image")]
    public Sprite itemPicking;
    public Sprite botInterface;
    public Sprite merchantInterface;

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
        GetComponent<Canvas>().enabled = false;
    }

    public void OpenPrompt(Interactable interactableType)
    {
        EnableCanvas();
        if (interactableType.GetType() == typeof(ItemInteractable))
        {
            prompt.sprite = itemPicking;
        }
        else if(interactableType.GetType() == typeof(BotInteractable))
        {
            prompt.sprite = botInterface;
        }
        else if(interactableType.GetType() == typeof(MerchantInteractable))
        {
            prompt.sprite = merchantInterface;
        }
    }

    public void DisableCanvas()
    {
        GetComponent<Canvas>().enabled = false;
    }

    private void EnableCanvas()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
