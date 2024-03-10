using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class NewRoadWindowBase : SetupWindowBase
    {
        protected SettingsLoader settingsLoader;
        protected Vector3 firstClick;
        protected Vector3 secondClick;

        private CreateRoadSave save;
        private RoadColors roadColors;
        private RoadDrawer roadDrawer;
        private List<RoadBase> allRoads;

        protected abstract SettingsLoader LoadSettingsLoader();
        protected abstract List<RoadBase> LoadAllRoads();
        protected abstract void SetTopText();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            firstClick = secondClick = Vector3.zero;
            settingsLoader = LoadSettingsLoader();
            save = settingsLoader.LoadCreateRoadSave();
            roadColors = settingsLoader.LoadRoadColors();
            roadDrawer = RoadDrawer.Initialize();
            allRoads = LoadAllRoads();

            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            SetTopText();
        }


        protected override void ScrollPart(float width, float height)
        {
            EditorGUI.BeginChangeCheck();
            roadColors.textColor = EditorGUILayout.ColorField("Text Color", roadColors.textColor);

            EditorGUILayout.BeginHorizontal();
            save.viewOtherRoads = EditorGUILayout.Toggle("View Other Roads", save.viewOtherRoads, GUILayout.Width(TOGGLE_WIDTH));
            roadColors.roadColor = EditorGUILayout.ColorField(roadColors.roadColor);
            EditorGUILayout.EndHorizontal();

            if (save.viewOtherRoads)
            {
                EditorGUILayout.BeginHorizontal();
                save.viewRoadsSettings.viewLanes = EditorGUILayout.Toggle("View Lanes", save.viewRoadsSettings.viewLanes, GUILayout.Width(TOGGLE_WIDTH));
                roadColors.laneColor = EditorGUILayout.ColorField(roadColors.laneColor);
                EditorGUILayout.EndHorizontal();

                if (save.viewRoadsSettings.viewLanes)
                {
                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewWaypoints = EditorGUILayout.Toggle("View Waypoints", save.viewRoadsSettings.viewWaypoints, GUILayout.Width(TOGGLE_WIDTH));
                    roadColors.waypointColor = EditorGUILayout.ColorField(roadColors.waypointColor);
                    roadColors.disconnectedColor = EditorGUILayout.ColorField(roadColors.disconnectedColor);
                    if (save.viewRoadsSettings.viewWaypoints == false)
                    {
                        save.viewRoadsSettings.viewLaneChanges = false;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges, GUILayout.Width(TOGGLE_WIDTH));
                    if (save.viewRoadsSettings.viewLaneChanges == true)
                    {
                        save.viewRoadsSettings.viewWaypoints = true;
                    }
                    roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {

                SceneView.RepaintAll();
            }
            base.ScrollPart(width, height);
        }


        public override void DrawInScene()
        {
            if (firstClick != Vector3.zero)
            {
                Handles.SphereHandleCap(0, firstClick, Quaternion.identity, 1, EventType.Repaint);
            }

            if (save.viewOtherRoads)
            {
                for (int i = 0; i < allRoads.Count; i++)
                {
                    roadDrawer.DrawPath(allRoads[i], MoveTools.None, roadColors.roadColor, roadColors.controlPointColor, roadColors.controlPointColor, roadColors.textColor);
                    if (save.viewRoadsSettings.viewLanes)
                    {
                        LaneDrawer.DrawAllLanes(allRoads[i], save.viewRoadsSettings.viewWaypoints, save.viewRoadsSettings.viewLaneChanges, roadColors.laneColor,
                            roadColors.waypointColor, roadColors.disconnectedColor, roadColors.laneChangeColor, roadColors.textColor);
                    }
                }
            }
            base.DrawInScene();
        }


        public override void UndoAction()
        {
            base.UndoAction();
            if (secondClick == Vector3.zero)
            {
                if (firstClick != Vector3.zero)
                {
                    firstClick = Vector3.zero;
                }
            }
        }


        public override void LeftClick(Vector3 mousePosition)
        {
            if (firstClick == Vector3.zero)
            {
                firstClick = mousePosition;
            }
            else
            {
                secondClick = mousePosition;
                CreateRoad();
            }
            base.LeftClick(mousePosition);
        }


        public override void DestroyWindow()
        {
            settingsLoader.SaveCreateRoadSettings(save, roadColors);
            base.DestroyWindow();
        }


        protected virtual void CreateRoad()
        {          
            firstClick = Vector3.zero;
            secondClick = Vector3.zero;
        }
    }
}
