using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ItemWheel;
using static System.Collections.Specialized.BitVector32;

public class ItemWheelUI : WheelUI
{
    public float freeDistance;
    public float itemWheelDistance;
    public float subsectionDistance;
    public int subsectionDeg;
    public Transform itemWheelTransform;
    public ItemWheel[] itemWheels = new ItemWheel[6];

    [Header("Section Background")]
    public Sprite oneWideSectionSprite;
    public Sprite twoWideSectionSprite;
    public Sprite subsectionSprite;

    private Animator anim;

    private ItemWheel currentWheel;
    private List<int>[] indexList;

    private int currentItem = 0;//default item if none is selected
    private int currentItem2 = 0;

    private int hoveredSection = -1;
    private int currentSection = -1; // come from currentItem from PlayerItemController
    private int curreentSubsection = -1;

    private ItemDatabase database;
    private UDictionaryIntInt resources;

    private Image[] sectionImages = new Image[12];
    private Image[][] subsectionImages = new Image[12][];
    private List<Image>[] itemSelected = new List<Image>[12];
    private TextMeshProUGUI[] stacks = new TextMeshProUGUI[12];

    private void Start()
    {
        database = PlayerItemController.instance.database;
        resources = InventoryHandler.instance.resources;
        anim = itemWheelTransform.GetComponent<Animator>();

        for (int i = 0; i < sectionImages.Length; ++i)
        {
            sectionImages[i] = itemWheelTransform.GetChild(i).GetComponent<Image>();
            sectionImages[i].gameObject.SetActive(false);

            subsectionImages[i] = new Image[5];
            for (int j = 0; j < 5; ++j)
            {
                subsectionImages[i][j] = sectionImages[i].transform.GetChild(1).GetChild(j).GetComponent<Image>();
                subsectionImages[i][j].gameObject.SetActive(false);
            }
            sectionImages[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!anim.GetBool("OpenWheel"))
        {
            return;
        }

        float mouseDistance = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2).magnitude;
        if (mouseDistance < freeDistance && hoveredSection != -1)
        {
            sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().hovered = false;
            hoveredSection = -1;
        }
        else if (mouseDistance >= freeDistance && mouseDistance < itemWheelDistance)
        {
            int section = ItemGetSection();
            if (hoveredSection != section)
            {
                if (hoveredSection != -1)
                {
                    sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().hovered = false;
                }
                hoveredSection = section;
                sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().hovered = true;
            }
        }
        else if (mouseDistance >= itemWheelDistance)
        {
            sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().subsectionHovered = GetSubsectionItemIndex(true);
        }
    }

    protected int ItemGetSection()
    {
        return GetSection(-60, 30, 12, freeDistance);
    }

    protected int GetSubsectionItemIndex(bool getSubsection = false)
    {
        List<int>[] sectionItemIndex = currentWheel.SectionItemIndex(hoveredSection / 2);
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
            if (!getSubsection)
                return sectionItemIndex[firstOrSecond][0];
            return -1;
        }

        int subsection = GetSection(hoveredSection / 2 * 60 - 60 + startAngle - sectionCount * subsectionDeg / 2, subsectionDeg, sectionCount, itemWheelDistance);

