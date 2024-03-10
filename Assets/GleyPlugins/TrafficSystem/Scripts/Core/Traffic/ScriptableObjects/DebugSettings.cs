using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Saves debug settings
    /// </summary>
    public class DebugSettings : ScriptableObject
    {
        public bool debug = false;
        public bool debugSpeed = false;
        public bool debugAI = false;
        public bool debugIntersections = false;
        public bool debugWaypoints = false;
        public bool debugDisabledWaypoints = false;
        public bool stopIntersectionUpdate = false;
        public bool drawBodyForces = false;
        public bool drawRaycasts = false;
        public bool debugDesnity = false;
    }
}