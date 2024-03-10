using System.Collections.Generic;

namespace GleyUrbanAssets
{
    /// <summary>
    /// This type of waypoint can spawn a vehicle, 
    /// used to store waypoint properties 
    /// </summary>
    [System.Serializable]
    public struct SpawnWaypoint
    {
        public int waypointIndex;
        public List<int> allowedVehicles;
        public SpawnWaypoint(int waypointIndex, List<int> allowedVehicles)
        {
            this.waypointIndex = waypointIndex;
            this.allowedVehicles = allowedVehicles;
        }
    }
}
