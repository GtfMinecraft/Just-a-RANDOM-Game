using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotInterface : MonoBehaviour, IDataPersistence
{
    [Header("Resource Replenish")]
    public Sprite[] woodList;
    public Sprite[] oreList;
    public Sprite[] cropList;
    public Image[] resources;
    public Image[] arrows;

    [Header("Power")]
    public Sprite battery;

    [Header("Crafting")]
    public Image background;
    public Transform itemHover;
    public GameObject craftingSlotPrefab;

    private ItemDatabase database;
    private List<int> unlockedCrafts;
    private int[] spriteIndex;
    private bool next = false;

    // Start is called before the first frame update
    private void Start()
    {
        database = PlayerItemController.instance.database;

        GetComponent<Canvas>().enabled = false;

        string indexString = PlayerPrefs.GetString("resourceReplenish");
        spriteIndex = new int[indexString.Length];
        for(int i=0; i<resources.Length; i++)
        {
            spriteIndex[i] = indexString[i] - 48;
        }
        resources[0].sprite = woodList[spriteIndex[0]];
        resources[1].sprite = oreList[spriteIndex[1]];
        resources[2].sprite = cropList[spriteIndex[2]];

        for (int i = 0; i < arrows.Length; ++i)
            arrows[i].alphaHitTestMinimumThreshold = 0.1f;
    }

    public void ArrowNext(bool next)
    {
        this.next = next;
    }

    public void SwapResourceReplenish(int index)
    {
        if (index == 0)
        {
            spriteIndex[0] += next ? 1 : -1;
            if (spriteIndex[0] == woodList.Length)
                spriteIndex[0] -= woodList.Length;
            if (spriteIndex[0] == -1)
                spriteIndex[0] += woodList.Length;
            resources[0].sprite = woodList[spriteIndex[0]];
        }
        else if (index == 1)
        {
            spriteIndex[1] += next ? 1 : -1;
            if (spriteIndex[1] == oreList.Length)
                spriteIndex[1] -= oreList.Length;
            if (spriteIndex[1] == -1)
                spriteIndex[1] += oreList.Length;
            resources[1].sprite = oreList[spriteIndex[1]];
        }
        else if(index == 2)
        {
            spriteIndex[2] += next ? 1 : -1;
            if (spriteIndex[2] == cropList.Length)
                spriteIndex[2] -= cropList.Length;
            if (spriteIndex[2] == -1)
                spriteIndex[2] += cropList.Length;
            resources[2].sprite = cropList[spriteIndex[2]];
        }
    }

    public void InstantiateCraft()
    {
        for (int i = 0; i < unlockedCrafts.Count; i++)
        {
            List<int> recipe = database.GetItem[unlockedCrafts[i]].recipe;
            bool craftable = true;

            for(int j=0; j < recipe.Count; j++)
            {
                if (InventoryHandler.instance.resources[recipe[j]] == 0)
                {
                    craftable = false;
                    break;
                }
            }

            if (craftable)
            {
                Instantiate(craftingSlotPrefab, background.transform);
            }
            
        }
    }

    public void OpenBotInterface()
    {
        InterfaceHandler.instance.OpenInterface(Interfaces.Bot, false, false, false);
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
        string temp = "";
        for (int i = 0; i < spriteIndex.Length; i++)
        {
            temp += spriteIndex[i].ToString();
        }
        PlayerPrefs.SetString("resourceReplenish", temp);
    }
}
