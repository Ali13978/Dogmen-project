using GleyTrafficSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    /// <summary>
    /// Converts editor waypoints to production waypoints
    /// </summary>
    public partial class GridEditor : Editor
    {
        static List<WaypointSettings> allEditorWaypoints;
        static GenericIntersectionSettings[] allEditorIntersections;

        static bool AssignTrafficWaypoints(CurrentSceneData currentSceneData)
        {
            if (currentSceneData == null || currentSceneData.grid == null || currentSceneData.grid.Length == 0)
            {
                Debug.LogError("Grid is null. Go to Window->Gley->Traffic System->Scene Setup->Grid Setup and set up your grid");
                return false;
            }

            System.DateTime startTime = System.DateTime.Now;
            SetTags();
            ClearAllWaypoints(currentSceneData);
            List<WaypointSettings> allWaypoints = FindObjectsOfType<WaypointSettings>().ToList();
            if (allWaypoints.Count <= 0)
            {
                Debug.LogError("No waypoints found. Go to Window->Gley->Traffic System->Road Setup and create a road");
                return false;
            }

            //reset intersection waypoints
            for (int i = 0; i < allWaypoints.Count; i++)
            {
                allWaypoints[i].enter = allWaypoints[i].exit = false;
            }

            allEditorWaypoints = new List<WaypointSettings>();
            allEditorIntersections = FindObjectsOfType<GenericIntersectionSettings>();
            for (int i = 0; i < allEditorIntersections.Length; i++)
            {
                List<IntersectionStopWaypointsSettings> intersectionWaypoints = allEditorIntersections[i].GetAssignedWaypoints();
                if (intersectionWaypoints != null)
                {
                    for (int j = 0; j < intersectionWaypoints.Count; j++)
                    {
                        for (int k = 0; k < intersectionWaypoints[j].roadWaypoints.Count; k++)
                        {
                            if (intersectionWaypoints[j].roadWaypoints[k] == null)
                            {
                                Debug.LogError(allEditorIntersections[i].name + " has null waypoints assigned, please check it.");
                                continue;
                            }
                            else
                            {
                                intersectionWaypoints[j].roadWaypoints[k].enter = true;
                            }
                        }
                    }
                }
                List<WaypointSettings> exitWaypoints = allEditorIntersections[i].GetExitWaypoints();
                if (exitWaypoints != null)
                {
                    for (int j = 0; j < exitWaypoints.Count; j++)
                    {
                        if (exitWaypoints[j] == null)
                        {
                            Debug.LogError(allEditorIntersections[i].name + " has null waypoints assigned, please check it.");
                        }
                        else
                        {
                            exitWaypoints[j].exit = true;
                        }
                    }
                }
            }

            for (int i = allWaypoints.Count - 1; i >= 0; i--)
            {
                if (allWaypoints[i].allowedCars.Count != 0)
                {
                    allEditorWaypoints.Add(allWaypoints[i]);
                    GridCell cell = currentSceneData.GetCell(allWaypoints[i].transform.position);
                    cell.AddWaypoint(allEditorWaypoints.Count - 1, allWaypoints[i].name, allWaypoints[i].allowedCars.Cast<int>().ToList(), allWaypoints[i].enter || allWaypoints[i].exit);

                }
            }
            currentSceneData.allWaypoints = allEditorWaypoints.ToPlayWaypoints(allEditorWaypoints).ToArray();
            AssignIntersections(currentSceneData);
            EditorUtility.SetDirty(currentSceneData);
            Debug.Log("Done assign vehicle waypoints in " + (System.DateTime.Now - startTime));
            return true;
        }



        private static void AssignIntersections(CurrentSceneData currentSceneData)
        {
            List<PriorityIntersection> priorityIntersections = new List<PriorityIntersection>();
            List<TrafficLightsIntersection> lightsIntersections = new List<TrafficLightsIntersection>();
            currentSceneData.allIntersections = new IntersectionData[allEditorIntersections.Length];
            for (int i = 0; i < allEditorIntersections.Length; i++)
            {
                if (allEditorIntersections[i].GetType().Equals(typeof(TrafficLightsIntersectionSettings)))
                {
                    TrafficLightsIntersection intersection = ((TrafficLightsIntersectionSettings)allEditorIntersections[i]).ToPlayModeIntersection(allEditorWaypoints);
                    GetPedestrianWaypoints(intersection, (TrafficLightsIntersectionSettings)allEditorIntersections[i]);
                    lightsIntersections.Add(intersection);

                    currentSceneData.allIntersections[i] = new IntersectionData(IntersectionType.TrafficLights, lightsIntersections.Count - 1);
                }

                if (allEditorIntersections[i].GetType().Equals(typeof(PriorityIntersectionSettings)))
                {
                    PriorityIntersection intersection = ((PriorityIntersectionSettings)allEditorIntersections[i]).ToPlayModeIntersection(allEditorWaypoints);
                    GetPedestrianWaypoints(intersection, (PriorityIntersectionSettings)allEditorIntersections[i]);
                    priorityIntersections.Add(intersection);
                    currentSceneData.allIntersections[i] = new IntersectionData(IntersectionType.Priority, priorityIntersections.Count - 1);
                }

                List<IntersectionStopWaypointsSettings> intersectionWaypoints = allEditorIntersections[i].GetAssignedWaypoints();
                for (int j = 0; j < intersectionWaypoints.Count; j++)
                {
                    for (int k = 0; k < intersectionWaypoints[j].roadWaypoints.Count; k++)
                    {
                        if (intersectionWaypoints[j].roadWaypoints[k] == null)
                        {
                            intersectionWaypoints[j].roadWaypoints.RemoveAt(k);
                        }
                        else
                        {
                            GridCell intersectionCell = currentSceneData.GetCell(intersectionWaypoints[j].roadWaypoints[k].transform.position);
                            intersectionCell.AddIntersection(i);
                        }
                    }
                }
            }
            currentSceneData.allPriorityIntersections = priorityIntersections.ToArray();
            currentSceneData.allLightsIntersections = lightsIntersections.ToArray();
        }

        static partial void GetPedestrianWaypoints(TrafficLightsIntersection intersection, TrafficLightsIntersectionSettings currentIntersection);
        static partial void GetPedestrianWaypoints(PriorityIntersection intersection, PriorityIntersectionSettings currentIntersection);


        private static void ClearAllWaypoints(CurrentSceneData currentSceneData)
        {
            if (currentSceneData.grid != null)
            {
                for (int i = 0; i < currentSceneData.grid.Length; i++)
                {
                    for (int j = 0; j < currentSceneData.grid[i].row.Length; j++)
                    {
                        currentSceneData.grid[i].row[j].ClearReferences();
                    }
                }
            }
        }
    }
}

