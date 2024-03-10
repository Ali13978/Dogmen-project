using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class EditRoadWindowBase : SetupWindowBase
    {
        const float minValue = 323;
        const float maxValue = 449;

        protected bool[] allowedCarIndex;
        protected RoadBase selectedRoad;   
        protected ViewRoadsSettings viewRoadsSettings;
        protected RoadColors roadColors;
        protected SettingsLoader settingsLoader;
        protected MoveTools moveTool;
        protected string agentName;
        protected int nrOfCars;

        private RoadDrawer roadDrawer;
        private float scrollAdjustment;
        private bool showCustomizations;

        protected abstract void SetTexts();
        protected abstract void SetTopText();
        protected abstract void UpdateLaneNumber();
        protected abstract void GenerateWaypoints();
        protected abstract void DrawSpeedProperties(int currentLane);
        protected abstract void ToggleAgentTypes(int currentLane, int agentIndex);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            roadDrawer = RoadDrawer.Initialize();
            SetTexts();
            return this;
        }


        public override void DrawInScene()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.DrawPath(selectedRoad, moveTool, roadColors.roadColor, roadColors.anchorPointColor, roadColors.controlPointColor, roadColors.textColor);
            LaneDrawer.DrawAllLanes(selectedRoad, viewRoadsSettings.viewWaypoints, viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
            base.DrawInScene();
        }


        public override void MouseMove(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            base.MouseMove(mousePosition);
            roadDrawer.SelectSegmentIndex(selectedRoad, mousePosition);
        }


        public override void LeftClick(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            roadDrawer.AddPathPoint(mousePosition, selectedRoad);
            base.LeftClick(mousePosition);
        }


        public override void RightClick(Vector3 mousePosition)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            roadDrawer.Delete(selectedRoad, mousePosition);
            base.RightClick(mousePosition);
        }



        protected override void TopPart()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SetTopText();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", viewRoadsSettings.viewWaypoints);
            viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", viewRoadsSettings.viewLaneChanges);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            selectedRoad.nrOfLanes = EditorGUILayout.IntField("Nr of lanes", selectedRoad.nrOfLanes);
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                UpdateLaneNumber();
            }

            selectedRoad.laneWidth = EditorGUILayout.FloatField("Lane width (m)", selectedRoad.laneWidth);
            selectedRoad.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", selectedRoad.waypointDistance);

            EditorGUI.BeginChangeCheck();
            moveTool = (MoveTools)EditorGUILayout.EnumPopup("Select move tool ", moveTool);

            showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
            if (showCustomizations == true)
            {
                scrollAdjustment = maxValue;
                roadColors.roadColor = EditorGUILayout.ColorField("Road Color", roadColors.roadColor);
                roadColors.laneColor = EditorGUILayout.ColorField("Lane Color", roadColors.laneColor);
                roadColors.waypointColor = EditorGUILayout.ColorField("Waypoint Color", roadColors.waypointColor);
                roadColors.disconnectedColor = EditorGUILayout.ColorField("Disconnected Color", roadColors.disconnectedColor);
                roadColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color", roadColors.laneChangeColor);
                roadColors.controlPointColor = EditorGUILayout.ColorField("Control Point Color", roadColors.controlPointColor);
                roadColors.anchorPointColor = EditorGUILayout.ColorField("Anchor Point Color", roadColors.anchorPointColor);
            }
            else
            {
                scrollAdjustment = minValue;
            }
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.TopPart();
        }



        protected override void ScrollPart(float width, float height)
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Global Lane Settings", EditorStyles.boldLabel);
            SelectAllowedCars();

            ShowSpeedSetup();

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply All Settings"))
            {

                ApplyGlobalCarSettings();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Individual Lane Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            LoadLanes();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            GUILayout.EndScrollView();

            base.ScrollPart(width, height);
        }


        protected virtual void ShowSpeedSetup()
        {

        }


        protected override void BottomPart()
        {
            if (selectedRoad == null)
            {
                Debug.LogWarning("No road selected");
                return;
            }

            if (GUILayout.Button("Generate waypoints"))
            {
                viewRoadsSettings.viewWaypoints = true;

                if (selectedRoad.nrOfLanes <= 0)
                {
                    Debug.LogError("Nr of lanes has to be >0");
                    return;
                }

                if (selectedRoad.waypointDistance <= 0)
                {
                    Debug.LogError("Waypoint distance needs to be >0");
                    return;
                }

                if (selectedRoad.laneWidth <= 0)
                {
                    Debug.LogError("Lane width has to be >0");
                    return;
                }
                GenerateWaypoints();

                EditorUtility.SetDirty(selectedRoad);
                AssetDatabase.SaveAssets();
            }

            DrawLinkOtherLanes();

            base.BottomPart();
        }


        protected virtual void DrawLinkOtherLanes()
        {

        }


        public override void DestroyWindow()
        {
            base.DestroyWindow();
        }


        protected virtual void SelectAllowedCars()
        {

            if (GUILayout.Button("Apply Global " + agentName + " Settings"))
            {
                ApplyGlobalCarSettings();
            }
        }


        protected virtual void ApplyGlobalCarSettings()
        {

            for (int i = 0; i < selectedRoad.lanes.Count; i++)
            {
                for (int j = 0; j < allowedCarIndex.Length; j++)
                {
                    selectedRoad.lanes[i].allowedCars[j] = allowedCarIndex[j];
                }
            }
        }


        private void LoadLanes()
        {
            if (selectedRoad)
            {
                if (selectedRoad.lanes != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    for (int i = 0; i < selectedRoad.lanes.Count; i++)
                    {
                        if (selectedRoad.lanes[i].laneDirection == true)
                        {
                            DrawLaneButton(i);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    for (int i = 0; i < selectedRoad.lanes.Count; i++)
                    {
                        if (selectedRoad.lanes[i].laneDirection == false)
                        {
                            DrawLaneButton(i);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }


        private void DrawLaneButton(int currentLane)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawSpeedProperties(currentLane);

            EditorGUILayout.LabelField("Allowed " + agentName + " types on this lane:");
            for (int i = 0; i < nrOfCars; i++)
            {
                if (i >= selectedRoad.lanes[currentLane].allowedCars.Length)
                {
                    selectedRoad.lanes[currentLane].UpdateAllowedCars(nrOfCars);
                }
                ToggleAgentTypes(currentLane, i);
            }
            EditorGUILayout.EndVertical();
        }
    }
}