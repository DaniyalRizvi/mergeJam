#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class UnityAdsAsset : AdNetworkAssetInfo
    {
       #region Constructors

       public UnityAdsAsset()
            : base(networkId: AdNetworkServiceId.kUnityAds,
                   name: "Unity Ads",
                   description: "Created by the leading mobile game engine Unity.",
                   importPaths: new string[]
                   {
                       "Packages/com.unity.ads",
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/UnityAdsAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Packages/com.unity.ads",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/UnityAds",
                   })
        { }

        #endregion
    }
}
#endif