using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using VoxelBusters.AdsKit.Adapters;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    [AdNetwork(AdNetworkServiceId.kUnityAds)]
    public class UnityAdsAdapter : AdNetworkAdapter, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        #region Properties

        private User GameUser { get; set; }

        #endregion

        #region Base class methods

        public override bool IsInitialised => Advertisement.isInitialized;

        public override bool IsSupported => Advertisement.isSupported;

        public override void Initialise(AdNetworkInitialiseProperties properties)
        {
            SetPrivacyConfiguration(properties.PrivacyConfiguration);
            Advertisement.Initialize(gameId: properties.ApiKey,
                                     testMode: properties.IsDebugBuild,
                                     initializationListener: this);
        }

        public override AdPlacementState GetPlacementState(string placement)
        {
            return AdPlacementState.Unknown;
        }

        public override void LoadAd(string placement, AdContentOptions options)
        {
            var     placementMeta   = GetAdPlacementMeta(placement);
            var     adMeta          = GetAdMetaWithPlacement(placement);
            string  adUnitId        = adMeta.GetAdUnitIdForActiveOrSimulationPlatform();
            if (placementMeta.AdType == AdType.Banner)
            {
                CacheContentOption(adUnitId, options);
                LoadBannerAd(adUnitId, placementMeta);
            }
            else
            {
                Advertisement.Load(placementId: adUnitId, loadListener: this);
            }
        }

        public override void ShowAd(string placement)
        {
            var     placementMeta   = GetAdPlacementMeta(placement);
            var     adMeta          = GetAdMetaWithPlacement(placement);
            string  adUnitId        = adMeta.GetAdUnitIdForActiveOrSimulationPlatform();
            if (placementMeta.AdType == AdType.Banner)
            {
                var     bannerOptions   = GetContentOptions<BannerAdOptions>(adUnitId);
                ShowBannerAd(adUnitId, placementMeta, bannerOptions);
            }
            else
            {
                Advertisement.Show(placementId: adUnitId, showListener: this);
            }
        }

        public override void HideBanner(string placementId, bool destroy = false)
        {
            Advertisement.Banner.Hide(destroy: destroy);
        }

        public override void SetPaused(bool pauseStatus)
        { }

        public override void SetPrivacyConfiguration(ApplicationPrivacyConfiguration config)
        {
            bool    consent             = (config.UsageConsent == ConsentStatus.Authorized);

            // GDPR compliance
            var     gdprMetaData        = new MetaData("gdpr");
            gdprMetaData.Set("consent", consent);
            Advertisement.SetMetaData(gdprMetaData);

            // Check whether app uses custom age gate
            if (config.IsAgeRestrictedUser != null)
            {
                // COPPA compliance
                var     userMetaData    = new MetaData("user");
                userMetaData.Set("nonbehavioral", config.IsCoppaApplicable());
                Advertisement.SetMetaData(userMetaData);

                var     ageGateMetaData = new MetaData("privacy");
                ageGateMetaData.Set("useroveragelimit", !config.IsAgeRestrictedUser.Value);
                Advertisement.SetMetaData(ageGateMetaData);
            }

            // Other compliances eg: CCPA
            var     privacyMetaData     = new MetaData("privacy");
            privacyMetaData.Set("consent", consent);
            Advertisement.SetMetaData(privacyMetaData);
        }

        public override void SetUser(User user)
        {
            // Cache information
            GameUser    = user;
        }

        public override void SetUserSettings(UserSettings settings)
        { }

        public override void SetOrientation(ScreenOrientation orientation)
        { }

        #endregion

        #region Public methods

        public void SetMetaData(MetaData metaData)
        {
            Assert.IsArgNotNull(metaData, nameof(metaData));

            Advertisement.SetMetaData(metaData);
        }

        #endregion

        #region Private methods

        private void LoadBannerAd(string adUnitId, AdPlacementMeta placementMeta)
        {
            var     options         = new BannerLoadOptions()
            {
                errorCallback       = (message) =>
                {
                    var     stateInfo       = new AdNetworkLoadAdStateInfo(adUnitId: adUnitId,
                                                                           adType: placementMeta.AdType,
                                                                           networkId: NetworkId,
                                                                           placement: placementMeta.Name,
                                                                           placementState: AdPlacementState.NotAvailable,
                                                                           error: new Error(message));
                    SendLoadAdStateChangeEvent(stateInfo);
                },
                loadCallback        = () =>
                {
                    var     stateInfo       = new AdNetworkLoadAdStateInfo(adUnitId: adUnitId,
                                                                           adType: placementMeta.AdType,
                                                                           networkId: NetworkId,
                                                                           placement: placementMeta.Name,
                                                                           placementState: AdPlacementState.Ready);
                    SendLoadAdStateChangeEvent(stateInfo);
                }
            };
            Advertisement.Banner.Load(adUnitId, options);
        }

        private void ShowBannerAd(string adUnitId, AdPlacementMeta placementMeta, BannerAdOptions options)
        {
            var     unityOptions    = new BannerOptions()
            {
                showCallback = () =>
                {
                    var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                           adType: placementMeta.AdType,
                                                                           placement: placementMeta.Name,
                                                                           networkId: NetworkId,
                                                                           state: ShowAdState.Started);
                    SendShowAdStateChangeEvent(stateInfo);
                },
                hideCallback = () =>
                {
                    var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                           adType: placementMeta.AdType,
                                                                           placement: placementMeta.Name,
                                                                           networkId: NetworkId,
                                                                           state: ShowAdState.Finished,
                                                                           resultCode: ShowAdResultCode.Finished);
                    SendShowAdStateChangeEvent(stateInfo);
                },
                clickCallback = () =>
                {
                    var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                           adType: placementMeta.AdType,
                                                                           placement: placementMeta.Name,
                                                                           networkId: NetworkId,
                                                                           state: ShowAdState.Clicked);
                    SendShowAdStateChangeEvent(stateInfo);
                },
            };
            Advertisement.Banner.SetPosition(UnityAdsUtility.ConvertToUnityPosition(options.Position.Preset ?? AdPositionPreset.BottomCenter));
            Advertisement.Banner.Show(adUnitId, unityOptions);
        }

        #endregion

        #region IUnityAdsInitializationListener implementation

        public void OnInitializationComplete()
        {
            var     stateInfo   = new AdNetworkInitialiseStateInfo(networkId: NetworkId,
                                                                   status: AdNetworkInitialiseStatus.Success);
            SendInitaliseStateChangeEvent(stateInfo);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            var     stateInfo   = new AdNetworkInitialiseStateInfo(networkId: NetworkId,
                                                                   status: AdNetworkInitialiseStatus.Fail,
                                                                   error: new Error(message));
            SendInitaliseStateChangeEvent(stateInfo);
        }

        #endregion

        #region IUnityAdsLoadListener implementation

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkLoadAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   networkId: NetworkId,
                                                                   placement: placementMeta.Name,
                                                                   placementState: AdPlacementState.Ready);
            SendLoadAdStateChangeEvent(stateInfo);
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkLoadAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   networkId: NetworkId,
                                                                   placement: placementMeta.Name,
                                                                   placementState: AdPlacementState.NotAvailable,
                                                                   error: new Error(message));
            SendLoadAdStateChangeEvent(stateInfo);
        }

        #endregion

        #region IUnityAdsShowListener implementation

        public void OnUnityAdsShowStart(string adUnitId)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   placement: placementMeta.Name,
                                                                   networkId: NetworkId,
                                                                   state: ShowAdState.Started);
            SendShowAdStateChangeEvent(stateInfo);
        }

        public void OnUnityAdsShowClick(string adUnitId)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   placement: placementMeta.Name,
                                                                   networkId: NetworkId,
                                                                   state: ShowAdState.Clicked);
            SendShowAdStateChangeEvent(stateInfo);
        }

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   placement: placementMeta.Name,
                                                                   networkId: NetworkId,
                                                                   state: ShowAdState.Finished,
                                                                   resultCode: UnityAdsUtility.ConvertToShowResultCode(showCompletionState));
            SendShowAdStateChangeEvent(stateInfo);
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            var     placementMeta   = GetAdPlacementMetaWithAdUnitId(adUnitId: adUnitId);
            var     stateInfo       = new AdNetworkShowAdStateInfo(adUnitId: adUnitId,
                                                                   adType: placementMeta.AdType,
                                                                   placement: placementMeta.Name,
                                                                   networkId: NetworkId,
                                                                   state: ShowAdState.Failed,
                                                                   error: new Error(message));
            SendShowAdStateChangeEvent(stateInfo);
        }

        #endregion
    }
}