        if (getSubsection)
        {
            return subsection;
        }
        return (subsection == -1) ? -1 : sectionItemIndex[firstOrSecond][subsection];
    }

    public void SwapTool()
    {
        if (hoveredSection == -1)
        {
            return;
        }

        int itemIndex = GetSubsectionItemIndex();
        if (itemIndex == -1)
        {
            return;
        }

        int itemID = currentWheel.itemID[itemIndex];
        int holdItemResult = PlayerItemController.instance.HoldItem(itemID);
        if (holdItemResult == 1 && currentItem != itemID)
        {
            currentItem = itemID;
        }
        else if (holdItemResult == 2 && currentItem2 != itemID)
        {
            currentItem2 = itemID;
        }
    }

    protected void SetSubsection(int section, int subsection)
    {
        int count = indexList[subsection].Count;

        if (count == 1)
        {
            count = 0;

            sectionImages[2 * section + subsection].transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[currentWheel.itemID[indexList[0][0]]].icon;

            if (currentWheel.stacks[indexList[subsection][0]] == 1)
            {
                sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentWheel.stacks[indexList[subsection][0]].ToString();
            }
        }
        else
        {
            sectionImages[2 * section + subsection].transform.GetChild(0).GetComponent<Image>().sprite = currentWheel.groupImages[2 * section + subsection];
            sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        sectionImages[2 * section + subsection].transform.GetChild(0).rotation = Quaternion.identity;

        for (int i = count; i < 5; ++i)
        {
            subsectionImages[2 * section + subsection][i].gameObject.SetActive(false);
        }

        sectionImages[2 * section + subsection].transform.GetChild(1).SetPositionAndRotation(itemWheelTransform.position, Quaternion.identity);


        for (int i = 0; i < count; ++i)
        {
            Transform sectionTransform = sectionImages[2 * section + subsection].transform;

            subsectionImages[2 * section + subsection][i].transform.localPosition = (itemWheelDistance + subsectionDistance) / 2 * (Quaternion.AngleAxis(-subsectionDeg / 2 * (count - 1) + i * subsectionDeg, Vector3.forward) * sectionTransform.localPosition.normalized);
            subsectionImages[2 * section + subsection][i].transform.localRotation = sectionTransform.localRotation * Quaternion.Euler(0, 0, -subsectionDeg / 2 * (count - 1) + i * subsectionDeg);

            subsectionImages[2 * section + subsection][i].sprite = subsectionSprite;
            subsectionImages[2 * section + subsection][i].transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[currentWheel.itemID[indexList[subsection][i]]].icon;
            subsectionImages[2 * section + subsection][i].transform.GetChild(0).rotation = Quaternion.identity;

            if (currentWheel.stacks[indexList[subsection][i]] == 1)
            {
                subsectionImages[2 * section + subsection][i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                subsectionImages[2 * section + subsection][i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentWheel.stacks[indexList[subsection][i]].ToString();
            }

            subsectionImages[2 * section + subsection][i].gameObject.SetActive(true);
        }
    }

    public void UpdateItemWheelUI(int item = 0)
    {
        if (InterfaceHandler.instance.currentInterface != Interfaces.item)
        {
            return;
        }

        InventoryTypes currentInventory = PlayerItemController.instance.currentInventory;

        if (currentInventory == InventoryTypes.storage)
        {
            InterfaceHandler.instance.CloseAllInterface();
            return;
        }

        currentWheel = itemWheels[(int)currentInventory - 1];

        if (item != 0 && !currentWheel.itemID.Contains(item))
        {
            return;
        }

        currentWheel.RefreshStack();


        //disable previous selected / second selected
        currentItem = PlayerItemController.instance.rightItem;
        currentItem2 = PlayerItemController.instance.leftItem;

        //put selected
        //put second selected

        int count = 0;
        for (int i = 0; i < 6; i++)
        {
            indexList = currentWheel.SectionItemIndex(i, count);
            count += currentWheel.subsection[i].TotalItemCount();

            if (indexList[0].Count == 0 && indexList[1].Count == 0)
            {
                sectionImages[2 * i].gameObject.SetActive(false);
                sectionImages[2 * i + 1].gameObject.SetActive(false);
            }
            else if (indexList[0].Count == 0)
            {
                sectionImages[2 * i + 1].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 30) * Mathf.Deg2Rad), 0);
                sectionImages[2 * i + 1].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 120);
                sectionImages[2 * i + 1].sprite = twoWideSectionSprite;

                SetSubsection(i, 1);

                sectionImages[2 * i].gameObject.SetActive(false);
                sectionImages[2 * i + 1].gameObject.SetActive(true);
            }
            else if (indexList[1].Count == 0)
            {
                sectionImages[2 * i].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 30) * Mathf.Deg2Rad), 0);
                sectionImages[2 * i].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 120);
                sectionImages[2 * i].sprite = twoWideSectionSprite;

                SetSubsection(i, 0);

                sectionImages[2 * i].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(false);
            }
            else
            {
                sectionImages[2 * i].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 45) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 45) * Mathf.Deg2Rad), 0);
                sectionImages[2 * i + 1].transform.localPosition = (freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 15) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 15) * Mathf.Deg2Rad), 0);
                sectionImages[2 * i].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 135);
                sectionImages[2 * i + 1].transform.localRotation = Quaternion.Euler(0, 0, i * 60 - 105);
                sectionImages[2 * i].sprite = sectionImages[2 * i + 1].sprite = oneWideSectionSprite;

                SetSubsection(i, 0);
                SetSubsection(i, 1);

                sectionImages[2 * i].gameObject.SetActive(true);
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

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(Vector3.zero, subsectionDistance);
    }
}
