#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores priority intersection properties
    /// </summary>
    public class PriorityIntersectionSettings : GenericIntersectionSettings
    {
        public List<IntersectionStopWaypointsSettings> enterWaypoints = new List<IntersectionStopWaypointsSettings>();
        public List<WaypointSettings> exitWaypoints = new List<WaypointSettings>();


        public override int GetNumberOfRoads(int nrOfPeopleCrossings)
        {
            return enterWaypoints.Count - nrOfPeopleCrossings;
        }


        public override bool CheckAssignements(int roadsToIgnore)
        {
            bool passed = true;
            for (int i = 0; i < enterWaypoints.Count; i++)
            {
                for (int j = 0; j < enterWaypoints[i].roadWaypoints.Count; j++)
                {
                    if (enterWaypoints[i].roadWaypoints[j] == null)
                    {
                        passed = false;
                    }
                }
            }

            if (exitWaypoints.Count == 0)
            {
                passed = false;
            }

            for (int i = 0; i < exitWaypoints.Count; i++)
            {
                if (exitWaypoints[i] == null)
                {
                    passed = false;
                }
            }

            if (passed == false)
            {
                Debug.LogError("Intersection component from " + gameObject.name + " has invalid assignments", gameObject);
            }
            return passed;
        }


        public override List<IntersectionStopWaypointsSettings> GetAssignedWaypoints()
        {
            return enterWaypoints;
        }


        public override List<WaypointSettings> GetExitWaypoints()
        {
            return exitWaypoints;
        }
    }
}
#endif