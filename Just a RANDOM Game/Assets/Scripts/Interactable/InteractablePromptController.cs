using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using TMPro;
using System;

public class InteractablePromptController : MonoBehaviour
{
    public static InteractablePromptController instance;

    public Image crosshair;

    [Header("Prompt Image")]
    public Image prompt;
    public Sprite itemPicking;
    public Sprite botInterface;

    [Header("Picked Drop")]
    public Transform pickedDropParent;
    public GameObject pickedDropPrefab;
    public float pickedDropHintTime = 1.2f;
    public Sprite[] pickedDropColors = new Sprite[4];
    /* White: Material
     * Blue: Tool & Armor
     * Orange: Consumable
     * Magenta: Special
     */

    private ItemDatabase database;
    private List<int> pickedDrops = new List<int>();

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
        database = PlayerItemController.instance.database;

        prompt.enabled = false;
        crosshair.enabled = false;
    }

    public void OpenPrompt(Interactable interactableType)
    {
        EnablePrompt();
        if (interactableType.GetType() == typeof(ItemInteractable))
        {
            prompt.sprite = itemPicking;
        }
        else if(interactableType.GetType() == typeof(BotInteractable))
        {
            prompt.sprite = botInterface;
        }
    }

    public void AddDrop(int itemID, int count = 1)
    {
        if (pickedDrops.Contains(itemID))
        {
            TMP_Text countText = pickedDropParent.GetChild(pickedDrops.IndexOf(itemID)).GetChild(2).GetComponent<TMP_Text>();
            countText.text = "x" + (Int32.Parse(countText.text.Substring(1)) + count);
            return;
        }

        Item item = database.GetItem[itemID];

        Transform hint = Instantiate(pickedDropPrefab, pickedDropParent).transform;
        int type = (int)item.itemType;
        if(type >= 13 && type <= 16)
            hint.GetComponent<Image>().sprite = pickedDropColors[0];
        else if(type == 7 || type == 12)
            hint.GetComponent<Image>().sprite = pickedDropColors[2];
        else if(type == 17)
            hint.GetComponent<Image>().sprite = pickedDropColors[3];
        else
            hint.GetComponent<Image>().sprite = pickedDropColors[1];
        hint.GetChild(0).GetComponent<Image>().sprite = item.icon;
        hint.GetChild(1).GetComponent<TMP_Text>().text = item.itemName;
        hint.GetChild(2).GetComponent<TMP_Text>().text = "x" + count;

        pickedDrops.Add(itemID);
        StartCoroutine(RemoveDrop(hint.gameObject, pickedDropHintTime));
    }

    private IEnumerator RemoveDrop(GameObject drop, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(drop);
    }

    public void ResetPickedDrops()
    {
        pickedDrops.Clear();
    }

    public void DisablePrompt()
    {
        prompt.enabled = false;
    }

    private void EnablePrompt()
    {
        prompt.enabled = true;
    }

    public void DisableCrosshair()
    {
        crosshair.enabled = false;
    }

    public void EnableCrosshair()
    {
        crosshair.enabled = true;
    }
}
