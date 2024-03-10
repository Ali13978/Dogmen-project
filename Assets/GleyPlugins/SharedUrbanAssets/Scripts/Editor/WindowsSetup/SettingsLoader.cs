using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public partial class SettingsLoader
    {
        private string path;


        public SettingsLoader(string path)
        {
            this.path = path;
        }


        private SettingsWindowData LoadSettingsAsset()
        {
            SettingsWindowData settingsWindowData = (SettingsWindowData)AssetDatabase.LoadAssetAtPath(path, typeof(SettingsWindowData));

            if (settingsWindowData == null)
            {
                SettingsWindowData asset = ScriptableObject.CreateInstance<SettingsWindowData>();
                string[] pathFolders = path.Split('/');
                string tempPath = pathFolders[0];
                if (path.Contains("Pedestrian"))
                {
                    asset.roadDefaults = new RoadDefaults(1, 1, 4);
                }
                else
                {
                    asset.roadDefaults = new RoadDefaults(2, 4, 4);
                }
                for (int i = 1; i < pathFolders.Length - 1; i++)
                {
                    if (!AssetDatabase.IsValidFolder(tempPath + "/" + pathFolders[i]))
                    {
                        AssetDatabase.CreateFolder(tempPath, pathFolders[i]);
                        AssetDatabase.Refresh();
                    }

                    tempPath += "/" + pathFolders[i];
                }

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                settingsWindowData = (SettingsWindowData)AssetDatabase.LoadAssetAtPath(path, typeof(SettingsWindowData));
            }

            return settingsWindowData;
        }


        internal void SaveCreateRoadSettings(CreateRoadSave createRoadSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.createRoadSave = createRoadSave;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        internal CreateRoadSave LoadCreateRoadSave()
        {
            return LoadSettingsAsset().createRoadSave;
        }


        internal void SaveViewRoadsSettings(ViewRoadsSave viewRoadsSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.roadColors = roadColors;
            settingsWindowData.viewRoadsSave = viewRoadsSave;
            EditorUtility.SetDirty(settingsWindowData);
        }


        internal ViewRoadsSave LoadViewRoadsSave()
        {
            return LoadSettingsAsset().viewRoadsSave;
        }


        internal void SaveConnectRoadsSettings(ConnectRoadsSave connectRoadsSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.connectRoadsSave = connectRoadsSave;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        internal ConnectRoadsSave LoadConnectRoadsSave()
        {
            return LoadSettingsAsset().connectRoadsSave;
        }


        internal RoadDefaults LoadRoadDefaultsSave()
        {
            return LoadSettingsAsset().roadDefaults;
        }


        internal RoadColors LoadRoadColors()
        {
            return LoadSettingsAsset().roadColors;
        }


        internal void SaveRoadColors(RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        internal void SaveCarRoutes(CarRoutesSave carRoutesSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.carRoutesSave = carRoutesSave;
            EditorUtility.SetDirty(settingsWindowData);
        }


        internal CarRoutesSave LoadCarRoutes()
        {
            return LoadSettingsAsset().carRoutesSave;
        }


        internal void SaveAllWaypointsSettings(ViewWaypointsSettings allWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.allWaypointsSettings = allWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadAllWaypointsSave()
        {
            return LoadSettingsAsset().allWaypointsSettings;
        }

        internal void SaveDisconnectedWaypointsSettings(ViewWaypointsSettings disconnectedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.disconnectedWaypointsSettings = disconnectedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadDisconnectedWaypointsSave()
        {
            return LoadSettingsAsset().disconnectedWaypointsSettings;
        }

        internal void SaveCarEditedWaypointsSettings(ViewWaypointsSettings carEditedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.carEditedWaypointsSettings = carEditedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadCarEditedWaypointsSave()
        {
            return LoadSettingsAsset().carEditedWaypointsSettings;
        }

        internal void SavePathProblemsWaypointsSettings(ViewWaypointsSettings pathProblemsWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.pathProblemsWaypointsSettings = pathProblemsWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadPathProblemsWaypointsSave()
        {
            return LoadSettingsAsset().pathProblemsWaypointsSettings;
        }
    }
}