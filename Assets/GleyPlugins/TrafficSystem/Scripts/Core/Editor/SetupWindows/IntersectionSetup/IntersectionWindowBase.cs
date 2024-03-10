using GleyUrbanAssets;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public partial class IntersectionWindowBase : SetupWindowBase
    {
        protected List<IntersectionStopWaypointsSettings> stopWaypoints = new List<IntersectionStopWaypointsSettings>();
        protected GenericIntersectionSettings selectedIntersection;
        protected IntersectionSave intersectionSave;
        protected RoadColors save;
        protected WaypointDrawer waypointDrawer;
        protected int selectedRoad;
        protected bool addWaypoints;

        private SettingsLoader settingsLoader;
        private bool hideWaypoints;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            selectedRoad = -1;
            name = selectedIntersection.name;
            waypointDrawer = CreateInstance<WaypointDrawer>();
            waypointDrawer.Initialize();
            waypointDrawer.onWaypointClicked += WaypointClicked;
            settingsLoader = new SettingsLoader(Constants.windowSettingsPath);
            save = settingsLoader.LoadRoadColors();
            intersectionSave = settingsLoader.LoadIntersectionsSettings();
            if (stopWaypoints == null)
            {
                stopWaypoints = new List<IntersectionStopWaypointsSettings>();
            }
            InitializePedestrianWaypoints();
            return this;
        }

        partial void InitializePedestrianWaypoints();

        public override void DrawInScene()
        {
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                if (stopWaypoints[i].draw)
                {
                    IntersectionDrawer.DrawStopWaypoints(stopWaypoints[i].roadWaypoints, intersectionSave.stopWaypointsColor, i + 1, save.textColor);
                }
                else
                {
                    for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                    {
                        if (stopWaypoints[i].roadWaypoints[j].draw)
                        {
                            IntersectionDrawer.DrawIntersectionWaypoint(stopWaypoints[i].roadWaypoints[j], intersectionSave.stopWaypointsColor, i + 1, save.textColor);
                        }
                    }
                }
            }

            if (addWaypoints)
            {
                if (hideWaypoints == false)
                {
                    waypointDrawer.DrawAllWaypoints(save.waypointColor, true, save.waypointColor, false, Color.white, false, Color.white, false, Color.white);
                }
            }

            DrawPedetsrianSceneWaypoints();
            base.DrawInScene();
        }

        partial void DrawPedetsrianSceneWaypoints();

        protected override void TopPart()
        {
            name = EditorGUILayout.TextField("Intersection Name", name);
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
            EditorGUI.BeginChangeCheck();
            hideWaypoints = EditorGUILayout.Toggle("Hide Waypoints ", hideWaypoints);
            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                window.BlockClicks(!hideWaypoints);
            }
            base.TopPart();
        }


        protected override void ScrollPart(float width, float height)
        {
            DrawPedestrianWaypoints();
            base.ScrollPart(width, height);
        }

        public override void DestroyWindow()
        {
            EditorUtility.SetDirty(selectedIntersection);
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            RemovePedestrianListeners();
            base.DestroyWindow();
        }

        partial void RemovePedestrianListeners();

        partial void DrawPedestrianWaypoints();

        protected void DrawStopWaypointButtons(bool showLights)
        {
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
                "\nEach road that enters in intersection should have its own set of stop waypoints"));
            Color oldColor;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                if (i == selectedRoad || selectedRoad == -1)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Road " + (i + 1));
                    EditorGUILayout.Space();
                    for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        stopWaypoints[i].roadWaypoints[j] = (WaypointSettings)EditorGUILayout.ObjectField(stopWaypoints[i].roadWaypoints[j], typeof(WaypointSettings), true);

                        oldColor = GUI.backgroundColor;
                        if (stopWaypoints[i].roadWaypoints[j].draw == true)
                        {
                            GUI.backgroundColor = Color.green;
                        }
                        if (GUILayout.Button("View"))
                        {
                            ViewWaypoint(stopWaypoints[i].roadWaypoints[j], i);
                        }
                        GUI.backgroundColor = oldColor;

                        if (GUILayout.Button("Delete"))
                        {
                            Delete(i, j);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    if (!addWaypoints)
                    {
                        if (GUILayout.Button("Assign Road"))
                        {
                            AddStopWaypoints(i);
                        }
                    }
                    oldColor = GUI.backgroundColor;
                    if (stopWaypoints[i].draw == true)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View Road Waypoints"))
                    {
                        ViewRoadWaypoints(i);

                    }
                    GUI.backgroundColor = oldColor;

                    if (!addWaypoints)
                    {
                        if (GUILayout.Button("Delete Road"))
                        {
                            stopWaypoints.RemoveAt(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (showLights)
                    {
                        if (addWaypoints)
                        {
                            AddLightObjects("Red Light", stopWaypoints[i].redLightObjects);
                            AddLightObjects("Yellow Light", stopWaypoints[i].yellowLightObjects);
                            AddLightObjects("Green Light", stopWaypoints[i].greenLightObjects);
                        }
                    }

                    if (selectedRoad != -1)
                    {
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Done"))
                        {
                            Cancel();
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            if (selectedRoad == -1)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Add Road"))
                {
                    stopWaypoints.Add(new IntersectionStopWaypointsSettings());
                }
            }
        }


        private void AddLightObjects(string title, List<GameObject> objectsList)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title + ":");
            for (int i = 0; i < objectsList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                objectsList[i] = (GameObject)EditorGUILayout.ObjectField(objectsList[i], typeof(GameObject), true);

                if (GUILayout.Button("Delete"))
                {
                    objectsList.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add " + title + " Objects"))
            {
                objectsList.Add(null);
            }
            EditorGUILayout.EndVertical();
        }


        void ViewWaypoint(WaypointSettings waypoint, int index)
        {
            waypoint.draw = !waypoint.draw;
            if (waypoint.draw == false)
            {
                stopWaypoints[index].draw = false;
            }
            SceneView.RepaintAll();
        }


        protected virtual void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            if (addWaypoints)
            {
                AddWaypointToList(clickedWaypoint);
            }
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }


        private void Cancel()
        {
            selectedRoad = -1;
            addWaypoints = false;
            SceneView.RepaintAll();
        }


        private void SaveSettings()
        {
            selectedIntersection.gameObject.name = name;
            if (stopWaypoints.Count > 0)
            {
                Vector3 position = new Vector3();
                for (int i = 0; i < stopWaypoints.Count; i++)
                {
                    position += stopWaypoints[i].roadWaypoints[0].transform.position;
                }
                selectedIntersection.transform.position = position / stopWaypoints.Count;
            }
        }


        private void Delete(int i, int j)
        {
            stopWaypoints[i].roadWaypoints[j].enter = false;
            stopWaypoints[i].roadWaypoints.RemoveAt(j);
            if (stopWaypoints[i].roadWaypoints.Count == 0)
            {
                stopWaypoints.RemoveAt(i);
            }
            SceneView.RepaintAll();
        }


        private void AddStopWaypoints(int selectedRoad)
        {
            this.selectedRoad = selectedRoad;
            addWaypoints = true;
        }


        private void ViewRoadWaypoints(int i)
        {
            stopWaypoints[i].draw = !stopWaypoints[i].draw;
            for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
            {
                stopWaypoints[i].roadWaypoints[j].draw = stopWaypoints[i].draw;
            }
        }


        private void AddWaypointToList(WaypointSettings waypoint)
        {
            if (selectedRoad != -1)
            {
                if (!stopWaypoints[selectedRoad].roadWaypoints.Contains(waypoint))
                {
                    stopWaypoints[selectedRoad].roadWaypoints.Add(waypoint);
                }
                else
                {
                    stopWaypoints[selectedRoad].roadWaypoints.Remove(waypoint);
                }
            }
            SceneView.RepaintAll();
        }
    }
}
