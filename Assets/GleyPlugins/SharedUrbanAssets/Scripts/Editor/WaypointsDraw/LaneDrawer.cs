using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class LaneDrawer
    {
        static GUIStyle style = new GUIStyle();


        public static void DrawAllLanes(RoadBase road, bool drawWaypoints, bool drawLaneChange, Color laneColor, Color waypointColor, Color disconnectedColor, Color laneChangeColor, Color textColor)
        {
            Transform lanesHolder = road.transform.Find(Constants.lanesHolderName);
            if (lanesHolder)
            {
                for (int i = 0; i < lanesHolder.childCount; i++)
                {
                    if (!drawWaypoints)
                    {
                        DrawSimplifiedLane(lanesHolder.GetChild(i), laneColor, textColor);
                    }
                    else
                    {
                        DrawLane(lanesHolder.GetChild(i), drawLaneChange, waypointColor, disconnectedColor, laneChangeColor);
                    }
                }
            }
        }


        public static void DrawLane(Transform holder, bool drawLaneChange, Color waypointColor, Color disconnectedColor, Color laneChangeColor)
        {
            if (holder != null)
            {
                for (int i = 0; i < holder.childCount; i++)
                {
                    WaypointSettingsBase waypointScript = holder.GetChild(i).GetComponent<WaypointSettingsBase>();
                    if (waypointScript != null)
                    {
                        if (waypointScript.neighbors.Count == 0 || waypointScript.prev.Count == 0)
                        {
                            DrawUnconnectedWaypoint(waypointScript.transform.position, disconnectedColor);
                        }

                        if (drawLaneChange)
                        {
                            for (int j = 0; j < waypointScript.otherLanes.Count; j++)
                            {
                                if (waypointScript.otherLanes[j] != null)
                                {
                                    DrawTriangle(waypointScript.transform.position, waypointScript.otherLanes[j].transform.position, laneChangeColor, true);
                                }
                                else
                                {
                                    for (int k = waypointScript.otherLanes.Count - 1; k >= 0; k--)
                                    {
                                        if (waypointScript.otherLanes[k] == null)
                                        {
                                            waypointScript.otherLanes.RemoveAt(k);
                                        }
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < waypointScript.neighbors.Count; j++)
                        {

                            if (waypointScript.neighbors[j] != null)
                            {
                                DrawTriangle(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, waypointColor, true);
                            }
                            else
                            {
                                for (int k = waypointScript.neighbors.Count - 1; k >= 0; k--)
                                {
                                    if (waypointScript.neighbors[k] == null)
                                    {
                                        waypointScript.neighbors.RemoveAt(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void DrawSimplifiedLane(Transform laneHolder, Color laneColor, Color textColor)
        {
            WaypointSettingsBase waypointScript;
            for (int i = 0; i < laneHolder.childCount; i++)
            {
                waypointScript = laneHolder.GetChild(i).GetComponent<WaypointSettingsBase>();
                for (int j = 0; j < waypointScript.neighbors.Count; j++)
                {
                    if (waypointScript.neighbors[j] != null)
                    {
                        DrawWaypointLine(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, laneColor);
                    }
                }
                if (i == 0 || i == laneHolder.childCount - 1)
                {
                    DrawLabel(waypointScript.transform.position, waypointScript.transform.parent.name, textColor);

                    if (waypointScript.neighbors.Count == 0)
                    {
                        if (waypointScript.prev.Count > 0)
                        {
                            DrawTriangle(waypointScript.prev[0].transform.position, waypointScript.transform.position, laneColor, false);
                        }
                    }

                    for (int j = 0; j < waypointScript.neighbors.Count; j++)
                    {
                        if (waypointScript.neighbors[j] == null)
                        {
                            waypointScript.neighbors.RemoveAt(j);
                        }
                        else
                        {
                            DrawTriangle(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, laneColor, false);
                        }
                    }
                }
            }
        }


        private static void DrawLabel(Vector3 position, string text, Color color)
        {
            style.normal.textColor = color;
            Handles.Label(position, text, style);
        }


        private static void DrawUnconnectedWaypoint(Vector3 position, Color disconnectedColor)
        {
            Handles.color = disconnectedColor;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(Vector3.up), 10, EventType.Repaint);
        }


        private static void DrawTriangle(Vector3 start, Vector3 end, Color waypointColor, bool drawLane)
        {
            Handles.color = waypointColor;
            Vector3 direction = (start - end).normalized;

            Vector3 point2 = end + Quaternion.Euler(0, -25, 0) * direction;

            Vector3 point3 = end + Quaternion.Euler(0, 25, 0) * direction;

            Handles.DrawPolyLine(end, point2, point3, end);

            if (drawLane)
            {
                Handles.DrawLine(start, end);
            }
        }


        private static void DrawWaypointLine(Vector3 start, Vector3 end, Color color)
        {
            Handles.color = color;
            Handles.DrawLine(start, end);
        }
    }
}
