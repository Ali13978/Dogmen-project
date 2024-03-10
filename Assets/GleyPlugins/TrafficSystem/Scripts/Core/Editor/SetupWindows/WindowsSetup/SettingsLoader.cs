using System.Collections.Generic;
using UnityEditor;

namespace GleyUrbanAssets
{
    public partial class SettingsLoader
    {
        internal void SaveEditRoadSettings(EditRoadSave editRoadSave, bool[] allowedCarIndex, RoadColors roadColors, RoadDefaults roadDefaults)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            editRoadSave.globalCarList = new List<GleyTrafficSystem.VehicleTypes>();
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                if (allowedCarIndex[i] == true)
                {
                    editRoadSave.globalCarList.Add((GleyTrafficSystem.VehicleTypes)i);
                }
            }
            settingsWindowData.editRoadSave = editRoadSave;
            settingsWindowData.roadColors = roadColors;
            settingsWindowData.roadDefaults = roadDefaults;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal EditRoadSave LoadEditRoadSave()
        {
            return LoadSettingsAsset().editRoadSave;
        }


        internal void SaveSpeedRoutes(SpeedRoutesSave speedRoutesSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.speedRoutesSave = speedRoutesSave;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal SpeedRoutesSave LoadSpeedRoutes()
        {
            return LoadSettingsAsset().speedRoutesSave;
        }

        internal void SaveIntersectionsSettings(IntersectionSave intersectionSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.intersectionSave = intersectionSave;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal IntersectionSave LoadIntersectionsSettings()
        {
            return LoadSettingsAsset().intersectionSave;
        }

        internal void SaveGiveWayWaypointsSettings(ViewWaypointsSettings giveWayWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.giveWayWaypointsSettings = giveWayWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadGiveWayWaypointsSave()
        {
            return LoadSettingsAsset().giveWayWaypointsSettings;
        }

        

        internal void SaveSpeedEditedWaypointsSettings(ViewWaypointsSettings speedEditedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.speedEditedWaypointsSettings = speedEditedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadSpeedEditedWaypointsSave()
        {
            return LoadSettingsAsset().speedEditedWaypointsSettings;
        }

        internal void SaveStopWaypointsSettings(ViewWaypointsSettings stopWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.stopWaypointsSettings = stopWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        internal ViewWaypointsSettings LoadStopdWaypointsSave()
        {
            return LoadSettingsAsset().stopWaypointsSettings;
        }

       
    }
}
