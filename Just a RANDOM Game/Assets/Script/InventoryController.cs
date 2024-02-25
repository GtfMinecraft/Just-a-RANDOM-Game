using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageController : MonoBehaviour
{
    private Image[] inventoryList = new Image[6];

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().enabled = false;
        for (int i=0;i<inventoryList.Length; i++)
        {
            inventoryList[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (GetComponent<Canvas>().enabled)
            {
                GetComponent<Canvas>().enabled = false;
            }
            else
            {
                GetComponent<Canvas>().enabled = true;
            }
        }
    }
}
