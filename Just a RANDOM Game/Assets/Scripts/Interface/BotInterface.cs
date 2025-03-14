using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class BotInterface : MonoBehaviour, IDataPersistence
{
    [Header("Crafting")]
    public Color faintCraftIcon;
    public Material glowMaterial;
    public Transform craftBackground;
    public Transform itemHover;
    public GameObject craftingIconPrefab;
    public GameObject materialIconPrefab;
    public Image clickTint;
    public Color successCraftColor;
    public Color failCraftColor;

    private ItemDatabase database;
    private UDictionaryIntInt resources;
    private List<int> unlockedCrafts;
    private int[] replenishIndexes;

    private Transform hoveredCraft;
    private Transform clickedCraft;

    // Start is called before the first frame update
    private void Start()
    {
        database = PlayerItemController.instance.database;
        resources = InventoryHandler.instance.resources;

        GetComponent<Canvas>().enabled = false;
        itemHover.gameObject.SetActive(false);

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
        Transform icon = Instantiate(craftingIconPrefab, craftBackground).transform;
        icon.GetChild(0).GetComponent<Image>().sprite = item.icon;

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener((eventData) => { 
            ShowItemHover(icon);
            if (hoveredCraft != null && hoveredCraft != clickedCraft)
                hoveredCraft.GetComponent<Image>().material = hoveredCraft.GetChild(0).GetComponent<Image>().material = null;
            hoveredCraft = icon;
            icon.GetComponent<Image>().material = icon.GetChild(0).GetComponent<Image>().material = glowMaterial;
        });
        icon.GetComponent<EventTrigger>().triggers[0] = entry1;

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerClick;
        entry2.callback.AddListener((eventData) => {
            PointerEventData pointer = (PointerEventData)eventData;
            if (pointer.button == PointerEventData.InputButton.Left)
                CraftItem(icon);
        });
        icon.GetComponent<EventTrigger>().triggers[1] = entry2;

        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerDown;
        entry3.callback.AddListener((eventData) => {
            PointerEventData pointer = (PointerEventData)eventData;
            if (pointer.button == PointerEventData.InputButton.Left)
            {
                clickTint.color = (icon.GetComponent<Image>().color == Color.white) ? successCraftColor : failCraftColor;
                clickTint.enabled = true;
                clickTint.transform.position = icon.position;
                clickedCraft = icon;
            }
        });
        icon.GetComponent<EventTrigger>().triggers[2] = entry3;

        EventTrigger.Entry entry4 = new EventTrigger.Entry();
        entry4.eventID = EventTriggerType.PointerUp;
        entry4.callback.AddListener((eventData) => {
            PointerEventData pointer = (PointerEventData)eventData;
            if (pointer.button == PointerEventData.InputButton.Left)
            {
                clickTint.enabled = false;
                if (clickedCraft != hoveredCraft)
                    clickedCraft.GetComponent<Image>().material = clickedCraft.GetChild(0).GetComponent<Image>().material = null;
                clickedCraft = null;
            }
        });
        icon.GetComponent<EventTrigger>().triggers[3] = entry4;
    }

    public void ShowItemHover(Transform sender)
    {
        int itemID = unlockedCrafts[sender.GetSiblingIndex()];
        InstantiateMaterial(itemID);

        Item item = database.GetItem[itemID];

        itemHover.position = sender.position;
        itemHover.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        itemHover.GetChild(1).GetComponent<TMP_Text>().text = item.itemDescription;
        itemHover.gameObject.SetActive(true);
    }

    public void CraftItem(Transform sender)
    {
        if (sender.GetChild(0).GetComponent<Image>().color == Color.white)
        {
            Item item = database.GetItem[unlockedCrafts[sender.GetSiblingIndex()]];
            UDictionaryIntInt recipe = item.recipe;

            foreach (KeyValuePair<int, int> entry in recipe)
            {
                InventoryHandler.instance.RemoveItem(entry.Key, entry.Value);
            }
            //play item anim to bot and play bot anim according to item then go back to player

            //need to adjust item spawn position
            ItemDropHandler.instance.SpawnNewDrop(item.ID, transform.parent.position, false);
        }
    }

    public void HideItemHover()
    {
        if(hoveredCraft != null && hoveredCraft != clickedCraft)
        {
            hoveredCraft.GetComponent<Image>().material = hoveredCraft.GetChild(0).GetComponent<Image>().material = null;
            hoveredCraft = null;
        }
        itemHover.gameObject.SetActive(false);
    }

    private void InstantiateMaterial(int itemID)
    {
        UDictionaryIntInt recipe = database.GetItem[itemID].recipe;

        for (int i = 0; i < itemHover.GetChild(2).childCount; i++)
            Destroy(itemHover.GetChild(2).GetChild(i).gameObject);

        foreach (KeyValuePair<int, int> entry in recipe)
        {
            GameObject icon = Instantiate(materialIconPrefab, itemHover.GetChild(2));
            icon.transform.GetComponent<Image>().sprite = database.GetItem[entry.Key].icon;
            icon.transform.GetChild(0).GetComponent<TMP_Text>().text = entry.Value.ToString();
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
                if (!resources.ContainsKey(entry.Key) || resources[entry.Key] < entry.Value)
                {
                    craftable = false;
                    break;
                }
            }

            craftBackground.GetChild(i).GetComponent<Image>().color = craftable ? Color.white : faintCraftIcon;
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
        HideItemHover();
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
