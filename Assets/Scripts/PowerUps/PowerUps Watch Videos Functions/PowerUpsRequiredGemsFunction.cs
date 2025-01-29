using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsRequiredGemsFunction : MonoBehaviour
{
    [SerializeField] private PowerUpEventHandler PowerUpEventHandler;
    [SerializeField] private GameObject PanelObject;
    [SerializeField] Button PowerUpButton;
    private const int Price = 50;
    [Space(10)]
    [SerializeField] private Button WatchAdButton;
     
    private void OnEnable()
    {
        //DTAdEventHandler.OnAdsShowed += OnAdWatched;
        //DTAdEventHandler.OnAdsShowFailed += OnWatchAdFailed;
        WatchAdButton.onClick.AddListener(WatchAd);
    }

    private void OnDisable()
    {
        WatchAdButton.onClick.RemoveAllListeners();
        //DTAdEventHandler.OnAdsShowed -= OnAdWatched;
        //DTAdEventHandler.OnAdsShowFailed -= OnWatchAdFailed;
    }  
    private void OnWatchAdFailed(string placementid)
    {
        if (placementid.Equals(Constants.RewardedID))
        {
            Debug.Log("Reward not Added!");
        }
    }
    private void OnAdWatched(string placementid)
    {
        if (placementid.Equals(Constants.RewardedID))
        {
            if(PowerUpEventHandler.powerUpType==PowerUpType.Fan)
            {
                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Fan, 5);
            }
            
            else if (PowerUpEventHandler.powerUpType == PowerUpType.Rocket)
            {

                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Rocket, 5);
            }
            
            else if (PowerUpEventHandler.powerUpType == PowerUpType.Jump)
            {
                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Jump, 5);
            }
            
            PowerUpButton.interactable = true;
            Debug.Log("Reward Added!");
        }
    }
    public void PowerUpButtonStatus(bool State)
    {
        PanelObject.SetActive(State);
    }
    private void WatchAd()
    {
        //DTAdsManager.Instance.LoadAd(Constants.RewardedID);
        //DTAdsManager.Instance.ShowAd(Constants.RewardedID);

        if (GemsManager.Instance.GetGems() >= Price)
        {
            GemsManager.Instance.UseGems(Price);
            if (PowerUpEventHandler.powerUpType == PowerUpType.Fan)
            {
                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Fan, 1);
            }
            else if (PowerUpEventHandler.powerUpType == PowerUpType.Rocket)
            {

                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Rocket, 1);
            }
            
            else if (PowerUpEventHandler.powerUpType == PowerUpType.Jump)
            {
                PowerUpsManager.Instance.AddPowerUp(PowerUpType.Jump, 1);
            }
            
            PowerUpButton.interactable = true;
        }
    }
}
