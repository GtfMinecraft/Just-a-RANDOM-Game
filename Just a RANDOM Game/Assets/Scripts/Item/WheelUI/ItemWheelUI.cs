using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemWheelUI : WheelUI
{
    public float freeDistance;
    public float itemWheelDistance;
    public int subsectionDeg;
    public Transform itemWheelTransform;
    public ItemWheel[] itemWheels = new ItemWheel[6];

    [Header("Section Background")]
    public Sprite oneWideSectionImage;
    public Sprite twoWideSectionImage;
    public Sprite subsectionImage;

    private Animator anim;

    private InventoryTypes currentInventory;

    private int currentItem = 0;//default item if none is selected
    private int currentItem2 = 0;

    private int hoveredSection = -1;
    private int currentSection = -1; // come from currentItem from PlayerItemController
    private int curreentSubsection = -1;

    private ItemDatabase database;

    private Image[] sectionImages = new Image[12];
    private List<Image>[] itemImages = new List<Image>[12];
    private List<Image>[] itemSelected = new List<Image>[12];
    private TextMeshProUGUI[] stacks = new TextMeshProUGUI[12];
    
    [CreateAssetMenu(fileName = "New Item Wheel", menuName = "Inventory/Item Wheel")]
    public class ItemWheel : ScriptableObject
    {
        public List<int> itemID = new List<int>();
        public Sprite[] groupImages = new Sprite[12];
        public Subsection[] subsection = new Subsection[6];

        private int[] stacks;

        [System.Serializable]
        public struct Subsection
        {
            public int firstSection;
            public int secondSection;

            public int TotalItemCount()
            {
                return firstSection + secondSection;
            }
        }

        public List<int>[] SectionItemIndex(int section, int count = -1)
        {
            if(count == -1)
            {
                count = 0;
                for (int i = 0; i < section; ++i)
                {
                    count += subsection[i].TotalItemCount();
                }
            }

            List<int>[] sectionItemIndex = new List<int>[2];
            for (int i = 0; i < subsection[section].firstSection; ++i)
            {
                if (stacks[count + i] > 0)
                {
                    sectionItemIndex[0].Add(count + i);
                }
            }
            for (int i = subsection[section].firstSection; i < subsection[section].secondSection; ++i)
            {
                if (stacks[count + i] > 0)
                {
                    sectionItemIndex[1].Add(count + i);
                }
            }
            return sectionItemIndex;
        }

        public void RefreshStack()
        {
            stacks = new int[itemID.Count];

            for(int i=0;i < itemID.Count; ++i)
            {
                stacks[i] = InventoryHandler.instance.resources[itemID[i]];
            }
        }
    }

    private void Start()
    {
        database = PlayerItemController.instance.database;
        anim = itemWheelTransform.GetComponent<Animator>();
        
        for(int i=0;i<itemImages.Length; ++i)
        {
            sectionImages[i] = itemWheelTransform.GetChild(i).GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (!anim.GetBool("OpenItemWheel"))
        {
            return;
        }

        float mouseDistance = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2).magnitude;
        if (mouseDistance < freeDistance && hoveredSection != -1)
        {
            CloseSubsection();
            hoveredSection = -1;
        }
        else if (mouseDistance >= freeDistance && mouseDistance < itemWheelDistance)
        {
            int section = ItemGetSection();
            if (hoveredSection != section)
            {
                if(hoveredSection != -1)
                {
                    //close hoveredSection
                }
                //open section if has subsection
                hoveredSection = section;
            }
        }
    }

    protected int ItemGetSection()
    {
        return GetSection(-60, 30, 12, freeDistance);
    }

    protected int GetSubsectionItemIndex()
    {
        List<int>[] sectionItemIndex = itemWheels[(int)currentInventory].SectionItemIndex(hoveredSection / 2);
        int[] itemCount = new int[2] { sectionItemIndex[0].Count, sectionItemIndex[1].Count };

        int firstOrSecond = hoveredSection % 2;
        int sectionCount = itemCount[firstOrSecond];
        int startAngle = (firstOrSecond == 0) ? 15 : 45;

        if (itemCount[0] == 0 && itemCount[1] == 0)
        {
            return -1;
        }
        else if (sectionCount == 0)
        {
            firstOrSecond ^= 1;
            sectionCount = itemCount[firstOrSecond];
            startAngle = 30;
        }
        else if (itemCount[firstOrSecond ^ 1] == 0)
        {
            startAngle = 30;
        }

        if (sectionItemIndex[firstOrSecond].Count == 1)
        {
            return sectionItemIndex[firstOrSecond][0];
        }

        int subsection = GetSection(hoveredSection / 2 * 60 - 60 + startAngle - sectionCount * subsectionDeg / 2, subsectionDeg, sectionCount, itemWheelDistance);

        return sectionItemIndex[firstOrSecond][subsection];
    }

    public void SwapTool()
    {
        if(hoveredSection == -1)
        {
            return;
        }

        int itemIndex = GetSubsectionItemIndex();
        if (itemIndex == -1)
        {
            CloseSubsection();
            hoveredSection = ItemGetSection();
            return;
        }

        int itemID = itemWheels[(int)currentInventory].itemID[itemIndex];
        int holdItemResult = PlayerItemController.instance.HoldItem(itemID);
        if (holdItemResult == 1 && currentItem != itemID)
        {
            //put selected
            currentItem = itemID;
        }
        else if (holdItemResult == 2 && currentItem2 != itemID) 
        {
            //put second selected
            currentItem2 = itemID;
        }
    }

    protected void CloseSubsection()
    {
        
    }

    protected void SelectSection()
    {

    }

    public void UpdateItemWheelUI(int item = 0)
    {
        if(InterfaceHandler.instance.currentInterface != Interfaces.item)
        {
            return;
        }

        currentInventory = PlayerItemController.instance.currentInventory;

        if(currentInventory == InventoryTypes.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
        }

        if (item != 0 && !itemWheels[(int)currentInventory - 1].itemID.Contains(item))
        {
            return;
        }

        itemWheels[(int)currentInventory].RefreshStack();

        currentItem = PlayerItemController.instance.rightItem;
        currentItem2 = PlayerItemController.instance.leftItem;

        SelectSection();

        int count = 0;
        for (int i = 0; i < itemImages.Length; i++)
        {
            List<int>[] indexList = itemWheels[(int)currentInventory].SectionItemIndex(i, count);
            count += itemWheels[(int)currentInventory].subsection[i].TotalItemCount();

            if (indexList[0].Count == 0 && indexList[1].Count == 0)
            {
                sectionImages[2 * i].gameObject.SetActive(false);
                sectionImages[2 * i + 1].gameObject.SetActive(false);
            }
            else if (indexList[0].Count == 0)
            {
                sectionImages[2 * i + 1].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3((float)Math.Cos(60 * i - 30), (float)Math.Sin(60 * i - 30), 0);
                sectionImages[2 * i + 1].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 120);
                sectionImages[2 * i + 1].sprite = twoWideSectionImage;

                if (indexList[1].Count == 1)
                {
                    sectionImages[2 * i + 1].transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[itemWheels[(int)currentInventory].itemID[indexList[1][0]]].icon;
                }
                else
                {

                }
  
                sectionImages[2 * i].gameObject.SetActive(false);
                sectionImages[2 * i + 1].gameObject.SetActive(true);
            }
            else if (indexList[1].Count == 0)
            {
                sectionImages[2 * i].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3((float)Math.Cos(60 * i - 30), (float)Math.Sin(60 * i - 30), 0);
                sectionImages[2 * i].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 120);
                sectionImages[2 * i].sprite = twoWideSectionImage;

                if (indexList[0].Count == 1)
                {
                    sectionImages[2 * i].transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[itemWheels[(int)currentInventory].itemID[indexList[0][0]]].icon;
                }
                else
                {

                }

                sectionImages[2 * i].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(false);
            }
            else
            {
                sectionImages[2 * i].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3((float)Math.Cos(60 * i - 45), (float)Math.Sin(60 * i - 45), 0);
                sectionImages[2 * i + 1].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3((float)Math.Cos(60 * i - 15), (float)Math.Sin(60 * i - 15), 0);
                sectionImages[2 * i].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 150);
                sectionImages[2 * i + 1].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 90);
                sectionImages[2 * i].sprite = sectionImages[2 * i + 1].sprite = oneWideSectionImage;

                sectionImages[2 * i ].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(true);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(Vector3.zero, freeDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, itemWheelDistance);
    }
}
