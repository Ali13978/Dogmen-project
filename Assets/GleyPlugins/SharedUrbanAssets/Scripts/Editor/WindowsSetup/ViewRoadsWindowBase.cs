using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class ViewRoadsWindowBase : SetupWindowBase
    {
        const float minValue = 184;
        const float maxValue = 220;
        const float minValueColor = 220;
        const float maxValueColor = 256;

        protected string drawAllRoadsText;

        private List<RoadBase> allRoads;
        private RoadDrawer roadDrawer;
        private ViewRoadsSave save;
        private RoadColors roadColors;
        private float scrollAdjustment;
        private bool drawAllRoads;
        private bool showCustomizations;
        private SettingsLoader settingsLoader;

        protected abstract void SetTexts();
        protected abstract SettingsLoader LoadSettingsLoader();
        protected abstract List<RoadBase> LoadAllRoads();
        protected abstract void DeleteCurrentRoad(RoadBase road);
        protected abstract void SelectWaypoint(RoadBase road);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            roadDrawer = RoadDrawer.Initialize();
            settingsLoader = LoadSettingsLoader();
            save = settingsLoader.LoadViewRoadsSave();
            roadColors = settingsLoader.LoadRoadColors();
            scrollAdjustment = minValue;
            allRoads = LoadAllRoads();
            SetTexts();

            return this;
        }


        public override void DrawInScene()
        {
            base.DrawInScene();

            if (GleyUtilities.SceneCameraMoved())
            {
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }

            for (int i = 0; i < allRoads.Count; i++)
            {
                if (allRoads[i].draw)
                {
                    roadDrawer.DrawPath(allRoads[i], MoveTools.None, roadColors.roadColor, roadColors.anchorPointColor, roadColors.controlPointColor, roadColors.textColor);
                    if (save.viewRoadsSettings.viewLanes)
                    {
                        LaneDrawer.DrawAllLanes(allRoads[i], save.viewRoadsSettings.viewWaypoints, save.viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                            roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
                    }
                }
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            string drawButton = drawAllRoadsText;
            if (drawAllRoads == true)
            {
                drawButton = "Clear All";
            }
            if (GUILayout.Button(drawButton))
            {
                drawAllRoads = !drawAllRoads;
                for (int i = 0; i < allRoads.Count; i++)
                {
                    allRoads[i].draw = drawAllRoads;
                }
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            if (showCustomizations == false)
            {
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
                save.viewRoadsSettings.viewLanes = EditorGUILayout.Toggle("View Lanes", save.viewRoadsSettings.viewLanes);
                if (save.viewRoadsSettings.viewLanes)
                {
                    scrollAdjustment = maxValue;
                    save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints);
                    save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges);
                }
                else
                {
                    scrollAdjustment = minValue;
                }
            }
            else
            {
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);


                EditorGUILayout.BeginHorizontal();
                save.viewRoadsSettings.viewLanes = EditorGUILayout.Toggle("View Lanes", save.viewRoadsSettings.viewLanes);
                roadColors.laneColor = EditorGUILayout.ColorField(roadColors.laneColor);
                EditorGUILayout.EndHorizontal();

                if (save.viewRoadsSettings.viewLanes)
                {
                    scrollAdjustment = maxValueColor;
                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                    roadColors.waypointColor = EditorGUILayout.ColorField(roadColors.waypointColor);
                    roadColors.disconnectedColor = EditorGUILayout.ColorField(roadColors.disconnectedColor);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                    roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    scrollAdjustment = minValueColor;
                }
                roadColors.textColor = EditorGUILayout.ColorField("Text Color ", roadColors.textColor);
                roadColors.roadColor = EditorGUILayout.ColorField("Road Color", roadColors.roadColor);
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
            EditorGUILayout.Space();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            for (int i = 0; i < allRoads.Count; i++)
            {
                MakeSelectRoadRow(allRoads[i]);
            }
            GUILayout.EndScrollView();
        }


        private void MakeSelectRoadRow(RoadBase road)
        {
            if (road.isInsidePrefab && !GleyPrefabUtilities.EditingInsidePrefab())
                return;
            if (GleyUtilities.IsPointInsideView(road.path[0]) || GleyUtilities.IsPointInsideView(road.path[road.path.NumPoints - 1]))
            {
                EditorGUILayout.BeginHorizontal();
                road.draw = EditorGUILayout.Toggle(road.draw, GUILayout.Width(TOGGLE_DIMENSION));
                GUILayout.Label(road.gameObject.name);



                if (GUILayout.Button("View"))
                {
                    GleyUtilities.TeleportSceneCamera(road.transform.position);
                    SceneView.RepaintAll();
                }
                if (GUILayout.Button("Select"))
                {
                    SelectWaypoint(road);
                }
                if (GUILayout.Button("Delete"))
                {
                    EditorGUI.BeginChangeCheck();
                    if (EditorUtility.DisplayDialog("Delete " + road.name + "?", "Are you sure you want to delete " + road.name + "? \nYou cannot undo this operation.", "Delete", "Cancel"))
                    {
                        DeleteCurrentRoad(road);
                    }
                    EditorGUI.EndChangeCheck();
                }

                if (GUI.changed)
                {
                    SceneView.RepaintAll();
                }

                EditorGUILayout.EndHorizontal();
            }
        }


        protected void DeletRoad<T>(RoadBase road) where T : RoadConnectionsBase
        {
            allRoads.Remove(road);
            T roadConnections = CreateInstance<T>();
            roadConnections.Initialize();
            for (int i = 0; i < roadConnections.ConnectionPools.Count; i++)
            {
                List<ConnectionCurve> curve = roadConnections.ConnectionPools[i].ContainsRoad(road);
                for (int j = 0; j < curve.Count; j++)
                {
                    roadConnections.DeleteConnection(curve[j]);
                }
            }
            Undo.DestroyObjectImmediate(road.gameObject);
            Undo.undoRedoPerformed += UndoPerformed;
        }


        private void UndoPerformed()
        {
            allRoads = LoadAllRoads();
        }


        public override void DestroyWindow()
        {
            Undo.undoRedoPerformed -= UndoPerformed;
            settingsLoader.SaveViewRoadsSettings(save, roadColors);
            base.DestroyWindow();
        }
    }
}
