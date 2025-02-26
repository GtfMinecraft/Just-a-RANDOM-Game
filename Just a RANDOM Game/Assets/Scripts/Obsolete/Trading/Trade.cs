using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trade", menuName = "Trade/Trade")]
public class Trade : ScriptableObject
{
    public Offer[] offers;

    [System.Serializable]
    public class Offer
    {
        public int item;
        [UDictionary.Split(30, 70)]
        public UDictionaryIntInt materials;
        public int quantity;// -1 = infinite
        public bool show = false;

        public bool Buy(UDictionaryIntInt playerMaterials)
        {
            if (quantity == 0) return false;
            if (!show) return false;

            foreach (var mat in materials)
            {
                if (playerMaterials[mat.Key] < mat.Value)
                {
                    return false;
                }
            }

            if (quantity > 1)
            {
                quantity--;
            }
            else if (quantity == 1)
            {
                quantity = 0;
                show = false;
            }

            return true;
        }
    }

    public void EnableOffer(int item)
    {
        for (int i = 0; i < offers.Length; ++i)
        {
            if (offers[i].item == item)
            {
                offers[i].show = true;
                TradingInterface.instance.RefreshTradingInterface();
                return;
            }
        }
    }

    //public void SetPrice(int item, int setPrice)
    //{
    //    for (int i = 0; i < offers.Length; ++i)
    //    {
    //        if (offers[i].merchandice == item)
    //        {
    //            offers[i].price = setPrice;
    //            TradingInterface.instance.RefreshTradingInterface();
    //            return;
    //        }
    //    }
    //}

    public bool Buy(int index)
    {
        bool tryBuy = offers[index].Buy(InventoryHandler.instance.resources);
        TradingInterface.instance.RefreshTradingInterface();
        return tryBuy;
    }
}
