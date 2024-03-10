using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class ConnectRoadsWindowBase : SetupWindowBase
    {
        const float minValue = 246;
        const float maxValue = 372;

        protected ConnectRoadsSave save;
        protected SettingsLoader settingsLoader;
        protected int nrOfCars;

        private List<RoadBase> allRoads;
        protected bool[] allowedCarIndex;
        private RoadDrawer roadDrawer;
        private RoadConnectionsBase roadConnections;    
        private RoadColors roadColors;
        private float scrollAdjustment;
        private bool drawAllConnections;
        private bool showCustomizations;

        protected abstract SettingsLoader LoadSettingsLoader();
        protected abstract RoadConnectionsBase LoadRoadConnections();
        protected abstract List<RoadBase> LoadAllRoads();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            settingsLoader = LoadSettingsLoader();
            save = settingsLoader.LoadConnectRoadsSave();
            roadColors = settingsLoader.LoadRoadColors();
            roadDrawer = RoadDrawer.Initialize();
            allRoads = LoadAllRoads();
            roadConnections = LoadRoadConnections();
            return this;
        }


        public override void DrawInScene()
        {
            if (GleyUtilities.SceneCameraMoved())
            {
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }

            MakeClickConnection(allRoads, roadConnections.ConnectionPools, roadColors.connectorLaneColor, roadColors.anchorPointColor,
            roadColors.roadConnectorColor, roadColors.selectedRoadConnectorColor, roadColors.disconnectedColor, save.waypointDistance, roadColors.textColor);
          

            for (int i = 0; i < allRoads.Count; i++)
            {
                roadDrawer.DrawPath(allRoads[i], MoveTools.None, roadColors.roadColor, roadColors.anchorPointColor, roadColors.controlPointColor, roadColors.textColor);
                LaneDrawer.DrawAllLanes(allRoads[i], save.viewRoadsSettings.viewWaypoints, save.viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                    roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
            }

            base.DrawInScene();
        }


        protected override void TopPart()
        {
            base.TopPart();
            string drawButton = "Draw All Connections";
            if (drawAllConnections == true)
            {
                drawButton = "Clear All";
            }

            if (GUILayout.Button(drawButton))
            {
                drawAllConnections = !drawAllConnections;
                for (int i = 0; i < roadConnections.ConnectionPools.Count; i++)
                {
                    for (int j = 0; j < roadConnections.ConnectionPools[i].GetNrOfConnections(); j++)
                    {
                        roadConnections.ConnectionPools[i].connectionCurves[j].draw = drawAllConnections;
                        if (drawAllConnections == false)
                        {
                            roadConnections.ConnectionPools[i].connectionCurves[j].drawWaypoints = false;
                        }
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            if (showCustomizations == false)
            {
                scrollAdjustment = minValue;
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
                save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints);
                save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges);

            }
            else
            {
                scrollAdjustment = maxValue;
                showCustomizations = EditorGUILayout.Toggle("Change Colors ", showCustomizations);
                EditorGUILayout.BeginHorizontal();
                save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                roadColors.waypointColor = EditorGUILayout.ColorField(roadColors.waypointColor);
                roadColors.disconnectedColor = EditorGUILayout.ColorField(roadColors.disconnectedColor);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
                EditorGUILayout.EndHorizontal();

                roadColors.textColor = EditorGUILayout.ColorField("Text Color", roadColors.textColor);
                roadColors.roadColor = EditorGUILayout.ColorField("Road Color", roadColors.roadColor);
                roadColors.laneColor = EditorGUILayout.ColorField("Lane Color", roadColors.laneColor);
                roadColors.connectorLaneColor = EditorGUILayout.ColorField("Connector Lane Color", roadColors.connectorLaneColor);
                roadColors.anchorPointColor = EditorGUILayout.ColorField("Anchor Point Color", roadColors.anchorPointColor);
                roadColors.roadConnectorColor = EditorGUILayout.ColorField("Road Connector Color", roadColors.roadConnectorColor);
                roadColors.selectedRoadConnectorColor = EditorGUILayout.ColorField("Selected Connector Color", roadColors.selectedRoadConnectorColor);
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }


        protected override void ScrollPart(float width, float height)
        {
            base.ScrollPart(width, height);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            for (int i = 0; i < roadConnections.ConnectionPools.Count; i++)
            {
                for (int j = 0; j < roadConnections.ConnectionPools[i].GetNrOfConnections(); j++)
                {
                    if (GleyUtilities.IsPointInsideView(roadConnections.ConnectionPools[i].GetInConnector(j).transform.position) ||
                       GleyUtilities.IsPointInsideView(roadConnections.ConnectionPools[i].GetOutConnector<WaypointSettingsBase>(j).transform.position))
                    {
                        EditorGUILayout.BeginHorizontal();

                        roadConnections.ConnectionPools[i].connectionCurves[j].draw = EditorGUILayout.Toggle(roadConnections.ConnectionPools[i].connectionCurves[j].draw, GUILayout.Width(TOGGLE_DIMENSION));
                        EditorGUILayout.LabelField(roadConnections.ConnectionPools[i].GetName(j));
                        Color oldColor = GUI.backgroundColor;
                        if (roadConnections.ConnectionPools[i].connectionCurves[j].drawWaypoints == true)
                        {
                            GUI.backgroundColor = Color.green;
                        }

                        if (GUILayout.Button("Waypoints", GUILayout.Width(BUTTON_DIMENSION)))
                        {
                            roadConnections.ConnectionPools[i].connectionCurves[j].drawWaypoints = !roadConnections.ConnectionPools[i].connectionCurves[j].drawWaypoints;
                        }
                        GUI.backgroundColor = oldColor;

                        if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
                        {
                            GleyUtilities.TeleportSceneCamera(roadConnections.ConnectionPools[i].GetOutConnector<WaypointSettingsBase>(j).gameObject.transform.position);
                            SceneView.RepaintAll();
                        }

                        if (GUILayout.Button("Delete", GUILayout.Width(BUTTON_DIMENSION)))
                        {
                            if (EditorUtility.DisplayDialog("Delete " + roadConnections.ConnectionPools[i].connectionCurves[j].name + "?", "Are you sure you want to delete " + roadConnections.ConnectionPools[i].connectionCurves[j].name + "? \nYou cannot undo this operation.", "Delete", "Cancel"))
                            {
                                roadConnections.DeleteConnection(roadConnections.ConnectionPools[i].connectionCurves[j]);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
        }


        protected override void BottomPart()
        {
            save.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", save.waypointDistance);
            if (GUILayout.Button("Generate Selected Connections"))
            {
                GenerateSelectedConnections();
            }
            base.BottomPart();
        }


        protected virtual void GenerateSelectedConnections()
        {
            SceneView.RepaintAll();
        }


        protected virtual void MakeClickConnection(List<RoadBase> allRoads, List<ConnectionPool> allConnections, Color connectorLaneColor, Color anchorPointColor, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance, Color textColor)
        {
            
        }


        public override void DestroyWindow()
        {
            settingsLoader.SaveConnectRoadsSettings(save, roadColors);
            base.DestroyWindow();
        }
    }
}
