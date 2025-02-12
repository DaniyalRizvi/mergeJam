using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsManager : Singelton<GemsManager>
{
    private int _gemAmount;
    internal bool IsInitialized;

    protected override void Awake()
    {
        base.Awake();
        _gemAmount = PlayerPrefs.HasKey("Gems") ? PlayerPrefs.GetInt("Gems") : 0;
        IsInitialized = true;
    }

    public void AddGems(int amount)
    {
        Debug.Log("Gems: "+_gemAmount);
        Debug.Log("Add Gems: " + amount);
        if (amount > 0)
        {
            _gemAmount += amount;
            SaveGems();
        }
        else
        {
            Debug.LogWarning("AddGems called with non-positive amount.");
        }
        Debug.Log("Gems: "+_gemAmount);
    }

    public bool UseGems(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("UseGems called with non-positive amount.");
            return false;
        }

        if (_gemAmount >= amount)
        {
            _gemAmount -= amount;
            SaveGems();
            return true;
        }

        Debug.LogWarning("Not enough gems to complete the transaction.");
        return false;
    }

    private void SaveGems()
    {
        UIManager.Instance.UpdateGems();
        PlayerPrefs.SetInt("Gems", _gemAmount);
        PlayerPrefs.Save();
    }

    public int GetGems()
    {
        return _gemAmount;
    }
}
