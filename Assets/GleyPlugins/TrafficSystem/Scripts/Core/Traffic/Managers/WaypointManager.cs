using GleyUrbanAssets;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Performs waypoint operations
    /// </summary>
    public class WaypointManager : WaypointManagerBase
    {
        internal WaypointManager Initialize(Waypoint[] allWaypoints, int nrOfVehicles, bool debugWaypoints, bool debugDisabledWaypoints)
        {
            WaypointEvents.onTrafficLightChanged += TrafficLightChanged;
            base.Initialize(allWaypoints, nrOfVehicles, debugWaypoints, debugDisabledWaypoints);
            return this;
        }


        protected override void TrafficLightChanged(int waypointIndex, bool newValue)
        {
            if (GetWaypoint<Waypoint>(waypointIndex).stop != newValue)
            {
                GetWaypoint<Waypoint>(waypointIndex).stop = newValue;
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] == waypointIndex)
                    {
                        WaypointEvents.TriggerStopStateChangedEvent(i, GetWaypoint<Waypoint>(waypointIndex).stop);
                    }
                }
            }

        }

        /// <summary>
        /// Set next waypoint and trigger the required events
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="waypointIndex"></param>
        internal override void SetNextWaypoint(int vehicleIndex, int waypointIndex)
        {
            base.SetNextWaypoint(vehicleIndex, waypointIndex);
            Waypoint targetWaypoint = GetTargetWaypointOfAgent<Waypoint>(vehicleIndex);
            if (targetWaypoint.stop == true)
            {
                WaypointEvents.TriggerStopStateChangedEvent(vehicleIndex, targetWaypoint.stop);
            }
            if (targetWaypoint.giveWay == true)
            {
                WaypointEvents.TriggerGiveWayStateChangedEvent(vehicleIndex, targetWaypoint.giveWay);
            }
        }


        /// <summary>
        /// Check if target waypoint of the vehicle is in intersection
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal bool IsInIntersection(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).IsInIntersection();
        }


        /// <summary>
        /// Check if can switch to target waypoint
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal bool CanEnterIntersection(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).CanChange();
        }


        /// <summary>
        /// Check if the previous waypoints are free
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="freeWaypointsNeeded"></param>
        /// <param name="possibleWaypoints"></param>
        /// <returns></returns>
        internal bool AllPreviousWaypointsAreFree(int vehicleIndex, int freeWaypointsNeeded, int waypointToCheck)
        {
            return IsTargetFree(GetWaypoint<WaypointBase>(waypointToCheck), freeWaypointsNeeded, GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex), vehicleIndex);
        }


        /// <summary>
        /// Check what vehicle is in front
        /// </summary>
        /// <param name="vehicleIndex1"></param>
        /// <param name="vehicleIndex2"></param>
        /// <returns>
        /// 1-> if 1 is in front of 2
        /// 2-> if 2 is in front of 1
        /// 0-> if it is not possible to determine
        /// </returns>
        internal int IsInFront(int vehicleIndex1, int vehicleIndex2)
        {
            //compares waypoints to determine which vehicle is in front 
            int distance = 0;
            //if no neighbors are available -> not possible to determine
            if (GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex1).neighbors.Count == 0)
            {
                return 0;
            }

            //check next 10 waypoints to find waypoint 2
            int startWaypointIndex = GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex1).neighbors[0];
            while (startWaypointIndex != GetTargetWaypointIndex(vehicleIndex2) && distance < 10)
            {
                distance++;
                if (GetWaypoint<WaypointBase>(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint<WaypointBase>(startWaypointIndex).neighbors[0];
            }


            int distance2 = 0;
            if (GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex2).neighbors.Count == 0)
            {
                return 0;
            }

            startWaypointIndex = GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex2).neighbors[0];
            while (startWaypointIndex != GetTargetWaypointIndex(vehicleIndex1) && distance2 < 10)
            {
                distance2++;
                if (GetWaypoint<WaypointBase>(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint<WaypointBase>(startWaypointIndex).neighbors[0];
            }

            //if no waypoints found -> not possible to determine
            if (distance == 10 && distance2 == 10)
            {
                return 0;
            }

            if (distance2 > distance)
            {
                return 2;
            }

            return 1;
        }


        /// <summary>
        /// Check if 2 vehicles have the same target
        /// </summary>
        /// <param name="vehicleIndex1"></param>
        /// <param name="VehicleIndex2"></param>
        /// <returns></returns>
        internal bool IsSameTarget(int vehicleIndex1, int VehicleIndex2)
        {
            return GetTargetWaypointIndex(vehicleIndex1) == GetTargetWaypointIndex(VehicleIndex2);
        }


        /// <summary>
        /// Get waypoint speed
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal float GetMaxSpeed(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).maxSpeed;
        }


        /// <summary>
        /// Converts distance to waypoint number
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="lookDistance"></param>
        /// <returns></returns>
        internal int GetNrOfWaypointsToCheck(int vehicleIndex, float lookDistance)
        {
            WaypointBase currentWaypoint = GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex);
            float waypointDistance = 4;
            if (currentWaypoint.neighbors.Count > 0)
            {
                waypointDistance = Vector3.Distance(currentWaypoint.position, GetWaypoint<WaypointBase>(currentWaypoint.neighbors[0]).position);
            }
            return Mathf.CeilToInt(lookDistance / waypointDistance);
        }


        /// <summary>
        /// Check if previous waypoints are free
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="level"></param>
        /// <param name="initialWaypoint"></param>
        /// <returns></returns>
        private bool IsTargetFree(WaypointBase waypoint, int level, WaypointBase initialWaypoint, int currentCarIndex)
        {
#if UNITY_EDITOR
            if (debugWaypoints)
            {
                Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.green, 1);
            }
#endif
            if (level == 0)
            {
                return true;
            }
            if (waypoint == initialWaypoint)
            {
                return true;
            }
            if (IsThisWaypointATarget(waypoint.listIndex))
            {
                if (GetTargetWaypointIndex(currentCarIndex) == waypoint.listIndex)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (waypoint.prev.Count <= 0)
                {
                    return true;
                }
                level--;
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    if (!IsTargetFree(GetWaypoint<WaypointBase>(waypoint.prev[i]), level, initialWaypoint, currentCarIndex))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            WaypointEvents.onTrafficLightChanged -= TrafficLightChanged;
        }
    }
}