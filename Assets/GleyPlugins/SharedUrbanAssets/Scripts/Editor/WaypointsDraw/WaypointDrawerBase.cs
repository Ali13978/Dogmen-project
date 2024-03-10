using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class WaypointDrawerBase<T> : Editor where T : WaypointSettingsBase
    {
        public delegate void WaypointClicked(T clickedWaypoint, bool leftClick);
        public event WaypointClicked onWaypointClicked;

        protected virtual void TriggetWaypointClickedEvent(T clickedWaypoint, bool leftClick)
        {
            if (onWaypointClicked != null)
            {
                onWaypointClicked(clickedWaypoint, leftClick);
            }
        }

        protected List<T> allWaypoints;
        protected GUIStyle style;

        private List<WaypointSettingsBase> disconnectedWaypoints;
        private List<WaypointSettingsBase> stopWaypoints;

        internal abstract void DrawWaypointsForCar(int car, Color color);
        protected abstract void DrawCompleteWaypoint(WaypointSettingsBase waypoint, Color color, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool drawPrev = false, Color prevColor = new Color());


        internal void Initialize()
        {
            style = new GUIStyle();
            LoadWaypoints();
        }


        internal void DrawAllWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
            }
        }


        internal List<WaypointSettingsBase> ShowDisconnectedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].neighbors.Count == 0)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, false, Color.white);
                }
            }

            if (nr != disconnectedWaypoints.Count)
            {
                UpdateDisconnectedWaypoints();
            }
            return disconnectedWaypoints;
        }


        internal List<WaypointSettingsBase> ShowStopWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].stop)
                    {
                        nr++;
                        DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                    }
                }
            }

            if (nr != stopWaypoints.Count)
            {
                UpdateStopWaypoints();
            }

            return stopWaypoints;
        }


        internal void DrawWaypointsForLink(WaypointSettingsBase currentWaypoint, List<WaypointSettingsBase> neighborsList, List<WaypointSettingsBase> otherLinesList, Color waypointColor, Color speedColor)
        {
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i] != currentWaypoint && !neighborsList.Contains(allWaypoints[i]) && !otherLinesList.Contains(allWaypoints[i]))
                {
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, true, waypointColor, true, speedColor, false, Color.white, false, Color.white);
                }
            }
        }


        internal void DrawCurrentWaypoint(WaypointSettingsBase waypoint, Color selectedColor, Color waypointColor, Color otherLaneColor, Color prevColor)
        {
            DrawCompleteWaypoint(waypoint, selectedColor, true, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, true, prevColor);
            for (int i = 0; i < waypoint.neighbors.Count; i++)
            {
                if (waypoint.neighbors[i] != null)
                {
                    DrawCompleteWaypoint(waypoint.neighbors[i], waypointColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
            for (int i = 0; i < waypoint.prev.Count; i++)
            {
                if (waypoint.prev[i] != null)
                {
                    DrawCompleteWaypoint(waypoint.prev[i], prevColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
            for (int i = 0; i < waypoint.otherLanes.Count; i++)
            {
                if (waypoint.otherLanes != null)
                {
                    DrawCompleteWaypoint(waypoint.otherLanes[i], otherLaneColor, false, waypointColor, false, Color.white, false, Color.white, true, otherLaneColor, false, prevColor);
                }
            }
        }


        internal void DrawSelectedWaypoint(WaypointSettingsBase selectedWaypoint, Color color)
        {
            Handles.color = color;
            Handles.CubeHandleCap(0, selectedWaypoint.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 1, EventType.Repaint);
        }


        protected virtual void LoadWaypoints()
        {
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                allWaypoints = FindObjectsOfType<T>().ToList();
            }
            else
            {
                allWaypoints = GleyPrefabUtilities.GetScenePrefabRoot().GetComponentsInChildren<T>().ToList();
            }
            UpdateDisconnectedWaypoints();
            UpdateStopWaypoints();
        }


        protected void DrawClickableButton(T waypoint, Color color)
        {
            if (!waypoint)
                return;
            Handles.color = color;
            if (Handles.Button(waypoint.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
            {
                TriggetWaypointClickedEvent(waypoint, Event.current.button == 0);
            }

        }


        protected void DrawWaypointConnections(WaypointSettingsBase waypoint, Color color, bool drawOtherLanes, Color otherLanesColor, bool drawPrev, Color prevColor)
        {
            if (!waypoint)
                return;
            Handles.color = color;
            if (waypoint.neighbors.Count > 0)
            {
                for (int i = 0; i < waypoint.neighbors.Count; i++)
                {
                    if (waypoint.neighbors[i] != null)
                    {
                        Handles.DrawLine(waypoint.transform.position, waypoint.neighbors[i].transform.position);

                        Vector3 direction = (waypoint.transform.position - waypoint.neighbors[i].transform.position).normalized;
                        Vector3 point1 = (waypoint.transform.position + waypoint.neighbors[i].transform.position) / 2;

                        Vector3 point2 = point1 + Quaternion.Euler(0, -25, 0) * direction;

                        Vector3 point3 = point1 + Quaternion.Euler(0, 25, 0) * direction;

                        Handles.DrawPolyLine(point1, point2, point3, point1);
                    }
                    else
                    {
                        Debug.LogWarning("waypoint " + waypoint.name + " has missing neighbors", waypoint);
                    }
                }
            }

            if (drawOtherLanes)
            {
                if (waypoint.otherLanes != null)
                {
                    for (int i = 0; i < waypoint.otherLanes.Count; i++)
                    {
                        if (waypoint.otherLanes[i] != null)
                        {
                            DrawTriangle(waypoint.transform.position, waypoint.otherLanes[i].transform.position, otherLanesColor, true);
                        }
                    }
                }
            }

            if (drawPrev)
            {
                Handles.color = prevColor;
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    if (waypoint.prev[i] != null)
                    {
                        Handles.DrawLine(waypoint.transform.position, waypoint.prev[i].transform.position);
                    }
                }
            }
        }


        private void UpdateDisconnectedWaypoints()
        {
            disconnectedWaypoints = new List<WaypointSettingsBase>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].neighbors != null)
                {
                    if (allWaypoints[i].neighbors.Count == 0)
                    {
                        disconnectedWaypoints.Add(allWaypoints[i]);
                    }
                    else
                    {
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            if (allWaypoints[i].neighbors[j] == null)
                            {
                                allWaypoints[i].neighbors.RemoveAt(j);
                                disconnectedWaypoints.Add(allWaypoints[i]);
                            }
                        }
                    }
                }
                else
                {
                    allWaypoints[i].neighbors = new List<WaypointSettingsBase>();
                    disconnectedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateStopWaypoints()
        {
            stopWaypoints = new List<WaypointSettingsBase>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].stop == true)
                {
                    stopWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void DrawTriangle(Vector3 start, Vector3 end, Color waypointColor, bool drawLane)
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
    }
}
