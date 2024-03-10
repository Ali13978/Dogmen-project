using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    public static class Manager
    {
        /// <summary>
        /// Initialize the traffic
        /// </summary>
        /// <param name="activeCamera">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool carPool, float minDistanceToAdd, float distanceToRemove)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.Initialize(new Transform[] { activeCamera }, nrOfVehicles, carPool, minDistanceToAdd, distanceToRemove, 1, -1, -1);
#endif
        }


        /// <summary>
        /// Initialize the traffic
        /// </summary>
        /// <param name="activeCamera">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="masterVolume">[-1,1] used to control the engine sound from your master volume</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool carPool, float minDistanceToAdd, float distanceToRemove, float masterVolume)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.Initialize(new Transform[] { activeCamera }, nrOfVehicles, carPool, minDistanceToAdd, distanceToRemove, masterVolume, -1, -1);
#endif
        }


        /// <summary>
        /// Initialize the traffic
        /// </summary>
        /// <param name="activeCamera">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="greenLightTime">roads green light duration in seconds</param>
        /// <param name="yelloLightTime">roads yellow light duration in seconds</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool carPool, float minDistanceToAdd, float distanceToRemove, float greenLightTime, float yelloLightTime)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.Initialize(new Transform[] { activeCamera }, nrOfVehicles, carPool, minDistanceToAdd, distanceToRemove, 1, greenLightTime, yelloLightTime);
#endif
        }


        /// <summary>
        /// Initialize the traffic
        /// </summary>
        /// <param name="activeCamera">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="masterVolume">[-1,1] used to control the engine sound from your master volume</param>
        /// <param name="greenLightTime">roads green light duration in seconds</param>
        /// <param name="yelloLightTime">roads yellow light duration in seconds</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool carPool, float minDistanceToAdd, float distanceToRemove, float masterVolume, float greenLightTime, float yelloLightTime)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.Initialize(new Transform[] { activeCamera }, nrOfVehicles, carPool, minDistanceToAdd, distanceToRemove, masterVolume, greenLightTime, yelloLightTime);
#endif
        }


        /// <summary>
        /// Initialize the traffic
        /// </summary>
        /// <param name="activeCameras">cameras that follows the player in multiplayer/split screen setup</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="masterVolume">[-1,1] used to control the engine sound from your master volume</param>
        /// <param name="greenLightTime">roads green light duration in seconds</param>
        /// <param name="yelloLightTime">roads yellow light duration in seconds</param>
        public static void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool carPool, float minDistanceToAdd, float distanceToRemove, float masterVolume, float greenLightTime, float yelloLightTime)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.Initialize(activeCameras, nrOfVehicles, carPool, minDistanceToAdd, distanceToRemove, masterVolume, greenLightTime, yelloLightTime);
#endif
        }


        /// <summary>
        /// Update active camera that is used to remove vehicles when are not in view
        /// </summary>
        /// <param name="activeCamera">represents the camera or the player prefab</param>
        public static void SetCamera(Transform activeCamera)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.UpdateCamera(new Transform[] { activeCamera });
#endif
        }


        /// <summary>
        /// Update active cameras that is used to remove vehicles when are not in view
        /// this is used in multiplayer/split screen setups
        /// </summary>
        /// <param name="activeCameras">represents the cameras or the players from your game</param>
        public static void SetCameras(Transform[] activeCameras)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.UpdateCamera(activeCameras);
#endif
        }


        /// <summary>
        /// Modify max number of active vehicles
        /// </summary>
        /// <param name="nrOfVehicles">new max number of vehicles, needs to be less than the initialization max number of vehicles</param>
        public static void SetTrafficDensity(int nrOfVehicles)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetTrafficDensity(nrOfVehicles);
#endif
        }


        /// <summary>
        /// Remove all vehicles on a range
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public static void ClearTrafficOnArea(Vector3 center, float radius)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.ClearTrafficOnArea(center, radius);
#endif
        }


        /// <summary>
        /// Disable all waypoints on the specified area to stop vehicles to go in a certain area for a limited amount of time
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public static void DisableAreaWaypoints(Vector3 center, float radius)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.DisableAreaWaypoints(center, radius);
#endif
        }


        /// <summary>
        /// Enable all disabled area waypoints
        /// </summary>
        public static void EnableAllWaypoints()
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.EnableAllWaypoints();
#endif
        }


        /// <summary>
        /// Turn all vehicle lights on or off
        /// </summary>
        /// <param name="on">if true, lights are on</param>
        public static void UpdateVehicleLights(bool on)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.UpdateVehicleLights(on);
#endif
        }


        /// <summary>
        /// Control the engine volume from your master volume
        /// </summary>
        /// <param name="volume">current engine AudioSource volume</param>
        public static void SetEngineVolume(float volume)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetMasterVolume(volume);
