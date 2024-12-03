using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpsManager : Singelton<PowerUpsManager>
{
    public bool isInitialized = false;
    private Dictionary<PowerUpType, int> _powerUps = new();

    private void Start()
    {
        var powerUpEnum = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToList();
        foreach (var var in powerUpEnum)
            _powerUps.Add(var, 0);
        LoadPowerUps();
    }

    private void LoadPowerUps()
    {
        var powerUpEnum = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToList();
        foreach (var var in powerUpEnum)
        {
            if (PlayerPrefs.HasKey(var.ToString()))
            {

                var amount = PlayerPrefs.GetInt(var.ToString());
                _powerUps[var] += amount;
            }
        }

        isInitialized = true;
    }

    private void SavePowerUps()
    {
        foreach (var kvp in _powerUps)
        {
            PlayerPrefs.SetInt(kvp.Key.ToString(), kvp.Value);
        }
    }


    public bool CanUsePowerUp(PowerUpType powerUpType)
    {
        var canUse = _powerUps[powerUpType] > 0;
        return canUse;
    }

    public void UsePowerUp(PowerUpType powerUpType)
    {
        if (_powerUps[powerUpType] > 0)
        {
            _powerUps[powerUpType]--;
            SavePowerUps();
        }
    }

    public void AddPowerUp(PowerUpType powerUpType, int amount)
    {
        _powerUps[powerUpType]+= amount;
        FindObjectsOfType<PowerUpEventHandler>().ToList().FirstOrDefault(i => i.powerUpType == powerUpType)?.SetText();
    }
    
    public int GetPowerUpAmount(PowerUpType powerUpType) => _powerUps[powerUpType];

    private void OnApplicationFocus(bool hasFocus)
    {
        SavePowerUps();
    }

    private void OnDestroy()
    {
        SavePowerUps();
    }
}


public enum PowerUpType
{
    Rocket,
    Fan
}