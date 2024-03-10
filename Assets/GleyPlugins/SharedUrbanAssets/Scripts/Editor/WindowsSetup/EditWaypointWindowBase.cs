using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class EditWaypointWindowBase<T2> : SetupWindowBase where T2 : WaypointSettingsBase
    {
        protected struct CarDisplay
        {
            public Color color;
            public int car;
            public bool active;
            public bool view;

            public CarDisplay(bool active, int car, Color color)
            {
                this.active = active;
                this.car = car;
                this.color = color;
                view = false;
            }
        }

        protected enum ListToAdd
        {
            None,
            Neighbors,
            OtherLanes
        }


        protected CarDisplay[] carDisplay;
        protected T2 selectedWaypoint;
        protected T2 clickedWaypoint;
        protected ListToAdd selectedList;
        protected int nrOfCars;

        private WaypointDrawerBase<T2> waypointDrawer;
        private SettingsLoader settingsLoader;
        private RoadColors roadColors;
        private float scrollAdjustment = 223;


        protected abstract CarDisplay[] SetCarDisplay();
        internal abstract WaypointDrawerBase<T2> SetWaypointsDrawer();
        internal abstract T2 SetSelectedWaypoint();
        internal abstract SettingsLoader SetSettingsLoader();
        protected abstract void ShowOtherLanes();
        protected abstract GUIContent SetAllowedAgentsText();
        internal abstract string SetLabels(int i);
        internal abstract void DrawCarSettings();
        protected abstract void SetCars();
        protected abstract void OpenEditWaypointWindow();
        internal abstract void ViewWaypoint(WaypointSettingsBase waypoint);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointDrawer = SetWaypointsDrawer(); 
            waypointDrawer.onWaypointClicked += WaypointClicked;
            waypointDrawer.Initialize();

            selectedWaypoint = SetSelectedWaypoint();
            settingsLoader = SetSettingsLoader();
            roadColors = settingsLoader.LoadRoadColors();

            carDisplay = SetCarDisplay();
            return this;
        }


        public override void DrawInScene()
        {
            base.DrawInScene();

            if (selectedList != ListToAdd.None)
            {
                waypointDrawer.DrawWaypointsForLink(selectedWaypoint, selectedWaypoint.neighbors, selectedWaypoint.otherLanes, roadColors.waypointColor, roadColors.speedColor);
            }

            waypointDrawer.DrawCurrentWaypoint(selectedWaypoint, roadColors.selectedWaypointColor, roadColors.waypointColor, roadColors.laneChangeColor, roadColors.prevWaypointColor);

            for (int i = 0; i < carDisplay.Length; i++)
            {
                if (carDisplay[i].view)
                {
                    waypointDrawer.DrawWaypointsForCar(carDisplay[i].car, carDisplay[i].color);
                }
            }

            if (clickedWaypoint)
            {
                waypointDrawer.DrawSelectedWaypoint(clickedWaypoint, roadColors.selectedRoadConnectorColor);
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
            roadColors.selectedWaypointColor = EditorGUILayout.ColorField("Selected Color ", roadColors.selectedWaypointColor);
            roadColors.waypointColor = EditorGUILayout.ColorField("Neighbor Color ", roadColors.waypointColor);
            roadColors.laneChangeColor = EditorGUILayout.ColorField("Lane Change Color ", roadColors.laneChangeColor);
            roadColors.prevWaypointColor = EditorGUILayout.ColorField("Previous Color ", roadColors.prevWaypointColor);

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Select Waypoint"))
            {
                Selection.activeGameObject = selectedWaypoint.gameObject;
            }

            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUI.BeginChangeCheck();
            if (selectedList == ListToAdd.None)
            {
                DrawCarSettings();
               
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(SetAllowedAgentsText(), EditorStyles.boldLabel);
                EditorGUILayout.Space();

                for (int i = 0; i < nrOfCars; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    carDisplay[i].active = EditorGUILayout.Toggle(carDisplay[i].active, GUILayout.MaxWidth(20));
                    EditorGUILayout.LabelField(SetLabels(i));
                    carDisplay[i].color = EditorGUILayout.ColorField(carDisplay[i].color, GUILayout.MaxWidth(80));
                    Color oldColor = GUI.backgroundColor;
                    if (carDisplay[i].view)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        carDisplay[i].view = !carDisplay[i].view;
                    }
                    GUI.backgroundColor = oldColor;

                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Set"))
                {
                    SetCars();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            MakeListOperations("Neighbors", "From this waypoint a moving agent can continue to the following ones", selectedWaypoint.neighbors, ListToAdd.Neighbors);
            ShowOtherLanes();

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            if (selectedWaypoint)
            {
                EditorUtility.SetDirty(selectedWaypoint);
            }
            settingsLoader.SaveRoadColors(roadColors);
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            base.DestroyWindow();
        }


        protected void MakeListOperations(string title, string description, List<WaypointSettingsBase> listToEdit, ListToAdd listType)
        {
            if (selectedList == listType || selectedList == ListToAdd.None)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel);
                EditorGUILayout.Space();
                for (int i = 0; i < listToEdit.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(listToEdit[i].name);
                    Color oldColor = GUI.backgroundColor;
                    if (listToEdit[i] == clickedWaypoint)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View", GUILayout.MaxWidth(64)))
                    {
                        if (listToEdit[i] == clickedWaypoint)
                        {
                            clickedWaypoint = null;
                        }
                        else
                        {
                            ViewWaypoint(listToEdit[i]);
                        }
                    }
                    GUI.backgroundColor = oldColor;
                    if (GUILayout.Button("Delete", GUILayout.MaxWidth(64)))
                    {
                        DeleteWaypoint(listToEdit[i], listType == ListToAdd.OtherLanes);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                if (selectedList == ListToAdd.None)
                {
                    if (GUILayout.Button("Add/Remove " + title))
                    {
                        waypointDrawer.Initialize();
                        selectedList = listType;
                    }
                }
                else
                {
                    if (GUILayout.Button("Done"))
                    {
                        selectedList = ListToAdd.None;
                        SceneView.RepaintAll();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }


        private void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            if (leftClick)
            {
                if (selectedList == ListToAdd.Neighbors)
                {
                    AddNeighbor(clickedWaypoint);
                }

                if (selectedList == ListToAdd.OtherLanes)
                {
                    AddOtherLanes(clickedWaypoint);
                }

                if (selectedList == ListToAdd.None)
                {
                    OpenEditWaypointWindow();

                }
            }
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }


        private void DeleteWaypoint(WaypointSettingsBase waypoint, bool other)
        {
            if (!other)
            {
                waypoint.prev.Remove(selectedWaypoint);
                selectedWaypoint.neighbors.Remove(waypoint);
            }
            else
            {
                selectedWaypoint.otherLanes.Remove(waypoint);
            }
            clickedWaypoint = null;
            SceneView.RepaintAll();
        }


        private void AddNeighbor(WaypointSettingsBase neighbor)
        {
            if (!selectedWaypoint.neighbors.Contains(neighbor))
            {
                selectedWaypoint.neighbors.Add(neighbor);
                neighbor.prev.Add(selectedWaypoint);
            }
        }


        private void AddOtherLanes(WaypointSettingsBase waypoint)
        {
            if (!selectedWaypoint.otherLanes.Contains(waypoint))
            {
                selectedWaypoint.otherLanes.Add(waypoint);
            }
        }
    }
}