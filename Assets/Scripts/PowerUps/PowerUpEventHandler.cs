using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PowerUpEventHandler : MonoBehaviour
{

    public PowerUpType powerUpType;
    
    private IPowerUp _powerUp;

    public TMP_Text text;
    private bool canUse => PowerUpsManager.Instance.CanUsePowerUp(powerUpType);
    private bool _isOnCooldown = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PowerUpsManager.Instance.isInitialized);
        switch (powerUpType)
        {
            case PowerUpType.Jump:
            {
                _powerUp = new Jump();
                break;
            }
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
            //TutorialManager.Instance.InitRocketPanel();
        }

        if (TutorialManager.Instance && powerUpType == PowerUpType.Rocket)
        {
            TutorialManager.Instance.tutorialCase++;
            //GameManager.Instance.FanPowerUps();
            //TutorialManager.Instance.InitJumpPanel();
        }

        if (TutorialManager.Instance && powerUpType == PowerUpType.Jump)
        {
            TutorialManager.Instance.tutorialCase++;
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

        //var text = GetComponentInChildren<TMP_Text>();
        var amount = PowerUpsManager.Instance.GetPowerUpAmount(powerUpType);
        text.SetText($"{amount}");
    }

    private void UsePowerUp()
    {
        if (_isOnCooldown)
            return;
        _isOnCooldown = true;
        StartCoroutine(Cooldown());
        if (canUse)
        {
            if (powerUpType == PowerUpType.Rocket)
            {
                StartCoroutine(WaitForPlayerClick());
                return;
            }
            
            else if (powerUpType == PowerUpType.Fan)
            {
                GetComponent<Button>().interactable = false;
                GameManager.Instance.FanPowerUps();
                _powerUp.Execute(CreateData());
                PowerUpsManager.Instance.UsePowerUp(powerUpType);
                SetText();
                GetComponent<Button>().interactable = true;
                return;
            }
            
            else if (powerUpType == PowerUpType.Jump)
            {
                GetComponent<Button>().interactable = false;
                GameManager.Instance.JumpPowerUps();
                var JumpData = CreateData() as JumpData;
                _powerUp.Execute(JumpData);
                PowerUpsManager.Instance.UsePowerUp(powerUpType);
                SetText();
                GetComponent<Button>().interactable = true;
                return;
            }
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
    
    private IEnumerator WaitForPlayerClick()
    {
        bool carSelected = false;
        Bus selectedBus = null;

        while (!carSelected)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    selectedBus = hit.collider.GetComponent<Bus>();
                    if (selectedBus != null)
                    {
                        carSelected = true;
                        GameManager.Instance.rocketPowerUp = true;
                    }
                }
            }
            yield return null;
        }

        // Instantiate rocket mesh
        Vector3 rocketStartPosition = selectedBus.transform.position + new Vector3(10, 10, 0); // Adjust as needed
        GameObject rocket = Instantiate(Resources.Load<GameObject>("RocketMesh"), rocketStartPosition, Quaternion.identity);
        rocket.transform.rotation = Quaternion.LookRotation(selectedBus.transform.position - rocketStartPosition);
    
        // Simulate rocket movement
        float duration = 0.5f;
        float elapsedTime = 0f;
        var data = CreateData() as RocketData;
        var level = data?.Level;
        Vector3 startPosition = rocket.transform.position;
        Vector3 peakPosition = startPosition + new Vector3(0, 3, 3);
        while (elapsedTime < duration / 2)
        {
            rocket.transform.position = Vector3.Lerp(startPosition, peakPosition, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            rocket.transform.position = Vector3.Lerp(peakPosition, selectedBus.transform.position, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        Destroy(rocket);
        GameManager.Instance.RocketPowerUps(selectedBus.transform);
        level.DestroyBus(selectedBus);
        PowerUpsManager.Instance.UsePowerUp(powerUpType);
        GameManager.Instance.rocketPowerUp = false;
        SetText();
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
            PowerUpType.Jump => new JumpData(LevelManager.Instance.GetCurrentLevel(), Constants.JumpForce),
            _ => null
        };
    }
}
