using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Used to store intersection objects
    /// </summary>
    [System.Serializable]
    public class IntersectionStopWaypointsIndex
    {
        public List<int> roadWaypoints = new List<int>();
        public List<GameObject> redLightObjects;
        public List<GameObject> yellowLightObjects;
        public List<GameObject> greenLightObjects;

        public IntersectionStopWaypointsIndex(List<int> roadWaypoints, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects)
        {
            this.roadWaypoints = roadWaypoints;
            this.redLightObjects = redLightObjects;
            this.yellowLightObjects = yellowLightObjects;
            this.greenLightObjects = greenLightObjects;
        }
    }
}
