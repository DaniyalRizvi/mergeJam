using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.AdsKit;

public class UIManager : Singelton<UIManager>
{
    private GameObject _levelCompleteUI;  
    private GameObject _levelFailedUI;  
    private GameObject _shopUI;  
    private Button _bannerAdBtn;  
    private Button _getGemsBtn;  
    private Button _openShopBtn;  
    private TMP_Text _gemsText;  

    void Start()
    {
        Init();
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
        _shopUI.SetActive(false);
        
        _getGemsBtn.onClick.RemoveAllListeners();
        _getGemsBtn.onClick.AddListener(WatchAd);
        
        _bannerAdBtn.onClick.RemoveAllListeners();
        _bannerAdBtn.onClick.AddListener(BannerAd);
        
        _openShopBtn.onClick.RemoveAllListeners();
        _openShopBtn.onClick.AddListener(OpenShop);
    }

    private void OpenShop()
    {
        _shopUI.SetActive(true);
    }

    private bool isShowingAd;

    private void BannerAd()
    {
        if (isShowingAd)
        {
            DTAdsManager.Instance.HideBannerAd(Constants.BannerID, true);
            _bannerAdBtn.GetComponentInChildren<TMP_Text>().SetText("Show Banner Ad");
        }
        else
        {
            DTAdsManager.Instance.ShowAd(Constants.BannerID);
            _bannerAdBtn.GetComponentInChildren<TMP_Text>().SetText("Hide Banner Ad");
        }

        isShowingAd = !isShowingAd;
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

    private void Init()
    {
        _levelCompleteUI = GameObject.Find("LevelCompleted");
        _levelFailedUI = GameObject.Find("LevelFailed");
        _shopUI = GameObject.Find("Shop");
        _getGemsBtn = GameObject.Find("GetGems").GetComponent<Button>();
        _bannerAdBtn = GameObject.Find("ShowBannerAd").GetComponent<Button>();
        _openShopBtn = GameObject.Find("OpenShop").GetComponent<Button>();
        _gemsText = GameObject.Find("GemsText").GetComponent<TMP_Text>();
    }
    
    public void ShowLevelCompleteUI()
    {
        _levelCompleteUI.SetActive(true);  
    }
    
    public void ShowLevelFailedUI()
    {
        _levelFailedUI.SetActive(true);  
    }
    
    public void ResetUI()
    {
        _levelCompleteUI.SetActive(false);
        _levelFailedUI.SetActive(false);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.OnLevelRestart?.Invoke();
    }

    public void UpdateGems()
    {
        _gemsText.SetText(GemsManager.Instance.GetGems().ToString());
    }
}