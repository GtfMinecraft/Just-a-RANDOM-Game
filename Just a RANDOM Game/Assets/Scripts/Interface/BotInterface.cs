using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotInterface : MonoBehaviour, IDataPersistence
{
    [Header("Crafting")]
    public Color faintCraftIcon;
    public Transform craftBackground;
    public Transform itemHover;
    public GameObject craftingIconPrefab;
    public GameObject materialIconPrefab;

    private ItemDatabase database;
    private UDictionaryIntInt resources;
    private List<int> unlockedCrafts;
    private int[] replenishIndexes;

    private Transform materialParent;

    // Start is called before the first frame update
    private void Start()
    {
        database = PlayerItemController.instance.database;
        resources = InventoryHandler.instance.resources;

        GetComponent<Canvas>().enabled = false;
        itemHover.gameObject.SetActive(false);

        materialParent = itemHover.GetChild(1);

        InstantiateCraft();
    }

    public void UnLockCraft(int itemID)
    {
        if (unlockedCrafts.Contains(itemID))
            return;

        unlockedCrafts.Add(itemID);
        SetCraftIcon(itemID);
    }

    private void InstantiateCraft()
    {
        for (int i = 0; i < unlockedCrafts.Count; i++)
        {
            SetCraftIcon(unlockedCrafts[i]);
        }
    }

    private void SetCraftIcon(int itemID)
    {
        Item item = database.GetItem[itemID];
        GameObject icon = Instantiate(craftingIconPrefab, craftBackground);
        icon.transform.GetChild(0).GetComponent<Image>().sprite = item.icon;

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((eventData) => { ShowItemHover(icon.transform); });
        icon.GetComponent<EventTrigger>().triggers[0] = entry1;

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerClick;
        entry2.callback.AddListener((eventData) => { /* detect if craftable (use image color) + remove materials and add crafted item to inv + play bot anim */ });
        icon.GetComponent<EventTrigger>().triggers[1] = entry2;
    }

    public void ShowItemHover(Transform sender)
    {
        int itemID = unlockedCrafts[sender.GetSiblingIndex()];
        InstantiateMaterial(itemID);

        itemHover.position = sender.position;
        itemHover.GetChild(0).GetComponent<TMP_Text>().text = database.GetItem[itemID].itemDescription;
        itemHover.gameObject.SetActive(true);
    }

    public void HideItemHover()
    {
        itemHover.gameObject.SetActive(false);
    }

    private void InstantiateMaterial(int itemID)
    {
        UDictionaryIntInt recipe = database.GetItem[itemID].recipe;

        for (int i = 0; i < itemHover.GetChild(1).childCount; i++)
            Destroy(itemHover.GetChild(1).GetChild(i).gameObject);

        foreach (KeyValuePair<int, int> entry in recipe)
        {
            GameObject icon = Instantiate(materialIconPrefab, materialParent);
            icon.transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[entry.Key].icon;
            icon.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = entry.Value.ToString();
        }
    }

    private void UpdateCraftMenu()
    {
        for (int i = 0; i < unlockedCrafts.Count; ++i)
        {
            Item item = database.GetItem[unlockedCrafts[i]];
            UDictionaryIntInt recipe = item.recipe;
            bool craftable = true;

            foreach (KeyValuePair<int, int> entry in recipe)
            {
                if (resources.ContainsKey(entry.Key) && resources[entry.Key] < entry.Value)
                {
                    craftable = false;
                    break;
                }
            }

            craftBackground.GetChild(i).GetChild(0).GetComponent<Image>().color = craftable ? Color.white : faintCraftIcon;
        }
    }

    public void OpenBotInterface()
    {
        InterfaceHandler.instance.OpenInterface(Interfaces.Bot, false, false, false);
        UpdateCraftMenu();
        GetComponent<Canvas>().enabled = true;
    }

    public void CloseBotInterface()
    {
        GetComponent<Canvas>().enabled = false;
        PlayerController.instance.forcedInteraction = false;
    }

    public void LoadData(GameData data)
    {
        unlockedCrafts = data.botCraftData.unlockedCrafts;
    }

    public void SaveData(GameData data)
    {
        data.botCraftData.unlockedCrafts = unlockedCrafts;
    }
}
