#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores traffic lights intersection properties
    /// </summary>
    public partial class TrafficLightsIntersectionSettings : GenericIntersectionSettings
    {
        public float greenLightTime=10;
        public float yellowLightTime=2;
        public List<IntersectionStopWaypointsSettings> stopWaypoints = new List<IntersectionStopWaypointsSettings>();


        public override int GetNumberOfRoads(int nrOfPeopleCrossings)
        {
            return stopWaypoints.Count-nrOfPeopleCrossings;
        }


        public override bool CheckAssignements(int roadsToIgnore)
        {
            bool passed = true;
            for (int i = 0; i < stopWaypoints.Count - roadsToIgnore; i++)
            {
                for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                {
                    if (stopWaypoints[i].roadWaypoints[j] == null)
                    {
                        passed = false;
                    }
                }

                if (stopWaypoints[i].redLightObjects.Count == 0)
                {
                    Debug.LogWarning("Intersection component from " + gameObject.name + " has no red lights on road " + (i + 1));
                }

                for (int j = 0; j < stopWaypoints[i].redLightObjects.Count; j++)
                {
                    if (stopWaypoints[i].redLightObjects[j] == null)
                    {
                        passed = false;
                    }
                }

                if (stopWaypoints[i].yellowLightObjects.Count == 0)
                {
                    Debug.LogWarning("Intersection component from " + gameObject.name + " has no yellow lights on road " + (i + 1));
                }

                for (int j = 0; j < stopWaypoints[i].yellowLightObjects.Count; j++)
                {
                    if (stopWaypoints[i].yellowLightObjects[j] == null)
                    {
                        passed = false;
                    }
                }

                if (stopWaypoints[i].greenLightObjects.Count == 0)
                {
                    Debug.LogWarning("Intersection component from " + gameObject.name + " has no green lights on road " + (i + 1));
                }

                for (int j = 0; j < stopWaypoints[i].greenLightObjects.Count; j++)
                {
                    if (stopWaypoints[i].greenLightObjects[j] == null)
                    {
                        passed = false;
                    }
                }
            }

            if (passed == false)
            {
                Debug.LogError("Intersection component from " + gameObject.name + " has invalid assignments");
            }
            return passed;
        }


        public override List<IntersectionStopWaypointsSettings> GetAssignedWaypoints()
        {
            return stopWaypoints;
        }


        public override List<WaypointSettings> GetExitWaypoints()
        {
            return new List<WaypointSettings>();
        }
    }
}
#endif
