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
    [Space(10)]
    public Button SettingButton;
    public SettingPanel SettingPanel;
    [Space(10)]
    public GameObject iapOverlay;
    public GameObject pahHolder;
    [FormerlySerializedAs("_levelCompleteUI")] public GameObject levelCompleteUI;
    [FormerlySerializedAs("_levelFailedUI")] public GameObject levelFailedUI;
    [FormerlySerializedAs("_shopUI")] public GameObject shopUI;
    [FormerlySerializedAs("_openShopBtn")] public Button openShopBtn;
    public GameObject hardLevelUI;
    public Button watchAdBtn;
    [FormerlySerializedAs("_gemsText")] public TMP_Text gemsText;
    [FormerlySerializedAs("_gemsText")] public TMP_Text levelgemsText;
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
            GemsManager.Instance.AddGems(20);
            levelgemsText.text="20";
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
        GemsManager.Instance.AddGems(10);
        levelgemsText.text="10";
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
        SetupHolders(LevelManager.Instance._levels[PlayerPrefs.GetInt("CurrentLevel")]);
    }

    private void SetupHolders(Level currentLevel)
    {
        var children = pahHolder.GetComponentsInChildren<PassengerAmountHolder>().ToList();
        foreach (var pah in children)
        {
            pah.gameObject.SetActive(true);
            pah.SetAmount(0);
            //Destroy(pah.gameObject);
        }
        //Debug.Log(children.Count);
        foreach (var color in currentLevel.colors)
        {
            bool colorMatched = false;

            children = pahHolder.GetComponentsInChildren<PassengerAmountHolder>().ToList();
            foreach (var child in children)
            {
                PassengerAmountHolder existingPah = child.GetComponent<PassengerAmountHolder>();
                //Debug.Log(existingPah);
                if (existingPah != null && existingPah.Color == color.color)
                {
                    existingPah.AddAmount(color.count); // Assuming AddAmount is a method in PassengerAmountHolder to add to the count
                    colorMatched = true;
                    break;
                }
            }

            if (!colorMatched)
            {
                //Debug.Log("Hereeee");
                PassengerAmountHolder pah = Instantiate(Resources.Load<GameObject>("Passenger Amount Holder")).GetComponent<PassengerAmountHolder>();
                pah.Init(color.color, color.count);
                pah.transform.SetParent(pahHolder.transform);
                pah.transform.localScale = Vector3.one;
            }
        }
        
        children = pahHolder.GetComponentsInChildren<PassengerAmountHolder>().ToList();

        foreach (var child in children)
        {
            if(child.GetAmount()==0)
                Destroy(child.gameObject);
        }

        // foreach (var color in currentLevel.colors)
        // {
        //     PassengerAmountHolder pah = Instantiate(Resources.Load<GameObject>("Passenger Amount Holder")).GetComponent<PassengerAmountHolder>();
        //     pah.Init(color.color, color.count);
        //     pah.transform.SetParent(pahHolder.transform);
        //     pah.transform.localScale= Vector3.one;
        // }
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