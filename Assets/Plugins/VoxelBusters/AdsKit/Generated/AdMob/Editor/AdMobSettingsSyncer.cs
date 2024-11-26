using System;
using System.Reflection;
using GoogleMobileAds.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VoxelBusters.AdsKit.Editor;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.AdsKit.Adapters.Editor
{
    public class AdMobSettingsSyncer : AdNetworkSettingsSyncer
    {

        protected override string GetNetworkId() => AdNetworkServiceId.kGoogleMobileAds;
        
        protected override void Sync(string apiKey, string apiSecret)
        {
            if(!string.IsNullOrEmpty(apiKey))
            {
                UnityEngine.Object adMobSettings = GetAdMobSettings();
                if(adMobSettings != null)
                {
                    #if UNITY_ANDROID
                        var propertyName = "GoogleMobileAdsAndroidAppId";
                    #elif UNITY_IOS
                        var propertyName = "GoogleMobileAdsIOSAppId";
                    #endif

                    var existingValue = adMobSettings.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance); //TODO: Move this code to ReflectionUtility after Essential Kit v3 release
                    if(apiKey != null && !apiKey.Equals(existingValue))
                    {
                        ReflectionUtility.SetPropertyValue(adMobSettings, propertyName, apiKey);    
                        EditorUtility.SetDirty(adMobSettings);
                        AssetDatabase.SaveAssets();
                    }
                }
                else
                {
                    DebugLogger.LogError("Unable to fetch admob settings via reflection. Contact AdsKit plugin support with your AdMob version.");
                }
            }
        }

        private UnityEngine.Object GetAdMobSettings()
        {
                Type type = Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");

                if(type == null)
                    return null;

                object admobSettings = type.GetMethod("LoadInstance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
                return (UnityEngine.Object)admobSettings;
        }
    }
}
