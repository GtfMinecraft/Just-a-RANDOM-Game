using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWheelUI : WheelUI
{
    public float freeDistance;

    private int currentItem = 0;//default item if none is selected
    private Image[][] itemImages = new Image[6][];
    private Image[][] itemSelected = new Image[6][];

    private void Start()
    {
        //for(int i=0; i<6; i++)
        //{
        //    for (int j = 0; j < itemImages.Length; j++)
        //    {
        //        itemImages[i] = transform.GetChild(i).GetComponent<Image>();
        //        itemSelected[i] = itemImages[i].transform.GetChild(1).GetComponent<Image>();
        //        itemSelected[i].enabled = false;
        //    }
        //}
        
        //currentTool = PlayerPrefs.GetInt("selectedTool");
        //if (currentTool != 0)
        //{
        //    toolSelected[currentTool - 1].enabled = true;
        //}
    }

    public void SwapTool()
    {
        //int section = GetSection(-60, 60, 6, freeDistance);
        //if (section == -1)
        //{
        //    return;
        //}
        //if (currentTool != section + 1)
        //{
        //    if (currentTool != 0)
        //    {
        //        toolSelected[currentTool - 1].enabled = false;
        //    }
        //    toolSelected[section].enabled = true;
        //    currentTool = section + 1;
        //    PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        //}
        //else
        //{
        //    toolSelected[section].enabled = false;
        //    currentTool = 0;
        //    PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
        //}
    }
}
