using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class IntersectionDrawer : SetupWindowBase
    {
        public delegate void IntersectionClicked(GenericIntersectionSettings clickedIntersection);
        public static event IntersectionClicked onIntersectionClicked;
        static void TriggetIntersectionClickedEvent(GenericIntersectionSettings clickedIntersection)
        {
            SettingsWindow.SetSelectedIntersection(clickedIntersection);
            if (onIntersectionClicked != null)
            {
                onIntersectionClicked(clickedIntersection);
            }
        }

        private static GUIStyle style = new GUIStyle();


        public static void DrawIntersection(GenericIntersectionSettings intersection, Color color, List<IntersectionStopWaypointsSettings> stopWaypoints, Color stopWaypointsColor, Color textColor, List<WaypointSettings> exitWaypoints = null, Color exitWaypointsColor = new Color())
        {
            if (GleyUtilities.IsPointInsideView(intersection.transform.position))
            {
                Handles.color = color;
                if (Handles.Button(intersection.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 1f, 1f, Handles.DotHandleCap))
                {
                    TriggetIntersectionClickedEvent(intersection);
                }
                style.normal.textColor = color;
                Handles.Label(intersection.transform.position, "\n" + intersection.name, style);
                for (int i = 0; i < stopWaypoints.Count; i++)
                {
                    DrawStopWaypoints(stopWaypoints[i].roadWaypoints, stopWaypointsColor, i + 1, textColor);
                }

                if (exitWaypoints != null)
                {
                    Handles.color = exitWaypointsColor;
                    for (int i = 0; i < exitWaypoints.Count; i++)
                    {
                        if (exitWaypoints[i] != null)
                        {
                            Handles.DrawSolidDisc(exitWaypoints[i].transform.position, Vector3.up, 1);
                        }
                        else
                        {
                            exitWaypoints.RemoveAt(i);
                        }
                    }
                }
            }
        }


        public static void DrawStopWaypoints(List<WaypointSettings> stopWaypoints, Color stopWaypointsColor, int road, Color textColor)
        {
            Handles.color = stopWaypointsColor;
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.alignment = TextAnchor.UpperRight;
            centeredStyle.normal.textColor = textColor;
            centeredStyle.fontStyle = FontStyle.Bold;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                if (stopWaypoints[i] != null)
                {
                    Handles.DrawSolidDisc(stopWaypoints[i].transform.position, Vector3.up, 1);
                    Handles.Label(stopWaypoints[i].transform.position, road.ToString(), centeredStyle);
                }
                else
                {
                    stopWaypoints.RemoveAt(i);
                }
            }
        }


        public static void DrawIntersectionWaypoint(WaypointSettingsBase stopWaypoint, Color stopWaypointsColor, int road, Color textColor)
        {
            Handles.color = stopWaypointsColor;
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.alignment = TextAnchor.UpperRight;
            centeredStyle.normal.textColor = textColor;
            centeredStyle.fontStyle = FontStyle.Bold;
            Handles.DrawSolidDisc(stopWaypoint.transform.position, Vector3.up, 1);
            Handles.Label(stopWaypoint.transform.position, road.ToString(), centeredStyle);
        }
    }
}
