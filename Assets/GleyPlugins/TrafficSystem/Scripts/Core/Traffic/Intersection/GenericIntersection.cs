using GleyUrbanAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Base class for all intersections
    /// </summary>
    [System.Serializable]
    public class GenericIntersection : IIntersection
    {
        public string name;


        public virtual void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {

        }

        public virtual void ResetIntersection()
        {

        }


        public virtual void UpdateIntersection(float realtimeSinceStartup)
        {

        }


        public virtual bool IsPathFree(int waypointIndex)
        {
            return false;
        }


        public virtual void VehicleEnter(int vehicleindex)
        {

        }


        public virtual void VehicleLeft(int vehicleIndex)
        {

        }


        public virtual List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return new List<IntersectionStopWaypointsIndex>();
        }

        public virtual void ResetIntersections(Vector3 center, float radius)
        {
            
        }

        internal virtual void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            
        }

        internal virtual void SetGreenRoad(int roadIndex, bool doNotChangeAgain)
        {
            
        }

        internal virtual void RemoveCar(int index)
        {
            
        }
    }
}