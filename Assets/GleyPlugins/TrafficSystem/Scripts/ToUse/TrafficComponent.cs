using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Add this component on a GameObject inside your scene to enable Traffic System
    /// </summary>
    public class TrafficComponent : MonoBehaviour
    {
        [Tooltip("Player is used to instantiate vehicles out of view")]
        public Transform player;
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
            Manager.Initialize(player, nrOfVehicles, vehiclePool, minDistanceToAdd, distanceToRemove, greenLightTime, yellowLightTime);
            //Uncomment this and a new traffic car will be added in front of your car most of the time
            //Manager.SetSpawnWaypointSelectorDelegate(GetBestNeighbor.GetForwardSpawnWaypoint);
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        Manager.AddVehicle(player.position, VehicleTypes.Car);
        //        Manager.RemoveVehicle(0);
        //    }
        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        Manager.AddVehicle(player.position, VehicleTypes.Car);
        //        Manager.RemoveVehicle(1);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        Manager.SetIntersectionRoadToGreen("Intersection_2", 1, true);
        //    }
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        Manager.SetIntersectionRoadToGreen("Intersection_2", 2, true);
        //    }
        //}
    }
}
