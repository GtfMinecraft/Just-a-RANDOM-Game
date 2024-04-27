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

    private int currentTool = 1;
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

        string selectedToolJSON = JsonUtility.ToJson(currentTool);
        //File.WriteAllText(Application.dataPath + "/savingText.txt", selectedToolJSON);

        //Read from memory to get which type of tool was selected
    }

    public void SwapTool()
    {
        int section = GetSection(-60, 60, 6, freeDistance);
        if(section == -1)
        {
            return;
        }
        toolSelected[currentTool].enabled = false;
        toolSelected[section].enabled = true;
        currentTool = section;
        if(File.Exists(Application.dataPath + "/savingText.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/savingText.txt");
            int a = JsonUtility.FromJson<int>(saveString);
            Debug.Log(saveString);
            Debug.Log(a);
        }
    }
}
