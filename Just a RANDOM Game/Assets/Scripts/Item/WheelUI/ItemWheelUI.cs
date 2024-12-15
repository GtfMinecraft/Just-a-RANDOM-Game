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
    public Transform itemWheelTransform;
    public ItemWheel[] itemWheels = new ItemWheel[6];

    [Header("Section Transform")]
    public float freeDistance;
    public float itemWheelDistance;
    public float subsectionDistance;
    public int subsectionDeg;
    public int oneWideSectionSize;
    public int twoWideSectionSize;

    [Header("Section Sprite")]
    public Sprite[] oneWideSectionSprites;
    public Sprite[] twoWideSectionSprites;
    public Sprite[] subsectionSprites;
    public Sprite sectionLockedBackground;
    public Sprite sectionLocked;

    [Header("Selected")]
    public Transform oneWideSelected;
    public Transform oneWideSelected2;
    public Transform twoWideSelected;
    public Transform twoWideSelected2;
    public Transform subsectionSelected;
    public Transform subsectionSelected2;

    private bool[] selectedBool = new bool[6];

    [Header("Element Stone")]
    public Image[] elementStones;

    private Animator anim;

    private ItemWheel currentWheel;
    private List<int>[] indexList;

    private int currentItem = 0;//default item if none is selected
    private int currentItem2 = 0;

    private int hoveredSection = -1;
    private int hoveredSubsection = -1;
    private int currentSection = -1;
    private int currentSection2 = -1;

    private ItemDatabase database;

    private Image[] sectionImages = new Image[12];
    private Image[][] subsectionImages = new Image[12][];

    private void Start()
    {
        database = PlayerItemController.instance.database;
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
            SetHoveredSection();
        }
        else if (mouseDistance >= itemWheelDistance && hoveredSection != -1)
        {
            int subsection = GetSubsectionItemIndex(true);
            if (hoveredSubsection != subsection)
            {
                hoveredSubsection = subsection;
                sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().subsectionHovered = hoveredSubsection;
            }
            if (hoveredSubsection == -1)
            {
                SetHoveredSection();
            }
        }
        else if (mouseDistance >= freeDistance)
        {
            SetHoveredSection();
        }
    }

    private void SetHoveredSection()
    {
        int section = ItemGetSection();

        if (hoveredSection != section)
        {
            if (hoveredSection != -1)
            {
                if (currentSection == hoveredSection)
                {
                    subsectionSelected.GetComponent<Image>().enabled = false;
                }
                if (currentSection2 == hoveredSection)
                {
                    subsectionSelected2.GetComponent<Image>().enabled = false;
                }
                sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().hovered = false;
            }
            hoveredSection = section;
            if(hoveredSection != -1)
            {
                sectionImages[hoveredSection].GetComponent<ItemWheelUIHover>().hovered = true;
                if (currentSection == hoveredSection && selectedBool[4])
                {
                    subsectionSelected.GetComponent<Image>().enabled = true;
                }
                if (currentSection2 == hoveredSection && selectedBool[5])
                {
                    subsectionSelected2.GetComponent<Image>().enabled = true;
                }
            }
        }
    }

    protected int ItemGetSection()
    {
        int section = GetSection(-60, 30, 12, freeDistance);

        if(section == -1)
        {
            return -1;
        }

        bool active = sectionImages[section].gameObject.activeSelf;

        return active ? section : section - 2 * (section & 1) + 1;
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
        PlayerItemController.instance.SwapHandItem(itemID);
    }

    protected int SetSubsection(int section, int subsection)
    {
        int hasSelectedItem = 0;

        int count = indexList[subsection].Count;

        if (count == 1)
        {
            count = 0;

            sectionImages[2 * section + subsection].transform.GetChild(0).GetComponent<Image>().sprite = database.GetItem[currentWheel.itemID[indexList[subsection][0]]].icon;

            if (currentWheel.stacks[indexList[subsection][0]] == 1)
            {
                sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
            else
            {
                sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = currentWheel.stacks[indexList[subsection][0]].ToString();
            }

            if(currentWheel.itemID[indexList[subsection][0]] == currentItem)
            {
                currentSection = 2 * section + subsection;

                hasSelectedItem = 1;
            }
            else if (currentWheel.itemID[indexList[subsection][0]] == currentItem2)
            {
                currentSection2 = 2 * section + subsection;

                hasSelectedItem = 2;
            }
        }
        else
        {
            sectionImages[2 * section + subsection].transform.GetChild(0).GetComponent<Image>().sprite = currentWheel.groupImages[2 * section + subsection];
            sectionImages[2 * section + subsection].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

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
            subsectionImages[2 * section + subsection][i].sprite = subsectionSprites[section];

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

            if (currentWheel.itemID[indexList[subsection][i]] == currentItem)
            {
                subsectionSelected.localPosition = subsectionImages[2 * section + subsection][i].transform.localPosition;
                subsectionSelected.localRotation = subsectionImages[2 * section + subsection][i].transform.localRotation;

                currentSection = 2 * section + subsection;
                selectedBool[4] = true;

                hasSelectedItem = 1;
            }
            else if (currentWheel.itemID[indexList[subsection][i]] == currentItem2)
            {
                //set second item

                subsectionSelected2.localPosition = subsectionImages[2 * section + subsection][i].transform.localPosition;
                subsectionSelected2.localRotation = subsectionImages[2 * section + subsection][i].transform.localRotation;

                currentSection2 = 2 * section + subsection;
                selectedBool[5] = true;

                hasSelectedItem = 2;
            }
        }

        return hasSelectedItem;
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

        itemWheelTransform.gameObject.SetActive(true);
        
        currentWheel = itemWheels[(int)currentInventory - 1];

        if (item != 0 && !currentWheel.itemID.Contains(item))
        {
            return;
        }

        currentWheel.RefreshStack();

        oneWideSelected.GetComponent<Image>().enabled = false;
        oneWideSelected2.GetComponent<Image>().enabled = false;
        twoWideSelected.GetComponent<Image>().enabled = false;
        twoWideSelected2.GetComponent<Image>().enabled = false;
        subsectionSelected.GetComponent<Image>().enabled = false;
        subsectionSelected2.GetComponent<Image>().enabled = false;

        for (int i = 0; i < elementStones.Length; ++i)
        {
            elementStones[i].enabled = false;
        }

        for (int i = 0; i < selectedBool.Length; ++i)
        {
            selectedBool[i] = false;
        }

        currentItem = PlayerItemController.instance.rightItems[(int)currentInventory];
        currentItem2 = PlayerItemController.instance.leftItems[(int)currentInventory];

        int count = 0;
        for (int i = 0; i < 6; i++)
        {
            indexList = currentWheel.SectionItemIndex(i, count);
            count += currentWheel.subsection[i].TotalItemCount();

            sectionImages[2 * i].transform.GetChild(0).rotation = Quaternion.identity;
            sectionImages[2 * i + 1].transform.GetChild(0).rotation = Quaternion.identity;

            if (indexList[0].Count == 0 && indexList[1].Count == 0)
            {
                sectionImages[2 * i].transform.SetLocalPositionAndRotation((freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 30) * Mathf.Deg2Rad), 0), Quaternion.Euler(0, 0, i * 60 - 120));
                sectionImages[2 * i].sprite = sectionLockedBackground;
                sectionImages[2 * i].rectTransform.sizeDelta = new Vector2(twoWideSectionSize, twoWideSectionSize);

                sectionImages[2 * i].transform.GetChild(0).GetComponent<Image>().sprite = sectionLocked;
                sectionImages[2 * i].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

                sectionImages[2 * i].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(false);
            }
            else if (indexList[0].Count == 0)
            {
                sectionImages[2 * i + 1].transform.SetLocalPositionAndRotation((freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 30) * Mathf.Deg2Rad), 0), Quaternion.Euler(0, 0, i * 60 - 120));
                sectionImages[2 * i + 1].sprite = twoWideSectionSprites[i];
                sectionImages[2 * i + 1].rectTransform.sizeDelta = new Vector2(twoWideSectionSize, twoWideSectionSize);

                int setSelected = SetSubsection(i, 1);

                if (setSelected == 1)
                {
                    twoWideSelected.SetLocalPositionAndRotation(sectionImages[2 * i + 1].transform.localPosition, sectionImages[2 * i + 1].transform.localRotation);
                    twoWideSelected.GetComponent<Image>().enabled = true;

                    selectedBool[2] = true;
                }
                else if(setSelected == 2)
                {
                    twoWideSelected2.SetLocalPositionAndRotation(sectionImages[2 * i + 1].transform.localPosition, sectionImages[2 * i + 1].transform.localRotation);
                    twoWideSelected2.GetComponent<Image>().enabled = true;

                    selectedBool[3] = true;
                }

                sectionImages[2 * i].gameObject.SetActive(false);
                sectionImages[2 * i + 1].gameObject.SetActive(true);

                if (!elementStones[i].enabled)
                {
                    elementStones[i].enabled = true;
                }
            }
            else if (indexList[1].Count == 0)
            {
                sectionImages[2 * i].transform.SetLocalPositionAndRotation((freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 30) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 30) * Mathf.Deg2Rad), 0), Quaternion.Euler(0, 0, i * 60 - 120));
                sectionImages[2 * i].sprite = twoWideSectionSprites[i];
                sectionImages[2 * i].rectTransform.sizeDelta = new Vector2(twoWideSectionSize, twoWideSectionSize);

                int setSelected = SetSubsection(i, 0);

                if(setSelected == 1)
                {
                    twoWideSelected.SetLocalPositionAndRotation(sectionImages[2 * i].transform.localPosition, sectionImages[2 * i].transform.localRotation);
                    twoWideSelected.GetComponent<Image>().enabled = true;

                    selectedBool[2] = true;
                }
                else if(setSelected == 2)
                {
                    twoWideSelected2.SetLocalPositionAndRotation(sectionImages[2 * i].transform.localPosition, sectionImages[2 * i].transform.localRotation);
                    twoWideSelected2.GetComponent<Image>().enabled = true;

                    selectedBool[3] = true;
                }

                sectionImages[2 * i].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(false);

                if (!elementStones[i].enabled)
                {
                    elementStones[i].enabled = true;
                }
            }
            else
            {
                sectionImages[2 * i].transform.SetLocalPositionAndRotation((freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 45) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 45) * Mathf.Deg2Rad), 0), Quaternion.Euler(0, 0, i * 60 - 135));
                sectionImages[2 * i + 1].transform.SetLocalPositionAndRotation((freeDistance + itemWheelDistance) / 2 * new Vector3(Mathf.Cos((60 * i - 15) * Mathf.Deg2Rad), Mathf.Sin((60 * i - 15) * Mathf.Deg2Rad), 0), Quaternion.Euler(0, 0, i * 60 - 105));
                sectionImages[2 * i].sprite = sectionImages[2 * i + 1].sprite = oneWideSectionSprites[i];
                sectionImages[2 * i].rectTransform.sizeDelta = sectionImages[2 * i + 1].rectTransform.sizeDelta = new Vector2(oneWideSectionSize, oneWideSectionSize);

                int setSelected = SetSubsection(i, 0);

                if (setSelected == 1)
                {
                    oneWideSelected.SetLocalPositionAndRotation(sectionImages[2 * i].transform.localPosition, sectionImages[2 * i].transform.localRotation);
                    oneWideSelected.GetComponent<Image>().enabled = true;

                    selectedBool[1] = true;
                }
                else if(setSelected == 2)
                {
                    oneWideSelected2.SetLocalPositionAndRotation(sectionImages[2 * i].transform.localPosition, sectionImages[2 * i].transform.localRotation);
                    oneWideSelected2.GetComponent<Image>().enabled = true;

                    selectedBool[2] = true;
                }

                setSelected = SetSubsection(i, 1);
                
                if (setSelected == 1)
                {
                    oneWideSelected.SetLocalPositionAndRotation(sectionImages[2 * i + 1].transform.localPosition, sectionImages[2 * i + 1].transform.localRotation);
                    oneWideSelected.GetComponent<Image>().enabled = true;
                }
                else if (setSelected == 2)
                {
                    oneWideSelected2.SetLocalPositionAndRotation(sectionImages[2 * i + 1].transform.localPosition, sectionImages[2 * i + 1].transform.localRotation);
                    oneWideSelected2.GetComponent<Image>().enabled = true;
                }

                sectionImages[2 * i].gameObject.SetActive(true);
                sectionImages[2 * i + 1].gameObject.SetActive(true);

                if (!elementStones[i].enabled)
                {
                    elementStones[i].enabled = true;
                }
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
