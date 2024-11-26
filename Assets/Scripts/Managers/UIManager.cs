using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VoxelBusters.AdsKit;

public class UIManager : Singelton<UIManager>
{
    [FormerlySerializedAs("_levelCompleteUI")] public GameObject levelCompleteUI;  
    [FormerlySerializedAs("_levelFailedUI")] public GameObject levelFailedUI;  
    [FormerlySerializedAs("_shopUI")] public GameObject shopUI;  
    [FormerlySerializedAs("_getGemsBtn")] public Button getGemsBtn;  
    [FormerlySerializedAs("_openShopBtn")] public Button openShopBtn;  
    [FormerlySerializedAs("_gemsText")] public TMP_Text gemsText;  

    void Start()
    {
        levelCompleteUI.SetActive(false);
        levelFailedUI.SetActive(false);
        shopUI.SetActive(false);
        
        getGemsBtn.onClick.RemoveAllListeners();
        getGemsBtn.onClick.AddListener(WatchAd);
        
        openShopBtn.onClick.RemoveAllListeners();
        openShopBtn.onClick.AddListener(OpenShop);
    }

    private void OpenShop()
    {
        shopUI.SetActive(true);
    }

    private void OnEnable()
    {
        DTAdEventHandler.OnAdsShowed += OnAdWatched;
        DTAdEventHandler.OnAdsShowFailed += OnWatchAdFailed;
    }

    private void OnDisable()
    {
        DTAdEventHandler.OnAdsShowed -= OnAdWatched;
        DTAdEventHandler.OnAdsShowFailed -= OnWatchAdFailed;
    }

    private void OnWatchAdFailed(string placementid)
    {
        if (placementid.Equals(Constants.RewardedID))
            Debug.Log("Reward not Added!");
    }

    private void OnAdWatched(string placementid)
    {
        if (placementid.Equals(Constants.RewardedID))
        {
            GemsManager.Instance.AddGems(5);
            Debug.Log("Reward Added!");
        }
    }

    private void WatchAd()
    {
        DTAdsManager.Instance.LoadAd(Constants.RewardedID);
        DTAdsManager.Instance.ShowAd(Constants.RewardedID);
    }
    
    public void ShowLevelCompleteUI()
    {
        levelCompleteUI.SetActive(true);  
    }
    
    public void ShowLevelFailedUI()
    {
        DTAdsManager.Instance.ShowAd(Constants.InterstitialId);
        levelFailedUI.SetActive(true);  
    }
    
    public void ResetUI()
    {
        levelCompleteUI.SetActive(false);
        levelFailedUI.SetActive(false);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.OnLevelRestart?.Invoke();
    }

    public void UpdateGems()
    {
        gemsText.SetText(GemsManager.Instance.GetGems().ToString());
    }
}