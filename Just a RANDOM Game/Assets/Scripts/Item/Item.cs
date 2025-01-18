using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("(Don't edit ID, edit in ItemDatabase)")]
    public int ID = 0;
    public string itemName = "New Item";
    public string itemDescription = "New Description";
    public int price = 0;
    public Sprite icon;
    public GameObject model;
    public InventoryTypes inventoryType = InventoryTypes.Storage;
    public ItemTypes itemType = ItemTypes.Basic;
    public int durability;
    public float attackSpeed;
    public int maxStack = 1;
}
