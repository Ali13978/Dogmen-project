using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointSetupWindow : SetupWindowBase
    {
        protected override void ScrollPart(float width, float height)
        {
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Show All Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowAllWaypoints), true);
               
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Disconnected Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowDisconnectedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Edited Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowVehicleTypeEditedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Speed Edited Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowSpeedEditedWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Give Way Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowGiveWayWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Stop Waypoints"))
            {
                window.SetActiveWindow(typeof(ShowStopWaypoints), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Show Vehicle Path Problems"))
            {
                window.SetActiveWindow(typeof(ShowVehiclePathProblems), true);
            }
            EditorGUILayout.Space();

            base.ScrollPart(width, height);
        }
    }
}