#endif
        }


        /// <summary>
        /// Set how far away active intersections should be -> default is 1
        /// If set is to 2 -> intersections will update on a 2 square distance from the player
        /// </summary>
        /// <param name="level">how many squares away should intersections be updated</param>
        public static void SetActiveSquares(int level)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetActiveSquaresLevel(level);
#endif
        }


        /// <summary>
        /// Remove a specific vehicle from scene
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle to remove</param>
        public static void RemoveVehicle(int vehicleIndex)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.RemoveVehicle(vehicleIndex);
#endif
        }


        /// <summary>
        /// Remove a specific vehicle from scene
        /// </summary>
        /// <param name="vehicleIndex">root GameObject of the vehicle to remove</param>
        public static void RemoveVehicle(GameObject vehicle)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.RemoveVehicle(vehicle);
#endif
        }


        /// <summary>
        /// Add a traffic vehicle to the closest waypoint from the given position
        /// This method will wait until that vehicle type will be available and the closest waypoint will be free to add a new vehicle on it.
        /// The method will run in background until the new vehicle is added.
        /// </summary>
        /// <param name="position">the position where to add a new vehicle</param>
        /// <param name="vehicleType">the type of vehicle to add</param>
        public static void AddVehicle(Vector3 position, VehicleTypes vehicleType)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.AddVehicle(position, vehicleType);
#endif
        }


        /// <summary>
        /// When this method is called, the vehicle passed as param is no longer controlled by the traffic system 
        /// until it is out of view and respawned
        /// </summary>
        /// <param name="vehicle"></param>
        public static void StopVehicleDriving(GameObject vehicle)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.StopVehicleDriving(vehicle);
#endif
        }


        /// <summary>
        /// If a vehicle sees a collider and that collider is destroyed from other script, trigger exit method is not fired, so it is needed 
        /// to be manually triggered using this method to remove the obstacle in front of the traffic vehicle
        /// </summary>
        /// <param name="collider"></param>
        public static void TriggerColliderRemovedEvent(Collider collider)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.TriggerColliderRemovedEvent(collider);
#endif
        }


        /// <summary>
        /// Force a road from an intersection to change to green
        /// </summary>
        /// <param name="intersectionName">name of the intersection to change</param>
        /// <param name="roadIndex">the road index to change</param>
        /// <param name="doNotChangeAgain">if true that road will stay green until this param is set back to false</param>
        public static void SetIntersectionRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain = false)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetIntersectionRoadToGreen(intersectionName, roadIndex, doNotChangeAgain);
#endif
        }

#if USE_GLEY_TRAFFIC

        /// <summary>
        /// Get all vehicles
        /// </summary>
        /// <returns>List with all vehicle components</returns>
        public static List<VehicleComponent> GetVehicleList()
        {
            return TrafficManager.Instance.GetVehicleList();
        }
#endif

        #region Delegates
        public static void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetTrafficLightsBehaviour(trafficLightsBehaviour);
#endif
        }


        /// <summary>
        /// Use to change the way a new traffic car is placed. 
        /// By default a random square around the player is used and a random waypoint from it is selected, 
        /// but it can be change to any method you want. 
        /// Currently there is another method that always selects the tile in front of the player:
        /// Manager.SetSpawnWaypointSelectorDelegate(GetBestNeighbor.GetForwardSpawnWaypoint);
        /// </summary>
        /// <param name="spawnWaypointSelector">pass your own method and it will be used for waypoint selection</param>
        public static void SetSpawnWaypointSelectorDelegate(SpawnWaypointSelector spawnWaypointSelector)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetSpawnWaypointSelectorDelegate(spawnWaypointSelector);
#endif
        }


        /// <summary>
        /// Set new action to use when a traffic vehicle sees the player
        /// </summary>
        /// <param name="playerInteraction"></param>
        public static void SetPlayerInteractionDelegate(EnvironmentInteraction playerInteraction)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetPlayerInteractionDelegate(playerInteraction);
#endif
        }


        /// <summary>
        /// Set new action to use when a traffic vehicle sees a building
        /// </summary>
        /// <param name="buildingInteraction"></param>
        public static void SetBuildingInteractionDelegate(EnvironmentInteraction buildingInteraction)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetBuildingInteractionDelegate(buildingInteraction);
#endif
        }


        /// <summary>
        /// Set new action to use when a traffic vehicle sees a dynamic object
        /// </summary>
        /// <param name="dynamicObjectInteraction"></param>
        public static void SetDynamicObjectInteractionDelegate(EnvironmentInteraction dynamicObjectInteraction)
        {
#if USE_GLEY_TRAFFIC
            TrafficManager.Instance.SetDynamicObjectInteractionDelegate(dynamicObjectInteraction);
#endif
        }

        #endregion
    }
}