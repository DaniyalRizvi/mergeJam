using System;
using System.Collections;
using Managers;
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

namespace IAP
{
    public class IAPManager : Singelton<IAPManager>, IDetailedStoreListener
    {
        public Button restorePurchasesButton;
        [SerializeField] private Transform shopItemParent;

        private static IStoreController _storeController; 
        private static IExtensionProvider _storeExtensionProvider; 
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googleExtensions;
        
        public static event RemoveAdsClicked OnRemoveAds;
        public delegate void RemoveAdsClicked();
        public Action OnInitializedPurchasing;
        
        const string k_Environment = "production";
        private bool IsUGSInit = false;

        protected override void Awake()
        {
            base.Awake();
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
            builder = LoadShopData(builder);
            UnityPurchasing.Initialize(this, builder);
        }

        private ConfigurationBuilder LoadShopData(ConfigurationBuilder builder)
        {
            var json = Resources.Load<TextAsset>("ShopData");
            var shopData = JsonConvert.DeserializeObject<ShopData>(json.text);
            foreach (var datum in shopData.ShopDataItem)
            {
                builder.AddProduct(datum.BundleID, ProductType.Consumable);
            }

            return builder;
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
            UIManager.Instance.EnableIAPOverlay(true);
            BuyProductID(bundleID);
        }

        #endregion

        #region IAP Callbacks

        public bool IsInitialized()
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
            UIManager.Instance.EnableIAPOverlay(false);
            string id = args.purchasedProduct.definition.id;
            var json = Resources.Load<TextAsset>("ShopData");
            switch (id)
            {
                case "mj_no_ads":
                {
                    //Note: Remove Banner Ads
                    DTAdsManager.Instance.adsRemoved = true;
                    break;
                }
                case "mj_bundle_vip":
                {
                    DTAdsManager.Instance.adsRemoved = true;
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Rocket,5);
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Fan,5);
                    break;
                }
                case "mj_gems_small":
                {
                    GemsManager.Instance.AddGems(50);
                    break;
                }
                case "mj_gems_medium":
                {
                    GemsManager.Instance.AddGems(150);
                    break;
                }
                case "mj_gems_large":
                {
                    GemsManager.Instance.AddGems(700);
                    break;
                }
                case "mj_gems_giant":
                {
                    GemsManager.Instance.AddGems(1800);
                    break;
                }
                case "mj_bundle_mega":
                {
                    GemsManager.Instance.AddGems(1000);
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Rocket,10);
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Fan,10);
                    break;
                }
                case "mj_bundle_delux":
                {
                    GemsManager.Instance.AddGems(1500);
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Rocket,25);
                    PowerUpsManager.Instance.AddPowerUp(PowerUpType.Fan,25);
                    break;
                }
            }

            return PurchaseProcessingResult.Complete;
        }
        public void OnPurchaseDeferred(Product product)
        {
            UIManager.Instance.EnableIAPOverlay(false);
            Debug.Log("Deferred product " + product.definition.id);
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            UIManager.Instance.EnableIAPOverlay(false);
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
            UIManager.Instance.EnableIAPOverlay(false);
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
                        HandleRestoredPurchase(item);
                        //if(item.definition.id== "mj_no_ads")
                        //{

                        //}
                    }
                    else
                    {
                    }
                });
            }
            
        }
        private void HandleRestoredPurchase(Product product)
        {
            // Example: unlock content or features for the user based on the product ID
            if (product.definition.id == "mj_no_ads")
            {
                if(product.hasReceipt)
                {

                Debug.Log("Restored 'your_product_id'. Unlocking content.");
                DTAdsManager.Instance.adsRemoved = true;
                }
                // Unlock content or update game state here
            }
        }
        #endregion

    }
}
