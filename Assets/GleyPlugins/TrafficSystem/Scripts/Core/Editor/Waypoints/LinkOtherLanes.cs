using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class LinkOtherLanes : MonoBehaviour
    {
        public static void Link(Road road)
        {
            int nrOfLanes = road.transform.Find(GleyUrbanAssets.Constants.lanesHolderName).childCount;
            float maxLength = road.waypointDistance * road.waypointDistance * 9;

            for (int i = 0; i < nrOfLanes; i++)
            {
                ClearLinks(road, i);
            }
            for (int i = 0; i < nrOfLanes; i++)
            {
                LinkSameDirectionLanes(road, i, nrOfLanes, maxLength);
            }
        }


        public static void Unlinck(Road road)
        {
            int nrOfLanes = road.transform.Find(GleyUrbanAssets.Constants.lanesHolderName).childCount;

            for (int i = 0; i < nrOfLanes; i++)
            {
                ClearLinks(road, i);
            }
        }


        private static void ClearLinks(Road road, int laneIndex)
        {
            Transform laneToLink = road.transform.Find(GleyUrbanAssets.Constants.lanesHolderName).Find(GleyUrbanAssets.Constants.laneNamePrefix + laneIndex);
            for (int i = 0; i < laneToLink.transform.childCount; i++)
            {
                laneToLink.transform.GetChild(i).GetComponent<WaypointSettings>().otherLanes = new List<GleyUrbanAssets.WaypointSettingsBase>();
            }
        }


        private static void LinkSameDirectionLanes(Road road, int laneIndex, int nrOfLanes, float maxLength)
        {
            Transform currentLane = road.transform.Find(GleyUrbanAssets.Constants.lanesHolderName).Find(GleyUrbanAssets.Constants.laneNamePrefix + laneIndex);
            int[] neighbors = GetNeighbors(laneIndex, nrOfLanes);
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (neighbors[i] != -1)
                {
                    if (road.lanes[neighbors[i]].laneDirection == road.lanes[laneIndex].laneDirection)
                    {
                        Transform otherLane = road.transform.Find(GleyUrbanAssets.Constants.lanesHolderName).Find(GleyUrbanAssets.Constants.laneNamePrefix + neighbors[i]);
                        int currentLaneCount = 0;
                        int otherLaneCount = 0;
                        if (currentLane.GetChild(0).name.Contains(GleyUrbanAssets.Constants.outWaypointEnding))
                        {
                            currentLaneCount = currentLane.childCount - 1;
                            otherLaneCount = otherLane.childCount - 1;
                        }
                        int startIndex = 0;
                        int currentIndex = 0;
                        int steps = 10;
                        for (int j = 0; j < currentLane.childCount; j++)
                        {

                            WaypointSettings currentLaneWaypoint = currentLane.GetChild(Mathf.Abs(currentLaneCount - j)).GetComponent<WaypointSettings>();

                            bool connected = false;
                            while (currentIndex < startIndex + steps && currentIndex < otherLane.childCount)
                            {
                                WaypointSettings otherLaneWaypoint = otherLane.GetChild(Mathf.Abs(otherLaneCount - currentIndex)).GetComponent<WaypointSettings>();
                                if (ConnectionIsValid(currentLaneWaypoint, otherLaneWaypoint, maxLength))
                                {
                                    currentLaneWaypoint.otherLanes.Add(otherLaneWaypoint);
                                    connected = true;
                                    break;
                                }
                                currentIndex++;
                            }
                            if (connected)
                            {
                                currentIndex++;
                                startIndex = currentIndex;
                            }
                            else
                            {
                                currentIndex = startIndex;
                            }
                        }
                    }
                }
            }
        }


        private static bool ConnectionIsValid(WaypointSettings currentLaneWaypoint, WaypointSettings otherLaneWaypoint, float maxLength)
        {
            if (currentLaneWaypoint.neighbors.Count == 0 || otherLaneWaypoint.neighbors.Count == 0 || currentLaneWaypoint.prev.Count == 0)
            {
                return false;
            }

            float angle = Vector3.Angle(currentLaneWaypoint.neighbors[0].transform.position - currentLaneWaypoint.transform.position, otherLaneWaypoint.transform.position - currentLaneWaypoint.transform.position);
            if (angle > 45)
            {

                return false;
            }

            angle = Vector3.Angle(currentLaneWaypoint.transform.position - otherLaneWaypoint.transform.position, otherLaneWaypoint.neighbors[0].transform.position - otherLaneWaypoint.transform.position);
            if (angle < 135)
            {
                return false;
            }

            angle = Vector3.Angle(currentLaneWaypoint.prev[0].transform.position - currentLaneWaypoint.transform.position, otherLaneWaypoint.transform.position - currentLaneWaypoint.transform.position);
            if (angle < 115)
            {
                return false;
            }

            if (Vector3.SqrMagnitude(currentLaneWaypoint.transform.position - otherLaneWaypoint.transform.position) > maxLength)
            {
                return false;
            }

            return true;
        }


        private static int[] GetNeighbors(int laneIndex, int nrOfLanes)
        {
            int[] result = new int[2];
            if (laneIndex == 0)
            {
                result[0] = 1;
                result[1] = 2;
            }
            if (laneIndex == 1)
            {
                result[0] = 0;
                result[1] = 3;
            }

            if (laneIndex >= 2)
            {
                result[0] = laneIndex - 2;
                result[1] = laneIndex + 2;
            }
            if (result[1] >= nrOfLanes)
            {
                result[1] = -1;
            }
            return result;
        }
    }
}

