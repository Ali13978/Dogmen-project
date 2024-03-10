#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores stop waypoints properties
    /// </summary>
    [System.Serializable]
    public class IntersectionStopWaypointsSettings
    {
        public List<WaypointSettings> roadWaypoints = new List<WaypointSettings>();
        public List<GameObject> redLightObjects = new List<GameObject>();
        public List<GameObject> yellowLightObjects = new List<GameObject>();
        public List<GameObject> greenLightObjects = new List<GameObject>();
        public bool draw = true;
    }
}
#endif