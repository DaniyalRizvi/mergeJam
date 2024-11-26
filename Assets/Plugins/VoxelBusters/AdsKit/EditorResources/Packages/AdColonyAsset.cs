#if UNITY_EDITOR
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;

namespace VoxelBusters.AdsKit.Editor.Clients
{
    public class AdColonyAsset : AdNetworkAssetInfo 
    {
       #region Constructors

       public AdColonyAsset()
            : base(networkId: AdNetworkServiceId.kAdColony,
                   name: AdNetworkServiceId.kAdColony,
                   description: "The Leading Independent Growth & Monetization Platform.",
                   importPaths: new string[]
                   {
                       "https://github.com/AdColony/AdColony-Unity-SDK/raw/master/Packages",
                       $"{AdsKitSettings.Package.GetEditorResourcesPath()}/Packages/AdColonyAdapter.unitypackage",
                   },
                   installPaths: new string[]
                   {
                       "Assets/AdColony",
                       $"{AdsKitSettings.Package.GetGeneratedPath()}/AdColony",
                   })
        { }

        #endregion

        #region Base class methods

        public override void OnInstall()
        {
            var     mainPath        = InstallPaths[0];
            AssemblyDefinitionServices.CreateDefinition(path: IOServices.CombinePath(mainPath, "Scripts"),
                                                        name: "AdColony");
            AssemblyDefinitionServices.CreateDefinition(path: IOServices.CombinePath(mainPath, "Editor"),
                                                        name: "AdColony.Editor",
                                                        includePlatforms: new string[] { "Editor" },
                                                        references: new string[] { "AdColony" });
        }

        #endregion
    }
}
#endif