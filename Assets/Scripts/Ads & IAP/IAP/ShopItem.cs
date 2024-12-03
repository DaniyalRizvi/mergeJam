using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IAP;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private TMP_Text _itemName;
    private Button _buyButton;
    public string bundleId;

    private void Awake()
    {
        bundleId = name;
        _itemName = gameObject.GetComponentsInChildren<TMP_Text>().ToList()
            .FirstOrDefault(i => i.name.Equals("Price Text"));
        _buyButton = gameObject.GetComponentInChildren<Button>();
    }

    private void OnEnable()
    {
        _buyButton.onClick.AddListener(BuyThis);
        UpdatePrices();
    }

    private void OnDisable()
    {
        _buyButton.onClick.RemoveAllListeners();
    }

    private void BuyThis()
    {
        IAPManager.Instance.BuyBundle(bundleId);
        UIManager.Instance.EnableIAPOverlay(true);
    }

    private void UpdatePrices()
    {
        var price = IAPManager.Instance.GetProductPrice(bundleId);
        _itemName.text = $"$ {price}";
    }
}