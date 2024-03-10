using GleyUrbanAssets;
using System;
using UnityEditor;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class SettingsWindow : SettingsWindowBase
    {
        private const string WINDOW_NAME = "Traffic System - v.";
        private const string PATH = "Assets//GleyPlugins/TrafficSystem/Scripts/Version.txt";
        private const int MIN_WIDTH = 400;
        private const int MIN_HEIGHT = 500;

        static SettingsWindow trafficWindow;
        static TrafficWindownNavigationData windowNavigationData;


        [MenuItem("Window/Gley/Traffic System", false, 90)]
        private static void Initialize()
        {
            trafficWindow = WindowLoader.LoadWindow<SettingsWindow>(PATH, WINDOW_NAME, MIN_WIDTH, MIN_HEIGHT);
            trafficWindow.Init(trafficWindow, typeof(MainMenuWindow), AllWindowsData.GetWindowsData(), new AllSettingsWindows());
        }


        protected override void Reinitialize()
        {
            if (trafficWindow == null)
            {
                Initialize();
            }
            else
            {
                trafficWindow.Init(trafficWindow, typeof(MainMenuWindow), AllWindowsData.GetWindowsData(), new AllSettingsWindows());
            }

        }


        protected override void ResetToHomeScreen(Type defaultWindow, bool now)
        {
            windowNavigationData = new TrafficWindownNavigationData();
            windowNavigationData.InitializeData();
            base.ResetToHomeScreen(defaultWindow, now);
        }


        protected override void MouseMove(Vector3 point)
        {
            if (activeSetupWindow.GetType() == typeof(EditRoadWindow))
            {
                activeSetupWindow.MouseMove(point);
            }
        }


        internal override LayerMask GetGroundLayer()
        {
            return windowNavigationData.GetRoadLayers();
        }


        //TODO these should not be static methods
        internal static void SetSelectedWaypoint(WaypointSettings waypoint)
        {
            windowNavigationData.SetSelectedWaypoint(waypoint);
        }


        internal static WaypointSettings GetSelectedWaypoint()
        {
            return windowNavigationData.GetSelectedWaypoint();
        }


        internal static void SetSelectedIntersection(GenericIntersectionSettings clickedIntersection)
        {
            windowNavigationData.SetSelectedIntersection(clickedIntersection);
        }


        internal static GenericIntersectionSettings GetSelectedIntersection()
        {
            return windowNavigationData.GetSelectedIntersection();
        }


        internal static Road GetSelectedRoad()
        {
            return windowNavigationData.GetSelectedRoad();
        }


        internal static void SetSelectedRoad(Road road)
        {
            windowNavigationData.SetSelectedRoad(road);
        }


        internal static void UpdateLayers()
        {
            windowNavigationData.UpdateLayers();
        }
    }
}
