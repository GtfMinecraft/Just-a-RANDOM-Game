using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Wheel", menuName = "Inventory/Item Wheel")]
public class ItemWheel : ScriptableObject
{
    public List<int> itemID = new List<int>();
    public Sprite[] groupImages = new Sprite[12];
    public Subsection[] subsection = new Subsection[6];

    public int[] stacks {  get; private set; }

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
        if (count == -1)
        {
            count = 0;
            for (int i = 0; i < section; ++i)
            {
                count += subsection[i].TotalItemCount();
            }
        }

        List<int>[] sectionItemIndex = new List<int>[2];
        sectionItemIndex[0] = new List<int>();
        sectionItemIndex[1] = new List<int>();

        for (int i = 0; i < subsection[section].firstSection; ++i)
        {
            if (stacks[count + i] > 0)
            {
                sectionItemIndex[0].Add(count + i);
            }
        }
        count += subsection[section].firstSection;
        for (int i = 0; i < subsection[section].secondSection; ++i)
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

        for (int i = 0; i < itemID.Count; ++i)
        {
            UDictionaryIntInt resources = InventoryHandler.instance.resources;
            if (resources.ContainsKey(itemID[i]))
            {
                stacks[i] = resources[itemID[i]];
            }
            else
            {
                stacks[i] = 0;
            }
        }
    }
}