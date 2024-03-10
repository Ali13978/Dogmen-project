using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class PriorityIntersectionWindow : IntersectionWindowBase
    {
        private List<WaypointSettings> exitWaypoints = new List<WaypointSettings>();
        private PriorityIntersectionSettings selectedPriorityIntersection;
        private float scrollAdjustment = 187;
        private bool addExitWaypoints;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            selectedPriorityIntersection = selectedIntersection as PriorityIntersectionSettings;
            stopWaypoints = selectedPriorityIntersection.enterWaypoints;
            exitWaypoints = selectedPriorityIntersection.exitWaypoints;
            return base.Initialize(windowProperties, window);
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                if (exitWaypoints[i].draw)
                {
                    IntersectionDrawer.DrawIntersectionWaypoint(exitWaypoints[i], intersectionSave.exitWaypointsColor, 0, save.textColor);
                }
            }
            base.DrawInScene();
            if (addExitWaypoints)
            {
                waypointDrawer.DrawAllWaypoints(save.waypointColor, true, save.waypointColor, false, Color.white, false, Color.white, false, Color.white);
            }
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            Color oldColor;
            if (!addExitWaypoints)
            {
                DrawStopWaypointButtons(false);
            }
            if (!addWaypoints)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Exit waypoints:","When a vehicle touches an exit waypoint, it is no longer considered inside intersection.\n" +
                    "For every lane that exits the intersection a single exit point should be marked"));
                EditorGUILayout.Space();

                for (int i = 0; i < exitWaypoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    exitWaypoints[i] = (WaypointSettings)EditorGUILayout.ObjectField(exitWaypoints[i], typeof(WaypointSettings), true);

                    oldColor = GUI.backgroundColor;
                    if (exitWaypoints[i].draw == true)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    if (GUILayout.Button("View"))
                    {
                        ViewWaypoint(exitWaypoints[i], i);
                    }
                    GUI.backgroundColor = oldColor;


                    if (GUILayout.Button("Delete"))
                    {
                        exitWaypoints[i].exit = false;
                        exitWaypoints.RemoveAt(i);
                        SceneView.RepaintAll();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (!addExitWaypoints)
                {
                    if (GUILayout.Button("Add Exit Waypoints"))
                    {
                        AddExitWaypoints();
                    }
                }
                oldColor = GUI.backgroundColor;
                if (intersectionSave.showExit == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View Exit Waypoints"))
                {
                    ViewAll();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();

                if (addExitWaypoints)
                {
                    if (GUILayout.Button("Done"))
                    {
                        Cancel();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();

        }


        private void ViewWaypoint(WaypointSettings waypoint, int index)
        {
            waypoint.draw = !waypoint.draw;
            if (waypoint.draw == false)
            {
                exitWaypoints[index].draw = false;
                intersectionSave.showExit = false;
            }
            SceneView.RepaintAll();
        }


        protected override void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            if (addExitWaypoints)
            {
                AddExitWaypointsToList(clickedWaypoint);
            }
            base.WaypointClicked(clickedWaypoint, leftClick);
        }


        private void AddExitWaypointsToList(WaypointSettings waypoint)
        {
            if (!exitWaypoints.Contains(waypoint))
            {
                waypoint.draw = true;
                exitWaypoints.Add(waypoint);
            }
            else
            {
                exitWaypoints.Remove(waypoint);
            }
            SceneView.RepaintAll();
        }


        private void AddExitWaypoints()
        {
            selectedRoad = -1;
            addExitWaypoints = true;
        }


        private void ViewAll()
        {
            intersectionSave.showExit = !intersectionSave.showExit;
            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                exitWaypoints[i].draw = intersectionSave.showExit;
            }
        }


        private void Cancel()
        {
            addExitWaypoints = false;
            SceneView.RepaintAll();
        }
    }
}
