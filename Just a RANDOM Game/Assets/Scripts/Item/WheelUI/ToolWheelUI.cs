using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ToolWheelUI : WheelUI
{
    public Transform toolWheel;
    public float freeDistance;

    [Header("Scroll Tool Image")]
    public Canvas scrollWheel;
    public GameObject[] scrollToolImages = new GameObject[6];
    public float scrollToolDuration = 0.7f;

    private float scrollToolTimer = 0;

    private Animator anim;

    private int hoveredTool = -1;//InventoryTypes - 1
    private int currentTool = 0;//InventoryTypes
    private Image[] toolImages = new Image[6];
    private GameObject[] toolSelected = new GameObject[6];

    private void Start()
    {
        anim = toolWheel.GetComponent<Animator>();

        for(int i = 0; i < toolImages.Length; i++)
        {
            toolImages[i] = toolWheel.GetChild(i).GetComponent<Image>();
            toolImages[i].transform.GetChild(0).gameObject.SetActive(false);
            toolSelected[i] = toolWheel.GetChild(toolWheel.childCount - 1).transform.GetChild(i).gameObject;
            toolSelected[i].SetActive(false);
        }
    }

    private void Update()
    {
        if(anim.GetBool("OpenWheel"))
        {
            int tool = ToolGetSection();
            if (hoveredTool != tool)
            {
                if(hoveredTool != -1)
                {
                    toolImages[hoveredTool].GetComponent<ToolWheelUIHover>().hovered = false;
                }
                hoveredTool = tool;
                if(hoveredTool != -1)
                {
                    toolImages[hoveredTool].GetComponent<ToolWheelUIHover>().hovered = true;
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
        int section = ToolGetSection();
        if (section == -1 || section == currentTool - 1)
        {
            return;
        }

        PlayerItemController.instance.ChangeInventory((InventoryTypes)(section + 1));
    }

    public void UpdateToolWheelUI()
    {
        if (InterfaceHandler.instance.currentInterface != Interfaces.tool)
            return;

        if (currentTool != (int)PlayerItemController.instance.currentInventory)
        {
            if (currentTool != 0)
                toolSelected[currentTool - 1].SetActive(false);
            currentTool = (int)PlayerItemController.instance.currentInventory;
            if (currentTool != 0)
                toolSelected[currentTool - 1].SetActive(true);
        }
    }

    public void ScrollToolImage(int inv)
    {
        if(inv == 0)
        {
            scrollWheel.enabled = false;
            return;
        }

        scrollToolTimer = Time.time;

        foreach (var image in scrollToolImages)
        {
            image.SetActive(false);
        }
        scrollToolImages[inv - 1].SetActive(true);
        scrollWheel.enabled = true;
        CloseScrollToolImage();
    }

    private async void CloseScrollToolImage()
    {
        await Task.Delay((int)(scrollToolDuration * 1000));
        if(Time.time - scrollToolTimer >= scrollToolDuration - 0.05f)
            scrollWheel.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, freeDistance);
    }
}
