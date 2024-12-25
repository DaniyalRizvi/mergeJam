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

        if (TutorialManager.Instance)
        {
            GetComponent<Button>().onClick.AddListener(UsePowerUpTutorial);
            yield break;
        }
        GetComponent<Button>().onClick.AddListener(UsePowerUp);
        SetText();
 
    }

    private void UsePowerUpTutorial()
    {
        _powerUp.Execute(CreateData());
        if (TutorialManager.Instance && powerUpType == PowerUpType.Fan)
        {
            GameManager.Instance.FanPowerUps();
            TutorialManager.Instance.tutorialCase++;
            //TutorialManager.Instance.InitRocket();
            TutorialManager.Instance.InitRocketPanel();
        }
        if (TutorialManager.Instance && powerUpType == PowerUpType.Rocket)
        {
            
            TutorialManager.Instance.tutorialCase++;
            TutorialManager.Instance.TutorialCompleted();
        }

        GetComponent<Button>().interactable = false;
    }

    public void SetText()
    {
        if (!PowerUpsManager.Instance.CanUsePowerUp(powerUpType))
        {
            Debug.LogError(powerUpType.ToString() + " Not Have This PowerUp");
            GetComponent<Button>().interactable = false;

            GetComponent<PowerUpsRequiredGemsFunction>().PowerUpButtonStatus(true);
        }
        else
        {
            GetComponent<Button>().interactable = true;
            GetComponent<PowerUpsRequiredGemsFunction>().PowerUpButtonStatus(false);
        }

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
                        GameManager.Instance.RocketPowerUps(bus.transform);
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
            GameManager.Instance.FanPowerUps();
            _powerUp.Execute(CreateData());
            PowerUpsManager.Instance.UsePowerUp(powerUpType);
            SetText();
            return;
        }
        //Remove Due to Other Requirement 
        //var requiredGems = GetRequiredGems();
        //if (GemsManager.Instance.GetGems() >= requiredGems)
        //{
        //    GemsManager.Instance.UseGems(requiredGems);
        //    _powerUp.Execute(CreateData());
        //}
        //else
        //{
        //    UIManager.Instance.OpenShop();
        //}
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
