using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Add this component on a GameObject inside your scene to enable Traffic System
    /// </summary>
    public class TrafficComponentMultiplayer : MonoBehaviour
    {
        [Tooltip("Player is used to instantiate vehicles out of view")]
        public Transform[] players;
        [Tooltip("Max number of active vehicles")]
        public int nrOfVehicles = 1;
        [Tooltip("List of different vehicles (Right Click->Create->Traffic System->Vehicle Pool)")]
        public VehiclePool vehiclePool;
        [Tooltip("Minimum distance from the player where a vehicle can be instantiated")]
        public float minDistanceToAdd = 100;
        [Tooltip("Distance from the player where a vehicle can be removed")]
        public float distanceToRemove = 150;
        [Tooltip("How long yellow light is on (if = -1 the value from the intersection component will be used)")]
        public float yellowLightTime = -1;
        [Tooltip("How long green light is on (if = -1 the value from the intersection component will be used)")]
        public float greenLightTime = -1;

        void Start()
        {
            Manager.Initialize(players, nrOfVehicles, vehiclePool, minDistanceToAdd, distanceToRemove, 1, greenLightTime, yellowLightTime);
        }
    }
}
