using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public string itemDescription = "New Description";
    public int price = 0;
    public Sprite icon;
    public InventoryTypes inventoryType = InventoryTypes.storage;
    public ItemTypes itemType = ItemTypes.Basic;
    public int durability;
    public float attackSpeed;
    public int currentStack = 1;
    public int maxStack = 1;
}
