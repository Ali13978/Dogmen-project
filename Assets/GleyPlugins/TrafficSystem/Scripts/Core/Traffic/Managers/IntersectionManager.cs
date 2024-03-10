using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Updates all intersections
    /// </summary>
    public class IntersectionManager : MonoBehaviour
    {
        private GenericIntersection[] allIntersections;
        private List<GenericIntersection> activeIntersections;
        private WaypointManager waypointManager;
        private bool debugIntersections;
        private bool stopIntersectionUpdate;
        float realtimeSinceStartup;


        /// <summary>
        /// Initialize intersection manager
        /// </summary>
        /// <param name="allIntersections"></param>
        /// <param name="activeIntersections"></param>
        /// <param name="waypointManager"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        /// <param name="debugIntersections"></param>
        /// <param name="stopIntersectionUpdate"></param>
        /// <returns></returns>
        public IntersectionManager Initialize(GenericIntersection[] allIntersections, List<GenericIntersection> activeIntersections, WaypointManager waypointManager, float greenLightTime, float yellowLightTime, bool debugIntersections, bool stopIntersectionUpdate)
        {
            IntersectionEvents.onActiveIntersectionsChanged += SetActiveIntersection;
            this.debugIntersections = debugIntersections;
            this.stopIntersectionUpdate = stopIntersectionUpdate;
            this.allIntersections = allIntersections;
            this.waypointManager = waypointManager;

            for (int i = 0; i < allIntersections.Length; i++)
            {
                allIntersections[i].Initialize(waypointManager, greenLightTime, yellowLightTime);
            }

            SetActiveIntersection(activeIntersections);
            return this;
        }


        /// <summary>
        /// Initialize all active intersections
        /// </summary>
        /// <param name="activeIntersections"></param>
        public void SetActiveIntersection(List<GenericIntersection> activeIntersections)
        {
            for (int i = 0; i < activeIntersections.Count; i++)
            {
                if (this.activeIntersections != null)
                {
                    if (!this.activeIntersections.Contains(activeIntersections[i]))
                    {
                        activeIntersections[i].ResetIntersection();
                    }
                }
            }
            this.activeIntersections = activeIntersections;
        }


        public void RemoveCarFromIntersection(int index)
        {
            for (int i = 0; i < activeIntersections.Count; i++)
            {
                activeIntersections[i].RemoveCar(index);
            }
        }


        /// <summary>
        /// Called on every frame to update active intersection road status
        /// </summary>
        public void UpdateIntersections()
        {
#if UNITY_EDITOR
            if (stopIntersectionUpdate)
                return;
#endif
            realtimeSinceStartup += Time.deltaTime;

            for (int i = 0; i < activeIntersections.Count; i++)
            {
                activeIntersections[i].UpdateIntersection(realtimeSinceStartup);
            }
        }

        internal void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            for (int i = 0; i < allIntersections.Length; i++)
            {
                allIntersections[i].SetTrafficLightsBehaviour(trafficLightsBehaviour);
            }
        }

        internal void SetRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain)
        {
            for(int i=0;i<allIntersections.Length;i++)
            {
                if (allIntersections[i].name == intersectionName)
                {
                    allIntersections[i].SetGreenRoad(roadIndex, doNotChangeAgain);
                }
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugIntersections)
            {
                for (int k = 0; k < allIntersections.Length; k++)
                {
                    List<IntersectionStopWaypointsIndex> stopWaypoints = allIntersections[k].GetWaypoints();
                    for (int i = 0; i < stopWaypoints.Count; i++)
                    {

                        for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                        {
                            if (waypointManager.GetWaypoint<Waypoint>(stopWaypoints[i].roadWaypoints[j]).stop == true)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawSphere(waypointManager.GetWaypoint<Waypoint>(stopWaypoints[i].roadWaypoints[j]).position, 1);
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}