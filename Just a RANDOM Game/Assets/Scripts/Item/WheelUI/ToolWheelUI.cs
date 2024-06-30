using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolWheelUI : WheelUI
{
    public Transform toolWheel;
    public float freeDistance;
    public Animator anim;

    private int hoveredTool = 0;
    private int currentTool = 0;//InventoryTypes
    private Image[] toolImages = new Image[6];
    private Image[] toolSelected = new Image[6];

    private void Start()
    {
        for(int i = 0; i < toolImages.Length; i++)
        {
            toolImages[i] = toolWheel.GetChild(i).GetComponent<Image>();
            toolSelected[i] = toolImages[i].transform.GetChild(1).GetComponent<Image>();
            toolSelected[i].enabled = false;
        }

        currentTool = PlayerPrefs.GetInt("selectedTool");
        if(currentTool != 0)
        {
            toolSelected[currentTool - 1].enabled = true;
        }
    }

    private void Update()
    {
        if(toolWheel.GetComponent<Animator>().GetBool("OpenToolWheel"))
        {
            if(currentTool != (int)PlayerItemController.instance.currentInventory)
            {
                if(currentTool != 0)
                    toolSelected[currentTool - 1].enabled = false;
                currentTool = (int)PlayerItemController.instance.currentInventory;
                if(currentTool != 0)
                    toolSelected[currentTool-1].enabled = true;
            }
            hoveredTool = ToolGetSection();
            for (int i = 0; i < 6; ++i)
            {
                if (i != hoveredTool)
                {
                    toolImages[i].GetComponent<ToolWheelUIHover>().hovered = false;
                }
                else
                {
                    toolImages[i].GetComponent<ToolWheelUIHover>().hovered = true;
                }
            }
        }
    }

    protected int ToolGetSection()
    {
        return GetSection(-60, 60, 6, freeDistance);
    }

    public void SwapTool()
    {
        int section = GetSection(-60, 60, 6, freeDistance);
        if (section == -1)
        {
            return;
        }
            
        if (currentTool != 0)
        {
            toolSelected[currentTool - 1].enabled = false;
        }
        toolSelected[section].enabled = true;
        currentTool = section + 1;
        PlayerItemController.instance.ChangeInventory((InventoryTypes)(currentTool));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, freeDistance);
    }
}
