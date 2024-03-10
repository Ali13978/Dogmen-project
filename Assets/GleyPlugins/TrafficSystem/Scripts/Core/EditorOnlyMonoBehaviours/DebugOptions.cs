#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class DebugOptions
    {
        public static DebugSettings LoadOrCreateDebugSettings()
        {
            DebugSettings debugSettings = (DebugSettings)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Resources/DebugOptions.asset", typeof(DebugSettings));
            if (debugSettings == null)
            {
                DebugSettings asset = ScriptableObject.CreateInstance<DebugSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins"))
                {
                    AssetDatabase.CreateFolder("Assets/", "GleyPlugins");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins", "TrafficSystem");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins/TrafficSystem", "Resources");
                    AssetDatabase.Refresh();
                }

                AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/TrafficSystem/Resources/DebugOptions.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                debugSettings = (DebugSettings)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Resources/DebugOptions.asset", typeof(DebugSettings));
            }

            return debugSettings;
        }


        public static bool GetDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debug;
        }


        public static bool GetSpeedDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugSpeed;
        }


        public static bool GetIntersectionDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugIntersections;
        }


        public static bool GetWaypointsDebug()
        {
            DebugSettings debugSettings = LoadOrCreateDebugSettings();
            return debugSettings.debugWaypoints;
        }
    }
}
#endif