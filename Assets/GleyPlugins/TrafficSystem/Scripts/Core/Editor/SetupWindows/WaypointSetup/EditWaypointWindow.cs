using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class EditWaypointWindow : EditWaypointWindowBase<WaypointSettings>
    {
        private int maxSpeed;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            maxSpeed = selectedWaypoint.maxSpeed;

            return this;
        }


        protected override void SetCars()
        {
            List<VehicleTypes> result = new List<VehicleTypes>();
            for (int i = 0; i < carDisplay.Length; i++)
            {
                if (carDisplay[i].active)
                {
                    result.Add((VehicleTypes)carDisplay[i].car);
                }
            }
            selectedWaypoint.allowedCars = result;
            if (result.Count > 0)
            {
                selectedWaypoint.carsLocked = true;
            }
            else
            {
                selectedWaypoint.carsLocked = false;
            }
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            SetCarType(waypointList, selectedWaypoint.allowedCars, selectedWaypoint.neighbors);
        }


        private void SetCarType(List<WaypointSettings> waypointList, List<VehicleTypes> carTypes, List<WaypointSettingsBase> neighbors)
        {
            if (carTypes == null || carTypes.Count == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                WaypointSettings neighbor = (WaypointSettings)neighbors[i];
                if (!waypointList.Contains(neighbor))
                {
                    if (!neighbor.carsLocked)
                    {
                        waypointList.Add(neighbor);
                        neighbor.allowedCars = carTypes;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetCarType(waypointList, carTypes, neighbors[i].neighbors);
                    }
                }
            }
        }


        internal override void DrawCarSettings()
        {
            selectedWaypoint.giveWay = EditorGUILayout.Toggle(new GUIContent("Give Way", "Vehicle will stop when reaching this waypoint and check if next waypoint is free before continuing"), selectedWaypoint.giveWay);
            maxSpeed = EditorGUILayout.IntField(new GUIContent("Max speed", "The maximum speed allowed in this waypoint"), maxSpeed);
            if (GUILayout.Button("Set Speed"))
            {
                if (maxSpeed != 0)
                {
                    selectedWaypoint.speedLocked = true;
                }
                else
                {
                    selectedWaypoint.speedLocked = false;
                }
                SetSpeed();
            }
        }


        protected override CarDisplay[] SetCarDisplay()
        {
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            CarDisplay[] carDisplay = new CarDisplay[nrOfCars];
            for (int i = 0; i < nrOfCars; i++)
            {
                carDisplay[i] = new CarDisplay(selectedWaypoint.allowedCars.Contains((VehicleTypes)i), i, Color.white);
            }
            return carDisplay;
        }


        internal override string SetLabels(int i)
        {
            return ((VehicleTypes)i).ToString();
        }


        internal override WaypointSettings SetSelectedWaypoint()
        {
            return SettingsWindow.GetSelectedWaypoint();
        }


        internal override SettingsLoader SetSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        internal override WaypointDrawerBase<WaypointSettings> SetWaypointsDrawer()
        {
            return CreateInstance<WaypointDrawer>();
        }


        internal override void ViewWaypoint(WaypointSettingsBase waypoint)
        {
            clickedWaypoint = (WaypointSettings)waypoint;
            GleyUtilities.TeleportSceneCamera(waypoint.transform.position);
        }


        private void SetSpeed()
        {
            List<WaypointSettings> waypointList = new List<WaypointSettings>();
            selectedWaypoint.maxSpeed = maxSpeed;
            SetSpeed(waypointList, selectedWaypoint.maxSpeed, selectedWaypoint.neighbors.Cast<WaypointSettings>().ToList());
        }


        private void SetSpeed(List<WaypointSettings> waypointList, int speed, List<WaypointSettings> neighbors)
        {
            if (speed == 0)
            {
                return;
            }

            for (int i = 0; i < neighbors.Count; i++)
            {
                if (!waypointList.Contains(neighbors[i]))
                {
                    if (!neighbors[i].speedLocked)
                    {
                        waypointList.Add(neighbors[i]);
                        neighbors[i].maxSpeed = speed;
                        EditorUtility.SetDirty(neighbors[i]);
                        SetSpeed(waypointList, speed, neighbors[i].neighbors.Cast<WaypointSettings>().ToList());
                    }
                }
            }
        }


        protected override void OpenEditWaypointWindow()
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), false);
        }


        protected override GUIContent SetAllowedAgentsText()
        {
            return new GUIContent("Allowed vehicles: ", "Only the following vehicles can pass through this waypoint");
        }


        protected override void ShowOtherLanes()
        {
            EditorGUILayout.Space();
            MakeListOperations("Other Lanes", "Connections to other lanes, used for overtaking", selectedWaypoint.otherLanes, ListToAdd.OtherLanes);
        }
    }
}
