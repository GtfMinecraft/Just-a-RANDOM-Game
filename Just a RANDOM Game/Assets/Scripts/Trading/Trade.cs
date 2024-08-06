using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Trade", menuName = "Trade/New Trade")]
public class Trade : ScriptableObject
{
    public Offer[] offers;

    [System.Serializable]
    public class Offer
    {
        public Item merchandice;
        public int price;
        public Item[] materials;
        public bool show = false;
        
        public Offer(Item setItem, int setPrice, Item[] setMaterials)
        {
            merchandice = setItem;
            price = setPrice;
            materials = setMaterials;
        }
    }

    public void AddOffer(Item item)
    {
        for (int i = 0; i < offers.Length; ++i)
        {
            if (offers[i].merchandice == item)
            {
                offers[i].show = true;
                TradingInterface.instance.RefreshTradingInterface();
                return;
            }
        }
    }

    public void SetPrice(Item item, int setPrice)
    {
        for (int i = 0; i < offers.Length; ++i)
        {
            if (offers[i].merchandice == item)
            {
                offers[i].price = setPrice;
                TradingInterface.instance.RefreshTradingInterface();
                return;
            }
        }
    }
}
