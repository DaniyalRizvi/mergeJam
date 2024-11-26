using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters
{
    public static class UnityAdsUtility
    {
        #region Static methods

        public static ShowAdResultCode ConvertToShowResultCode(UnityAdsShowCompletionState state)
        {
            switch (state)
            {
                case UnityAdsShowCompletionState.COMPLETED:
                    return ShowAdResultCode.Finished;

                case UnityAdsShowCompletionState.SKIPPED:
                    return ShowAdResultCode.Skipped;

                default:
                    DebugLogger.LogWarning($"Could not convert placement state value: {state}");
                    return ShowAdResultCode.Unknown;
            }
        }

        public static BannerPosition ConvertToUnityPosition(AdPositionPreset value)
        {
            switch (value)
            {
                case AdPositionPreset.BottomCenter:
                    return BannerPosition.BOTTOM_CENTER;

                case AdPositionPreset.BottomLeft:
                    return BannerPosition.BOTTOM_LEFT;

                case AdPositionPreset.BottomRight:
                    return BannerPosition.BOTTOM_RIGHT;

                case AdPositionPreset.Center:
                    return BannerPosition.CENTER;

                case AdPositionPreset.TopCenter:
                    return BannerPosition.TOP_CENTER;

                case AdPositionPreset.TopLeft:
                    return BannerPosition.TOP_LEFT;

                case AdPositionPreset.TopRight:
                    return BannerPosition.TOP_RIGHT;

                default:
                    return BannerPosition.CENTER;
            }
        }

        #endregion
    }
}