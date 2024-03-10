using GleyUrbanAssets;
using System;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public partial class TrafficLightsIntersectionWindow : IntersectionWindowBase
    {
        private TrafficLightsIntersectionSettings selectedTrafficLightsIntersection;
        private float scrollAdjustment = 223;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            selectedTrafficLightsIntersection = selectedIntersection as TrafficLightsIntersectionSettings;
            stopWaypoints = selectedTrafficLightsIntersection.stopWaypoints;
            return base.Initialize(windowProperties, window);
        }

       

        protected override void TopPart()
        {
            base.TopPart();
            selectedTrafficLightsIntersection.greenLightTime = EditorGUILayout.FloatField("Green Light Time", selectedTrafficLightsIntersection.greenLightTime);
            selectedTrafficLightsIntersection.yellowLightTime = EditorGUILayout.FloatField("Yellow Light Time", selectedTrafficLightsIntersection.yellowLightTime);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            DrawStopWaypointButtons(true);
            
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


       
    }
}
