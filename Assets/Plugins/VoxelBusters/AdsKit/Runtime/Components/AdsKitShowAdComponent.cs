using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit
{
    [AddComponentMenu("Voxel Busters/Ads Kit/Show Ad")]
    [DisallowMultipleComponent]
    public class AdsKitShowAdComponent : ActionTriggerComponent
    {
        #region Fields

        [SerializeField, AdPlacement]
        private     string              m_placement;

        #endregion

        #region Events
        [Space]
        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<string> m_onShow;

        [SerializeField]
        private UnityEvent<string> m_onFinish;

        [SerializeField]
        private UnityEvent<string> m_onClick;

        [SerializeField]
        private UnityEvent<string, string> m_onError;

        #endregion


        #region Public methods

        public void ShowAd()
        {
            ShowAdInternal();
        }

        public override void ExecuteAction()
        {
            ShowAd();
        }

        #endregion

        #region Private methods

        private void ShowAdInternal()
        {
            if (string.IsNullOrEmpty(m_placement))
            {
                SendErrorEvent($"Placement is null or empty.");
                return;
            }

            if (!AdsManager.IsInitialised)
            {
                SendErrorEvent($"Ads Kit not initialised. Either initialise with AdsKitIniitialiseComponent or AdsKitManager.Initialise");
                return;
            }

            IsDone = true;
            var operation = AdsManager.ShowAd(m_placement);

            operation.OnAdStart += (adContent) =>
            {
                m_onShow?.Invoke(adContent.Placement);
            };

            operation.OnAdClick += (adContent) =>
            {
                m_onClick?.Invoke(adContent.Placement);
            };

            operation.OnAdFinish += (adContent) =>
            {
                m_onFinish?.Invoke(adContent.Placement);
            };

            operation.OnComplete += (op) =>
            {
                if(op.Error != null)
                {
                    SendErrorEvent(((AdContent)op).Error.Description);
                }
            };
        }

        private void SendErrorEvent(string errorDescription)
        {
            DebugLogger.LogError(AdsKitSettings.Domain, errorDescription);
            m_onError?.Invoke(m_placement, errorDescription);
        }

        #endregion
    }
}