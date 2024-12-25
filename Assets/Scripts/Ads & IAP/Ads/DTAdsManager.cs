using IAP;
using UnityEngine;
using VoxelBusters.AdsKit;
namespace Managers
{
    [RequireComponent(typeof(DTAdEventHandler))]
    public class DTAdsManager : MonoBehaviour
    {
        #region Public-Variables
        public bool adsRemoved;
        public bool sceneLoaded;
        public bool isInitialised;
        #endregion

        #region Private-Variables
        private LoadAdRequest _loadedAdRequest;
        private string _currentPlacementId;
        #endregion
        #region Singleton

        private static DTAdsManager _instance;

        private static readonly object Lock = new();
    

        public static DTAdsManager Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null) return _instance;
                    _instance = (DTAdsManager)FindObjectOfType(typeof(DTAdsManager));

                    if (FindObjectsOfType(typeof(DTAdsManager)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance != null) return _instance;

                    return null;
                }
            }
        }

        private static bool IsDontDestroyOnLoad()
        {
            if (_instance == null)
            {
                return false;
            }

            if ((_instance.gameObject.hideFlags & HideFlags.DontSave) == HideFlags.DontSave)
            {
                return true;
            }

            return false;
        }

        private static bool _applicationIsQuitting;

        public void OnDestroy()
        {
            if (IsDontDestroyOnLoad())
            {
                _applicationIsQuitting = true;
            }
        }
    
        #endregion

        #region Unity-Calls

        private void OnEnable()
        {
            DTAdEventHandler.OnAdsInitializationFailed += InitializeAdsManager;
            DTAdEventHandler.OnAdsLoadFailed += ReloadAd;
            //  DTAdEventHandler.OnAdsLoaded += ShowAd;
        //    DTAdEventHandler.OnAdsShowFailed += ShowAdAgain;
            IAPManager.OnRemoveAds += RemoveAds;
        }
        private void OnDisable()
        {
            DTAdEventHandler.OnAdsInitializationFailed -= InitializeAdsManager;
            DTAdEventHandler.OnAdsLoadFailed -= ReloadAd;
            // DTAdEventHandler.OnAdsLoaded -= ShowAd;
          //  DTAdEventHandler.OnAdsShowFailed -= ShowAdAgain;
            IAPManager.OnRemoveAds -= RemoveAds;
        }
        private void Awake()
        {
            if(Instance!=null)
                DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeAdsManager();
            sceneLoaded = true;
        }

        #endregion

        #region Ads-Manager

        #region Public-Methods
        // Initialize Ads Manager 
        private void InitializeAdsManager()
        {
            IConsentFormProvider contentForm = GetConsentFormProvider();
                if (contentForm != null)
                    InitialiseAdsManager(contentForm);
        }
        private void ReloadAd()
        {
            if(!string.IsNullOrEmpty(_currentPlacementId))
                LoadAd(_currentPlacementId);
        }
        public void LoadAd(string placementId)
        {
            if(adsRemoved && placementId.Equals(Constants.InterstitialId))
                return;
            _currentPlacementId = placementId;
            if(Utils.CheckInternetAvailability())
             _loadedAdRequest= AdsManager.LoadAd(placementId);
        }

        private void ShowAdAgain(string placementId)
        {
            if (Utils.CheckInternetAvailability())
            {
                _loadedAdRequest = AdsManager.LoadAd(placementId);
                _loadedAdRequest.OnComplete += (handle =>
                {
                    if (handle.Error==null)
                        ShowAd(placementId);
                    else
                        Debug.Log("Failed initialising Ads Kit with: " + handle.Error);
                });
            }
        }

        public void ShowAd(string placementId)
        {
            if(!sceneLoaded)
                return;
            if (adsRemoved && placementId.Equals(Constants.InterstitialId))
                return;
            _currentPlacementId = placementId;
            if (Utils.CheckInternetAvailability())
                AdsManager.ShowAd(placementId);
        }

        public void HideBannerAd(string placementId,bool destroy=false)
        {
            _currentPlacementId = placementId;
            if(!destroy)
                AdsManager.HideAd(placementId);
            else
                AdsManager.HideAd(placementId, destroy: true);
        }

        private void RemoveAds()
        {
            DTAdsManager.Instance.HideBannerAd(Constants.BannerID);
            adsRemoved = true;
        }

    
        #endregion
        #region Private-Methods
        
        private IConsentFormProvider GetConsentFormProvider()
        {
            var consentFormProvider = AdServices.GetConsentFormProvider();
            if(consentFormProvider == null)
                Debug.Log("There are no IConsentFormProvider implementations available. For a default plugin's implementation, enable Ad Mob network or implement a custom IConsentFormProvider on your own");
            return consentFormProvider;
        }
        private void InitialiseAdsManager(IConsentFormProvider consentProvider)
        {
            if(AdsManager.IsInitialisedOrWillChange)
            {
                Debug.Log("Initialisation is in progress or already initialised.");
                return;
            }
            if (Utils.CheckInternetAvailability())
            {
             var operation=AdsManager.Initialise(consentProvider);
             operation.OnComplete+=(op) =>
             {
                 if (op.Error == null)
                 {
                     isInitialised = true;
                     AdsManager.RegisterListener(GetComponent<DTAdEventHandler>());
                     Debug.Log("Initialise complete. You can start loading or showing the ads from now.");
                 }
                 else
                     Debug.Log("Failed initialising Ads Kit with: " + op.Error);
             };
            }
        }
        /*private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            sceneLoaded = true;
        }
        private void SceneUnloaded(Scene arg0)
        {
            sceneLoaded = false;
        }*/
        #endregion
        #endregion
     
    }
}
