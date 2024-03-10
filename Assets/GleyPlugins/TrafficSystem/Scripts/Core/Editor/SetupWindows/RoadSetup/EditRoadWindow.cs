using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class EditRoadWindow : EditRoadWindowBase
    {
        EditRoadSave save;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            selectedRoad = SettingsWindow.GetSelectedRoad();
            settingsLoader = new SettingsLoader(Constants.windowSettingsPath);
            save = settingsLoader.LoadEditRoadSave();
            moveTool = save.moveTool;
            viewRoadsSettings = save.viewRoadsSettings;
            roadColors = settingsLoader.LoadRoadColors();
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            allowedCarIndex = new bool[nrOfCars];
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                if (save.globalCarList.Contains((VehicleTypes)i))
                {
                    allowedCarIndex[i] = true;
                }
            }
            return this;
        }


        protected override void UpdateLaneNumber()
        {
            selectedRoad.UpdateLaneNumber(save.maxSpeed, System.Enum.GetValues(typeof(VehicleTypes)).Length);
        }


        protected override void GenerateWaypoints()
        {
            CreateInstance<WaypointGeneratorTraffic>().GenerateWaypoints(selectedRoad, window.GetGroundLayer());
        }


        protected override void ShowSpeedSetup()
        {
            EditorGUILayout.BeginHorizontal();
            save.maxSpeed = EditorGUILayout.IntField("Global Max Speed", save.maxSpeed);
            if (GUILayout.Button("Apply Speed"))
            {
                SetSpeedOnLanes(selectedRoad, save.maxSpeed);
            }
            EditorGUILayout.EndHorizontal();
        }


        private void SetSpeedOnLanes(RoadBase selectedRoad, int maxSpeed)
        {
            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                selectedRoad.lanes[i].laneSpeed = maxSpeed;
            }
        }


        protected override void ApplyGlobalCarSettings()
        {
            SetSpeedOnLanes(selectedRoad, save.maxSpeed);
            base.ApplyGlobalCarSettings();
        }


        protected override void DrawLinkOtherLanes()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Link other lanes"))
            {
                viewRoadsSettings.viewWaypoints = true;
                viewRoadsSettings.viewLaneChanges = true;
                LinkOtherLanes.Link((Road)selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button("Unlink other lanes"))
            {
                LinkOtherLanes.Unlinck((Road)selectedRoad);
                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();
        }


        public override void DestroyWindow()
        {
            save.moveTool = moveTool;
            save.viewRoadsSettings = viewRoadsSettings;
            settingsLoader.SaveEditRoadSettings(save, allowedCarIndex, roadColors, new RoadDefaults(selectedRoad.nrOfLanes, selectedRoad.laneWidth, selectedRoad.waypointDistance));
            base.DestroyWindow();
        }


        protected override void SelectAllowedCars()
        {
            GUILayout.Label("Allowed Car Types:");
            for (int i = 0; i < nrOfCars; i++)
            {
                allowedCarIndex[i] = EditorGUILayout.Toggle(((VehicleTypes)i).ToString(), allowedCarIndex[i]);
            }
            base.SelectAllowedCars();
        }


        protected override void ToggleAgentTypes(int currentLane, int agentIndex)
        {
            selectedRoad.lanes[currentLane].allowedCars[agentIndex] = EditorGUILayout.Toggle(((VehicleTypes)agentIndex).ToString(), selectedRoad.lanes[currentLane].allowedCars[agentIndex]);
        }


        protected override void SetTopText()
        {
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
        }


        protected override void SetTexts()
        {
            agentName = "Car";
        }


        protected override void DrawSpeedProperties(int currentLane)
        {
            GUILayout.BeginHorizontal();
            selectedRoad.lanes[currentLane].laneSpeed = EditorGUILayout.IntField("Lane " + currentLane + ", Lane Speed:", selectedRoad.lanes[currentLane].laneSpeed);
            string buttonLebel = "<--";
            if (selectedRoad.lanes[currentLane].laneDirection == false)
            {
                buttonLebel = "-->";
            }
            if (GUILayout.Button(buttonLebel))
            {
                selectedRoad.lanes[currentLane].laneDirection = !selectedRoad.lanes[currentLane].laneDirection;
                WaypointsGenerator.SwitchLaneDirection(selectedRoad, currentLane);
            }
            GUILayout.EndHorizontal();
        }
    }
}
