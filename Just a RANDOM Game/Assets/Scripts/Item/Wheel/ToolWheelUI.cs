using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToolWheelUI : WheelUI
{
    public float freeDistance;

    private int currentTool = 0;//InventoryTypes
    private Image[] toolImages = new Image[6];
    private Image[] toolSelected = new Image[6];

    private void Start()
    {
        for(int i = 0; i < toolImages.Length; i++)
        {
            toolImages[i] = transform.GetChild(i).GetComponent<Image>();
            toolSelected[i] = toolImages[i].transform.GetChild(1).GetComponent<Image>();
            toolSelected[i].enabled = false;
        }

        currentTool = PlayerPrefs.GetInt("selectedTool");
        if(currentTool != 0)
        {
            toolSelected[currentTool - 1].enabled = true;
            PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        }
    }

    public void SwapTool()
    {
        int section = GetSection(-60, 60, 6, freeDistance);
        if(section == -1)
        {
            return;
        }
        if(currentTool != section + 1)
        {
            if(currentTool != 0)
            {
                toolSelected[currentTool - 1].enabled = false;
            }
            toolSelected[section].enabled = true;
            currentTool = section + 1;
            PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        }
        else
        {
            toolSelected[section].enabled = false;
            currentTool = 0;
            PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        }
    }
}
