using System;
using System.Collections;
using System.Collections.Generic;
using IAP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private TMP_Text _itemName;
    private Button _buyButton;
    private string _bundleId;

    private void Awake()
    {
        _buyButton = gameObject.GetComponentInChildren<Button>();
        _itemName = gameObject.GetComponentInChildren<TMP_Text>();
        _buyButton.onClick.AddListener(BuyThis);
        Init(new ShopItemData("Gems", "mj_gems_5"));
    }

    private void BuyThis()
    {
        IAPManager.Instance.BuyBundle(_bundleId);
    }

    public void Init(ShopItemData itemData)
    {
        _itemName.SetText(itemData.name);
        _bundleId = itemData.id;
    }
}


[Serializable]
public class ShopItemData
{
    public string name;
    public string id;

    public ShopItemData(string gems, string mjGems)
    {
        name = gems;
        id = mjGems;
    }
}
