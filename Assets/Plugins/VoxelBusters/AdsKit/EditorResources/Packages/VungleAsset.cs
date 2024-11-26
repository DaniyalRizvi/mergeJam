#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class VungleAsset : AdNetworkAssetInfo
    {
        #region Constructors

        public VungleAsset()
            : base(networkId: AdNetworkServiceId.kVungle,
                   name: "Vungle",
                   description: "Vungle is an ad network providing a technology platform.",
                   importPaths: new string[]
                   {
                       "https://github.com/Vungle/Unity-Plugin/tree/master",//6.11.0.1
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/VungleAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Assets/Vungle",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/Vungle",
                   })
        { }

        #endregion

        #region Base class methods

        public override void OnInstall()
        {
            var     mainPath        = InstallPaths[0];
            AssemblyDefinitionServices.CreateDefinition(path: IOServices.CombinePath(mainPath, "Scripts"),
                                                        name: "Vungle");
            AssemblyDefinitionServices.CreateDefinition(path: IOServices.CombinePath(mainPath, "Editor"),
                                                        name: "Vungle.Editor",
                                                        includePlatforms: new string[] { "Editor" },
                                                        references: new string[] { "Vungle" });
        }

        #endregion
    }
}
#endif