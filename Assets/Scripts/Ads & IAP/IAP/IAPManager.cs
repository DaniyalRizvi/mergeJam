using System;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace IAP
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        #region Singleton

        private static IAPManager _instance;

        private static readonly object Lock = new();

        public static IAPManager Instance
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
                    _instance = (IAPManager)FindObjectOfType(typeof(IAPManager));

                    if (FindObjectsOfType(typeof(IAPManager)).Length > 1)
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

        public Button restorePurchasesButton;

        private static IStoreController _storeController; 
        private static IExtensionProvider _storeExtensionProvider; 
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googleExtensions;
        
        public static event RemoveAdsClicked OnRemoveAds;
        public delegate void RemoveAdsClicked();
        public Action OnInitializedPurchasing;
        
        const string k_Environment = "production";
        private bool IsUGSInit = false;

        private void Awake()
        {
            Initialize(OnSuccess, OnError);
        }
        void Initialize(Action onSuccess, Action<string> onError)
        {
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(k_Environment);
        
                UnityServices.InitializeAsync(options).ContinueWith(task => onSuccess());
            }
            catch (Exception exception)
            {
                onError(exception.Message);
            }
        }
        
        void OnSuccess()
        {
            var text = "Congratulations!\nUnity Gaming Services has been successfully initialized.";
            Debug.Log(text);
            IsUGSInit = true;
        }
        
        void OnError(string message)
        {
            var text = $"Unity Gaming Services failed to initialize with error: {message}.";
            Debug.LogError(text);
            IsUGSInit = true;
        }


        private IEnumerator Start()
        {
            yield return new WaitUntil(() => IsUGSInit);
            Init();
        }

        private void Init()
        {
            if (_storeController == null)
                InitializePurchasing();
            
            if(restorePurchasesButton)
                restorePurchasesButton.onClick.AddListener(RestorePurchases);
        }
        private void InitializePurchasing()
        {
            if (IsInitialized()) return;

            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
            var module = StandardPurchasingModule.Instance();
            module.useFakeStoreAlways = true;
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("mj_gems_5", ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }
        private void BuyProductID(string productId)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (IsInitialized())
                {
                    var product = _storeController.products.WithID(productId);

                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product:" + product.definition.id));
                        _storeController.InitiatePurchase(product);
                    }
                    else
                    {
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
            }
        }
        public string GetProductPrice(string productID)
        {
            var product = _storeController.products.WithID(productID);
            return product != null ? product.metadata.localizedPriceString : "--";
        }
        #region Purchasing Functions

        public void BuyBundle(string bundleID)
        {
            BuyProductID(bundleID);
        }

        #endregion

        #region IAP Callbacks

        private bool IsInitialized()
        {
            return _storeController != null && _storeExtensionProvider != null;
        }
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError(error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError(error+ " :     " + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string id = args.purchasedProduct.definition.id;

            Debug.LogError($"Purchased: {args.purchasedProduct.definition.id}");
            switch (id)
            {
                case "mj_gems_5":
                {
                    GemsManager.Instance.AddGems(5);
                    break;
                }
            }
            return PurchaseProcessingResult.Complete;
        }
        public void OnPurchaseDeferred(Product product)
        {
            Debug.Log("Deferred product " + product.definition.id);
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId, failureReason));
        }
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
            _appleExtensions = extensions.GetExtension<IAppleExtensions>();
            _googleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            var dict = _appleExtensions.GetIntroductoryPriceDictionary();

            foreach (var item in controller.products.all)
            {
                if (item.receipt != null)
                {
                    var introJson = dict == null || !dict.TryGetValue(item.definition.storeSpecificId, out var value)
                        ? null
                        : value;

                    if (item.definition.type == ProductType.Subscription)
                    {
                        var p = new SubscriptionManager(item, introJson);
                        var info = p.getSubscriptionInfo();
                        Debug.Log("SubInfo: " + info.getProductId());
                        Debug.Log("isSubscribed: " + info.isSubscribed());
                        Debug.Log("isFreeTrial: " + info.isFreeTrial());
                    }
                }
            }
            
            
            RestorePurchases();

            OnInitializedPurchasing?.Invoke();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId, failureDescription.message)); 
        }

        private void RestorePurchases()
        {
            foreach (var item in _storeController.products.all)
            {
                if(item.definition.type != ProductType.NonConsumable) continue;
                
                _storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions ((x, _) => {
                    if (x)
                    {
                    }
                    else
                    {
                    }
                });
            }
            
        }
        #endregion
        
    }
}
