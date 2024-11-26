using UnityEngine;
using VoxelBusters.AdsKit;
using VoxelBusters.CoreLibrary;

namespace Managers
{
    public class DTAdEventHandler : MonoBehaviour,IAdLifecycleEventListener
    {
        #region Events

        public static event AdsInitializedSuccess OnAdsInitializationSuccess;
        public delegate void AdsInitializedSuccess();
        public static event AdsInitializedFailed OnAdsInitializationFailed;
        public delegate void AdsInitializedFailed();
        
        public static event AdsLoaded OnAdsLoaded;
        public delegate void AdsLoaded(string placementId);
        
        public static event AdsShowed OnAdsShowed;
        public delegate void AdsShowed(string placementId);
        
        public static event AdsLoadFailed OnAdsLoadFailed;
        public delegate void AdsLoadFailed();
        
        public static event AdsShowFailed OnAdsShowFailed;
        public delegate void AdsShowFailed(string placementId);
        
        #endregion
        public int CallbackOrder=>1;
        public void OnInitialisationComplete(InitialiseResult result)
        {
            //Debug.Log("Initialization Success... "+result.SubscribedAdNetworks.Length);
            OnAdsInitializationSuccess?.Invoke();
        }

        public void OnInitialisationFail(Error error)
        {
            Debug.Log("Initialization Failed... "+error.Code+":::: "+error.Description);
            OnAdsInitializationFailed?.Invoke();
        }

        public void OnLoadAdComplete(string placement, LoadAdResult result)
        {
            Debug.Log("Ad Loaded Successfully.... ");
            OnAdsLoaded?.Invoke(placement);
        }

        public void OnLoadAdFail(string placement, Error error)
        {
            Debug.Log("Ad Load Failed.... "+error.Code+" :::: "+error.Description);
            OnAdsLoadFailed?.Invoke();
        }

        public void OnShowAdStart(string placement)
        {
            //Debug.Log($"started showing ad for placementId: {placement}.");
        }

        public void OnShowAdClick(string placement)
        {
            //Debug.Log($"recognised click on ad for placementId: {placement}.");
        }

        public void OnShowAdComplete(string placement, ShowAdResult result)
        {
          //  Debug.Log($"showed ad for placementId: {placement}.");
          OnAdsShowed?.Invoke(placement);
          //Toast.Show($"Watched Ad Successfully!", ToastColor.General, ToastPosition.MiddleCenter);
        }

        public void OnShowAdFail(string placement, Error error)
        {
            //Debug.Log($"Show ad Failed for placementId: {placement}.");
            OnAdsShowFailed?.Invoke(placement);
        }

        public void OnAdImpressionRecorded(string placement)
        {
        //    Debug.Log($"recorded impression for ad for placementId: {placement}.");
        }

        public void OnAdPaid(string placement, AdTransaction transaction)
        {
          //  Debug.Log($"recorded pay for ad: {placement} with transaction: {transaction}.");
        }
    }
}
