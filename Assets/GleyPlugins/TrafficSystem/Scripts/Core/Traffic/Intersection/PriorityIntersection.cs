using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEngine;
namespace GleyTrafficSystem
{
    [System.Serializable]
    public partial class PriorityIntersection : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> enterWaypoints;
        public List<int> exitWaypoints;

        private float currentTime;
        private float requiredTime;
        private List<int> carsInIntersection;
        private int currentRoadIndex;
        private int tempRoadIndex;
        private bool changeRequested;


        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enterWaypoints"></param>
        /// <param name="exitWaypoints"></param>
        public PriorityIntersection(string name, List<IntersectionStopWaypointsIndex> enterWaypoints, List<int> exitWaypoints)
        {
            this.name = name;
            this.enterWaypoints = enterWaypoints;
            this.exitWaypoints = exitWaypoints;
        }


        /// <summary>
        /// Assigns corresponding waypoints to work with this intersection
        /// </summary>
        /// <param name="waypointManager"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        public override void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
            carsInIntersection = new List<int>();
            requiredTime = 3;
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                for (int j = 0; j < enterWaypoints[i].roadWaypoints.Count; j++)
                {
                    waypointManager.GetWaypoint<Waypoint>(enterWaypoints[i].roadWaypoints[j]).SetIntersection(this, true, false, true, false);
                }
            }

            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                waypointManager.GetWaypoint<Waypoint>(exitWaypoints[i]).SetIntersection(this, false, false, false, true);
            }
        }


        /// <summary>
        /// Reset the intersection when becomes inactive
        /// </summary>
        public override void ResetIntersection()
        {
            base.ResetIntersection();
            carsInIntersection = new List<int>();
        }


        public override List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return enterWaypoints;
        }

        /// <summary>
        /// Check if the intersection road is free and update intersection priority
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        public override bool IsPathFree(int waypointIndex)
        {
            int waypointRoad = 0;
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                //if the waypoint is in the enter waypoints list needs to be verified if is free
                if (enterWaypoints[i].roadWaypoints.Contains(waypointIndex))
                {
                    waypointRoad = i;
                    bool stopChange = false;
                    //if vehicle is on current road, wait to pass before changing the road priority
                    if (i == currentRoadIndex)
                    {
                        if (currentRoadIndex > tempRoadIndex || (tempRoadIndex == 3 && currentRoadIndex == 0))
                        {
                            changeRequested = false;
                            stopChange = true;
                        }
                    }

                    //construct priority if vehicle is not on the priority road
                    if (stopChange == false)
                    {
                        if (waypointRoad == 0)
                        {
                            if (tempRoadIndex == 2 || tempRoadIndex == 3)
                            {
                                tempRoadIndex = waypointRoad;
                                changeRequested = true;
                                currentTime = Time.timeSinceLevelLoad;
                            }
                        }
                        if (waypointRoad == 1)
                        {
                            if (tempRoadIndex == 0 || tempRoadIndex == 3)
                            {
                                tempRoadIndex = waypointRoad;
                                changeRequested = true;
                                currentTime = Time.timeSinceLevelLoad;
                            }
                        }
                        if (waypointRoad == 2)
                        {
                            if (tempRoadIndex == 0 || tempRoadIndex == 1)
                            {
                                tempRoadIndex = waypointRoad;
                                changeRequested = true;
                                currentTime = Time.timeSinceLevelLoad;
                            }
                        }

                        if (waypointRoad == 3)
                        {
                            if (tempRoadIndex == 1 || tempRoadIndex == 2)
                            {
                                tempRoadIndex = waypointRoad;
                                changeRequested = true;
                                currentTime = Time.timeSinceLevelLoad;
                            }
                        }
                    }
                    break;
                }
            }

            //if a new vehicle is requesting access to intersection but there are vehicles on intersection -> wait
            if (changeRequested == true)
            {
                if (carsInIntersection.Count >= 1)
                {
                    return false;
                }
                changeRequested = false;
                currentRoadIndex = tempRoadIndex;
            }

            //if the number of vehicles in intersection is <3 -> permit access
            if (carsInIntersection.Count <= 3)
            {
                if (enterWaypoints[currentRoadIndex].roadWaypoints.Contains(waypointIndex))
                {
                    return true;
                }
            }

            //after some time change the priority road
            if (Time.timeSinceLevelLoad - currentTime > requiredTime)
            {
                tempRoadIndex = waypointRoad;
                changeRequested = true;
                currentTime = Time.timeSinceLevelLoad;
            }

            return false;
        }

        internal override void RemoveCar(int index)
        {
            base.RemoveCar(index);
            VehicleLeft(index);
        }

        /// <summary>
        /// Called every time a vehicle passes through an exit waypoint
        /// </summary>
        /// <param name="waypoint"></param>
        public override void VehicleLeft(int vehicleIndex)
        {
            carsInIntersection.Remove(vehicleIndex);
        }


        /// <summary>
        /// Called every time a vehicle passes through a stop waypoint
        /// </summary>
        /// <param name="waypoint"></param>
        public override void VehicleEnter(int vehicleIndex)
        {
            carsInIntersection.Add(vehicleIndex);
        }

        public override void UpdateIntersection(float realtimeSinceStartup)
        {
            base.UpdateIntersection(realtimeSinceStartup);
            string text = name;
            for(int i=0;i<carsInIntersection.Count;i++)
            {
                text += " " + carsInIntersection[i];
            }
            //Debug.Log(text);
        }
    }
}