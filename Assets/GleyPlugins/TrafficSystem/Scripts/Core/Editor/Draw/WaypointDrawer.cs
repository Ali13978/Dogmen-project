
using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointDrawer : WaypointDrawerBase<WaypointSettings>
    {
        private List<WaypointSettings> speedEditedWaypoints;
        private List<WaypointSettings> giveWayWaypoints;
        private List<WaypointSettings> pathProblems;
        private List<WaypointSettings> carTypeEditedWaypoints;


        protected override void TriggetWaypointClickedEvent(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetSelectedWaypoint(clickedWaypoint);
            base.TriggetWaypointClickedEvent(clickedWaypoint, leftClick);
        }


        protected override void LoadWaypoints()
        {
            base.LoadWaypoints();
            UpdateCarTypeEditedWaypoints();
            UpdateSpeedEditedWaypoints();
            UpdateGiveWayWaypoints();
        }


        internal List<WaypointSettings> ShowSpeedEditedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].speedLocked)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }
            if (nr != speedEditedWaypoints.Count)
            {
                UpdateSpeedEditedWaypoints();
            }

            return speedEditedWaypoints;
        }


        private void UpdateSpeedEditedWaypoints()
        {
            speedEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].speedLocked == true)
                {
                    speedEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        internal List<WaypointSettings> ShowGiveWayWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].giveWay)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }

            if (nr != giveWayWaypoints.Count)
            {
                UpdateGiveWayWaypoints();
            }

            return giveWayWaypoints;
        }


        internal List<WaypointSettings> ShowVehicleProblems(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            pathProblems = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    int nr = allWaypoints[i].allowedCars.Count;
                    for (int j = 0; j < allWaypoints[i].allowedCars.Count; j++)
                    {
                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            if (((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Contains(allWaypoints[i].allowedCars[j]))
                            {
                                nr--;
                                break;
                            }
                        }
                    }
                    if (nr != 0)
                    {
                        pathProblems.Add(allWaypoints[i]);
                        DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, true, carsColor, drawOtherLanes, otherLanesColor);

                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            for (int j = 0; j < ((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Count; j++)
                            {
                                DrawCompleteWaypoint(allWaypoints[i].neighbors[k], connectionColor, showConnections, connectionColor, showSpeed, speedColor, true, carsColor, drawOtherLanes, otherLanesColor);
                            }
                        }
                    }
                }

            }
            return pathProblems;
        }


        internal List<int> GetDifferentSpeeds()
        {
            List<int> result = new List<int>();
            LoadWaypoints();

            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (!result.Contains(allWaypoints[i].maxSpeed))
                {
                    result.Add(allWaypoints[i].maxSpeed);
                }
            }
            return result;
        }


        internal void ShowSpeedLimits(int speed, Color color)
        {
            if (color.a == 0)
            {
                color = Color.white;
            }
            Handles.color = color;
            if (allWaypoints == null)
            {
                LoadWaypoints();
            }

            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].maxSpeed == speed)
                    {
                        if (Handles.Button(allWaypoints[i].transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
                        {
                            TriggetWaypointClickedEvent(allWaypoints[i], Event.current.button == 0);
                        }
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            Handles.DrawLine(allWaypoints[i].transform.position, allWaypoints[i].neighbors[j].transform.position);
                        }
                    }
                }
            }
        }


        internal override void DrawWaypointsForCar(int car, Color color)
        {
            Handles.color = color;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (GleyUtilities.IsPointInsideView(allWaypoints[i].transform.position))
                {
                    if (allWaypoints[i].allowedCars.Contains((VehicleTypes)car))
                    {
                        if (Handles.Button(allWaypoints[i].transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 0.5f, 0.5f, Handles.DotHandleCap))
                        {
                            TriggetWaypointClickedEvent(allWaypoints[i], Event.current.button == 0);
                        }
                        for (int j = 0; j < allWaypoints[i].neighbors.Count; j++)
                        {
                            Handles.DrawLine(allWaypoints[i].transform.position, allWaypoints[i].neighbors[j].transform.position);
                        }
                    }
                }
            }
        }


        internal List<WaypointSettings> ShowCarTypeEditedWaypoints(Color waypointColor, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor)
        {
            int nr = 0;
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].carsLocked)
                {
                    nr++;
                    DrawCompleteWaypoint(allWaypoints[i], waypointColor, showConnections, connectionColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor);
                }
            }

            if (nr != carTypeEditedWaypoints.Count)
            {
                UpdateCarTypeEditedWaypoints();
            }

            return carTypeEditedWaypoints;
        }


        private void UpdateCarTypeEditedWaypoints()
        {
            carTypeEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].carsLocked == true)
                {
                    carTypeEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateGiveWayWaypoints()
        {
            giveWayWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                if (allWaypoints[i].giveWay == true)
                {
                    giveWayWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        protected override void DrawCompleteWaypoint(WaypointSettingsBase waypoint, Color color, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool drawPrev = false, Color prevColor = new Color())
        {
            if (!waypoint)
                return;
            if (GleyUtilities.IsPointInsideView(waypoint.transform.position))
            {
                DrawClickableButton((WaypointSettings)waypoint, color);
                if (showConnections)
                {
                    DrawWaypointConnections(waypoint, connectionColor, drawOtherLanes, otherLanesColor, drawPrev, prevColor);
                }
                if (showSpeed)
                {
                    ShowSpeed((WaypointSettings)waypoint, speedColor);
                }
                if (showCars)
                {
                    ShowCars((WaypointSettings)waypoint, carsColor);
                }
            }
        }


        private void ShowSpeed(WaypointSettings waypoint, Color color)
        {
            if (!waypoint)
                return;
            style.normal.textColor = color;
            Handles.Label(waypoint.transform.position, waypoint.maxSpeed.ToString(), style);
        }


        private void ShowCars(WaypointSettings waypoint, Color color)
        {
            if (!waypoint)
                return;
            style.normal.textColor = color;
            string text = "";
            for (int j = 0; j < waypoint.allowedCars.Count; j++)
            {
                text += waypoint.allowedCars[j] + "\n";
            }
            Handles.Label(waypoint.transform.position, text, style);
        }
    }
}
