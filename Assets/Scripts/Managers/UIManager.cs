using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VoxelBusters.AdsKit;

public class UIManager : Singelton<UIManager>
{
    public Button SettingButton;
    public SettingPanel SettingPanel;
    [Space(10)]
    public GameObject iapOverlay;
    public GameObject pahHolder;
    [FormerlySerializedAs("_levelCompleteUI")] public GameObject levelCompleteUI;
    [FormerlySerializedAs("_levelFailedUI")] public GameObject levelFailedUI;
    [FormerlySerializedAs("_shopUI")] public GameObject shopUI;
    [FormerlySerializedAs("_openShopBtn")] public Button openShopBtn;
    public Button watchAdBtn;
    [FormerlySerializedAs("_gemsText")] public TMP_Text gemsText;
    public List<ShopItem> iapHolders;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => GemsManager.Instance.IsInitialized);
        iapOverlay.SetActive(false);
        levelCompleteUI.SetActive(false);
        levelFailedUI.SetActive(false);
        shopUI.SetActive(false);
        openShopBtn.onClick.RemoveAllListeners();
        openShopBtn.onClick.AddListener(OpenShop);
        watchAdBtn.onClick.RemoveAllListeners();
        watchAdBtn.onClick.AddListener(WatchAd);

        SettingButton?.onClick.RemoveAllListeners();
        SettingButton?.onClick.AddListener(() =>
        {
            SettingPanel.SettingPanelState(true);
        });
        UpdateGems();
    }

    public void OpenShop()
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
        {
            Debug.Log("Reward not Added!");
            OpenShop();
        }
    }

    private void OnAdWatched(string placementid)
    {
        if (placementid.Equals(Constants.RewardedID))
        {
            GemsManager.Instance.UseGems(10);
            GemsManager.Instance.AddGems(200);
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
        watchAdBtn.interactable = true;
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
        SetupHolders(LevelManager.Instance.GetCurrentLevel());
    }

    private void SetupHolders(Level currentLevel)
    {
        foreach (var pah in pahHolder.GetComponentsInChildren<PassengerAmountHolder>())
        {
            Destroy(pah.gameObject);
        }
        foreach (var color in currentLevel.colors)
        {
            PassengerAmountHolder pah = Instantiate(Resources.Load<GameObject>("Passenger Amount Holder")).GetComponent<PassengerAmountHolder>();
            pah.Init(color.color, color.count);
            pah.transform.SetParent(pahHolder.transform);
            pah.transform.localScale= Vector3.one;
        }
    }

    public void UpdateHolder(Colors color)
    {
        pahHolder.GetComponentsInChildren<PassengerAmountHolder>().FirstOrDefault(i => i.Color == color)?.UpdateAmount();
    }

    public void RestartLevel()
    {
        LevelManager.Instance.OnLevelRestart?.Invoke();
    }

    public void UpdateGems()
    {
        gemsText.SetText(GemsManager.Instance.GetGems().ToString());
    }

    public void EnableIAPOverlay(bool enable)
    {
        iapOverlay.SetActive(enable);
    }
}