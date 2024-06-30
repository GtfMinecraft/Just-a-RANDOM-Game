using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public List<InventoryData> inventoryData;
    public bool inventoryFirstInstance;

    //TODO: put all data that need to save in this class

    public GameData()
    {
        inventoryData = new List<InventoryData> 
        {
            new(InventoryTypes.storage), 
            new(InventoryTypes.weapon), 
            new(InventoryTypes.axe),
            new(InventoryTypes.pickaxe),
            new(InventoryTypes.hoe),
            new(InventoryTypes.rod),
            new(InventoryTypes.food),
        };
        inventoryFirstInstance = true;
    }

    public class InventoryData
    {
        public List<int> itemIDs;
        //TODO: fill in the defaultItemIDs
        public List<int> currentStacks;

        public InventoryData() { }

        public InventoryData(InventoryTypes inv)
        {
            //TODO: define Inventory
            switch(inv)
            {
                case InventoryTypes.storage:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.weapon:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.axe:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.pickaxe:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.hoe:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.rod:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case InventoryTypes.food:
                    itemIDs = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    currentStacks = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                default:
                    Debug.LogError("Undefined InventoryData when creating an instace of GameData.InventoryData");
                    itemIDs = new List<int>();
                    currentStacks = new List<int>();
                    break;
            }
        }
    }
}
