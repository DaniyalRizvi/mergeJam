using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PowerUpEventHandler : MonoBehaviour
{
    public PowerUpType powerUpType;
    
    private IPowerUp _powerUp;
    
    private bool canUse => PowerUpsManager.Instance.CanUsePowerUp(powerUpType);
    private bool _isOnCooldown = false;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PowerUpsManager.Instance.isInitialized);
        GetComponent<Button>().onClick.AddListener(UsePowerUp);
        switch (powerUpType)
        {
            case PowerUpType.Rocket:
                _powerUp = new Rocket();
                break;
            case PowerUpType.Fan:
                _powerUp = new Fan();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        SetText();
    }

    public void SetText()
    {
        var text = GetComponentInChildren<TMP_Text>();
        var amount = PowerUpsManager.Instance.GetPowerUpAmount(powerUpType);
        text.SetText($"{amount}");
    }

    private void UsePowerUp()
    {
        if (_isOnCooldown)
            return;
        _isOnCooldown = true;
        StartCoroutine(Cooldown());
        if (canUse && powerUpType == PowerUpType.Rocket)
        {
            var data = CreateData() as RocketData;
            var canExecute = _powerUp.ExecuteWithReturn(data);
            if (canExecute)
            {
                var level = data?.Level;
                var colors = data?.Colors;
                if (level != null)
                {
                    var busses = level.gameObject.GetComponentsInChildren<Bus>().ToList();
                    busses.Shuffle();
                    foreach (var bus in busses.Where(bus => colors.Contains(bus.busColor)))
                    {
                        level.DestroyBus(bus);
                        break;
                    }
                }

                PowerUpsManager.Instance.UsePowerUp(powerUpType);            
            }
            SetText();
            return;
        }
        if (canUse)
        {
            _powerUp.Execute(CreateData());
            PowerUpsManager.Instance.UsePowerUp(powerUpType);
            SetText();
            return;
        }

        var requiredGems = GetRequiredGems();
        if (GemsManager.Instance.GetGems() >= requiredGems)
        {
            GemsManager.Instance.UseGems(requiredGems);
            _powerUp.Execute(CreateData());
            return;
        }
        else
        {
            UIManager.Instance.OpenShop();
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(10f);
        _isOnCooldown = false;
    }

    private int GetRequiredGems()
    {
        return powerUpType switch
        {
            PowerUpType.Rocket => Constants.RocketRequirement,
            PowerUpType.Fan => Constants.FanRequirement,
            _ => int.MaxValue
        };
    }

    private object CreateData()
    {
        return powerUpType switch
        {
            PowerUpType.Rocket => new RocketData(LevelManager.Instance.GetCurrentLevel(), LevelManager.Instance.GetCurrentLevelColors()),
            PowerUpType.Fan => new FanData(LevelManager.Instance.GetCurrentLevel(), Constants.FanForce),
            _ => null
        };
    }
}
