#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class TapjoyAsset : AdNetworkAssetInfo
    {
       #region Constructors

       public TapjoyAsset()
            : base(networkId: AdNetworkServiceId.kTapjoy,
                   name: AdNetworkServiceId.kTapjoy,
                   description: "Tapjoy is an advertising and app monetisation platform.",
                   importPaths: new string[]
                   {
                       "https://dev.tapjoy.com/en/unity-plugin/Quickstart#id-2-sdk-integration",
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/TapjoyAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Assets/TapjoySDK",
                       $"{AdsKitSettings.Package.GetEssentialsPath()}/Scripts/Tapjoy",
                   })
        { }

        #endregion
    }
}
#endif