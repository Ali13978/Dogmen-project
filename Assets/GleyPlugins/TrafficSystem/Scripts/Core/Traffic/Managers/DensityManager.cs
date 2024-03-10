using GleyUrbanAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
#if USE_GLEY_TRAFFIC
using Unity.Mathematics;
#endif
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Controls the number of active vehicles
    /// </summary>
    public class DensityManager : MonoBehaviour
    {
        private PositionValidator positionValidator;
        private GridManager gridManager;
#if USE_GLEY_TRAFFIC
        private TrafficVehicles trafficVehicles;
        private WaypointManager waypointManager;
        private int maxNrOfVehicles;
        private int currentnrOfVehicles;


        private bool newVehicleRequested;



        /// <summary>
        /// Initializes the density manager script
        /// </summary>
        /// <param name="trafficVehicles"></param>
        /// <param name="waypointManager"></param>
        /// <param name="currentSceneData"></param>
        /// <param name="activeCameras"></param>
        /// <param name="maxNrOfVehicles">Is the maximum allowed number of vehicles in the current scene. It cannot be increased later</param>
        /// <returns></returns>
        public DensityManager Initialize(TrafficVehicles trafficVehicles, WaypointManager waypointManager, GridManager gridManager, PositionValidator positionValidator, NativeArray<float3> activeCameraPositions, int maxNrOfVehicles, Vector3 playerPosition, Vector3 playerDirection)
        {
            this.positionValidator = positionValidator;
            this.trafficVehicles = trafficVehicles;
            this.waypointManager = waypointManager;

            this.gridManager = gridManager;
            this.maxNrOfVehicles = maxNrOfVehicles;
            List<GridCell> gridCells = new List<GridCell>();
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                gridCells.Add(gridManager.GetCell(activeCameraPositions[i].x, activeCameraPositions[i].z));
            }

            LoadInitialVehicles(gridCells, playerPosition, playerDirection);

            return this;
        }


        /// <summary>
        /// Change vehicle density
        /// </summary>
        /// <param name="nrOfVehciles">cannot be greater than max vehicle number set on initialize</param>
        public void UpdateMaxCars(int nrOfVehciles)
        {
            maxNrOfVehicles = nrOfVehciles;
        }





        /// <summary>
        /// Ads new vehicles if required
        /// </summary>
        public void UpdateVehicleDensity(Vector3 playerPosition, Vector3 playerDirection, int activeCameraIndex)
        {
            if (currentnrOfVehicles < maxNrOfVehicles)
            {
                GridCell gridCell = gridManager.GetCell(activeCameraIndex);
                AddVehicle(false, gridCell.row, gridCell.column, playerPosition, playerDirection);
            }
        }

        /// <summary>
        /// Remove a vehicle if required
        /// </summary>
        /// <param name="index">vehicle to remove</param>
        /// <param name="force">remove the vehicle even if not all conditions for removing are met</param>
        /// <returns>true if a vehicle was really removed</returns>
        public void RemoveVehicle(int index)
        {
            currentnrOfVehicles--;
            waypointManager.RemoveTargetWaypoint(index);
            AIEvents.TriggerVehicleChangedStateEvent(index, trafficVehicles.GetCollider(index), SpecialDriveActionTypes.Destroyed);
            trafficVehicles.DisableVehicle(index);

            //Debug.Log("REMOVE CARS FROM INTERSECTION HERE " + index);
        }


        /// <summary>
        /// Update the active camera used to determine if a vehicle is in view
        /// </summary>
        /// <param name="activeCamerasPosition"></param>
        public void UpdateCameraPositions(Transform[] activeCameras)
        {
            positionValidator.UpdateCamera(activeCameras);
        }


        /// <summary>
        /// Add all vehicles around the player even if they are inside players view
        /// </summary>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        private void LoadInitialVehicles(List<GridCell> gridCells, Vector3 playerPosition, Vector3 playerDirection)
        {
            for (int i = 0; i < maxNrOfVehicles; i++)
            {
#if DEBUG_TRAFFIC
                AddVehicleDebug(i);
#else
                int cellIndex = UnityEngine.Random.Range(0, gridCells.Count);
                AddVehicle(true, gridCells[cellIndex].row, gridCells[cellIndex].column, playerPosition, playerDirection);
#endif
            }
        }


        /// <summary>
        /// Trying to load an idle vehicle if exists
        /// </summary>
        /// <param name="firstTime">initial load</param>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        private void AddVehicle(bool firstTime, int currentGridRow, int currentGridColumn, Vector3 playerPosition, Vector3 playerDirection)
        {
            //TODO Optimize this process to never fail to add a new vehicle if requested
            int idleVehicleIndex = trafficVehicles.GetIdleVehicleIndex();

            //if an idle vehicle exists
            if (idleVehicleIndex != -1)
            {
                int freeWaypointIndex = GetFreeWaypointToInstantiate(firstTime, currentGridRow, currentGridColumn, trafficVehicles.GetVehicleType(idleVehicleIndex), trafficVehicles.GetHalfVehicleLength(idleVehicleIndex), trafficVehicles.GetVehicleHeight(idleVehicleIndex), trafficVehicles.GetVehicleWidth(idleVehicleIndex), trafficVehicles.GetFrontWheelOffset(idleVehicleIndex), playerPosition, playerDirection);

                //if a free waypoint exists
                if (freeWaypointIndex != -1)
                {
                    VehicleComponent vehicle = trafficVehicles.GetIdleVehicle(idleVehicleIndex);
                    //if a vehicle is suitable for that waypoint
                    if (vehicle)
                    {
                        currentnrOfVehicles++;
                        int index = vehicle.GetIndex();
                        waypointManager.SetTargetWaypoint(index, freeWaypointIndex);
                        trafficVehicles.ActivateVehicle(vehicle, waypointManager.GetTargetWaypointPosition(index), waypointManager.GetTargetWaypointRotation(index));
                        DensityEvents.TriggerVehicleAddedEvent(index);
                    }
                }
            }
        }


        /// <summary>
        /// Get a random free waypoint from a grid cell
        /// </summary>
        /// <param name="firstTime"></param>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        /// <param name="carType"></param>
        /// <param name="halfCarLength"></param>
        /// <param name="halfCarHeight"></param>
        /// <returns></returns>
        int GetFreeWaypointToInstantiate(bool firstTime, int currentGridRow, int currentGridColumn, VehicleTypes carType, float halfCarLength, float halfCarHeight, float halfCarWidth, float frontWheelOffset, Vector3 playerPosition, Vector3 playerDirection)
        {
            int freeWaypointIndex;

            //get a free waypoint with the specified characteristics
            if (firstTime)
            {
                freeWaypointIndex = gridManager.GetNeighborCellWaypoint(currentGridRow, currentGridColumn, UnityEngine.Random.Range(0, 2), carType, playerPosition, playerDirection);
            }
            else
            {
                freeWaypointIndex = gridManager.GetNeighborCellWaypoint(currentGridRow, currentGridColumn, 1, carType, playerPosition, playerDirection);
            }

            if (freeWaypointIndex != -1)
            {
                //if a valid waypoint was found, check if it was not manually disabled
                if (waypointManager.IsDisabled(freeWaypointIndex))
                {
                    return -1;
                }

                //check if the car type can be instantiated on selected waypoint
                if (positionValidator.IsValid(waypointManager.GetWaypointPosition(freeWaypointIndex), halfCarLength, halfCarHeight, halfCarWidth, firstTime, frontWheelOffset, waypointManager.GetOrientation(freeWaypointIndex)))
                {
                    return freeWaypointIndex;
                }
            }

            return -1;
        }

        internal bool IsWaypointFree(int waypointIndex, float halfCarLength, float halfCarHeight, float halfCarWidth, float frontWheelOffset)
        {
            return positionValidator.IsValid(waypointManager.GetWaypointPosition(waypointIndex), halfCarLength, halfCarHeight, halfCarWidth, true, frontWheelOffset, waypointManager.GetOrientation(waypointIndex));
        }
        public void AddVehicleAtPosition(Vector3 position, VehicleTypes type)
        {
            if (newVehicleRequested == false)
            {
                newVehicleRequested = true;
                StartCoroutine(WaitForAddVehicle(position, type));
            }
        }

        /// <summary>
        /// Makes waypoints on a given radius unavailable
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void DisableAreaWaypoints(Vector3 center, float radius)
        {
            GridCell cell = gridManager.GetCell(center);
            List<Vector2Int> neighbors = gridManager.GetCellNeighbors(cell.row, cell.column, 1, false);
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                cell = gridManager.GetCell(neighbors[i]);
                for (int j = 0; j < cell.waypointsInCell.Count; j++)
                {
                    Waypoint waypoint = waypointManager.GetWaypoint<Waypoint>(cell.waypointsInCell[j]);
                    if (Vector3.SqrMagnitude(center - waypoint.position) < radius)
                    {
                        waypointManager.AddDisabledWaypoints(waypoint);
                    }
                }
            }
        }


        internal int GetClosestWayoint(Vector3 position, VehicleTypes type)
        {
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetCell(position.x, position.z).spawnWaypoints.Where(cond1 => cond1.allowedVehicles.Contains((int)type)).ToList();

            if (possibleWaypoints.Count == 0)
                return -1;

            float distance = float.MaxValue;
            int waypointIndex = -1;
            for (int i = 0; i < possibleWaypoints.Count; i++)
            {
                float newDistance = Vector3.SqrMagnitude(waypointManager.GetWaypointPosition(possibleWaypoints[i].waypointIndex) - position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    waypointIndex = possibleWaypoints[i].waypointIndex;
                }
            }
            return waypointIndex;
        }

        IEnumerator WaitForAddVehicle(Vector3 position, VehicleTypes type)
        {
            int idleVehicleIndex = trafficVehicles.GetIdleVehicleIndex();
            while (idleVehicleIndex == -1)
            {
                //Debug.Log("Wait for free vehicle");
                yield return null;
                idleVehicleIndex = trafficVehicles.GetIdleVehicleIndexOfType(type);
            }



            //get closest waypoint
            int waypointIndex = GetClosestWayoint(position, type);
            if (waypointIndex == -1)
            {
                Debug.Log("No waypoints found");
            }
            else
            {
                VehicleComponent vehicle = trafficVehicles.GetIdleVehicle(idleVehicleIndex);
                //Debug.Log("Check for free waypoint");
                bool isWaypointFree = IsWaypointFree(waypointIndex, vehicle.length / 2, vehicle.coliderHeight, vehicle.colliderWidth / 2, vehicle.frontTrigger.localPosition.z);
                while (isWaypointFree == false)
                {

                    //Debug.Log("Waypoint not free");
                    yield return null;
                    isWaypointFree = IsWaypointFree(waypointIndex, vehicle.length / 2, vehicle.coliderHeight, vehicle.colliderWidth / 2, vehicle.frontTrigger.localPosition.z);
                }

                //Debug.Log("All done -> Add vehicle");

                //if a vehicle is suitable for that waypoint
                if (vehicle)
                {
                    currentnrOfVehicles++;
                    int index = vehicle.GetIndex();
                    waypointManager.SetTargetWaypoint(index, waypointIndex);
                    trafficVehicles.ActivateVehicle(vehicle, waypointManager.GetTargetWaypointPosition(index), waypointManager.GetTargetWaypointRotation(index));
                    DensityEvents.TriggerVehicleAddedEvent(index);
                    //Debug.Log("Vehicle added at position -> DONE");
                }
            }
            newVehicleRequested = false;
        }


#if DEBUG_TRAFFIC
        private void AddVehicleDebug(int waypointIndex)
        {
            int idleVehicleIndex = trafficVehicles.GetIdleVehicleIndex();

            if (idleVehicleIndex != -1)
            {
                VehicleComponent vehicle = trafficVehicles.GetIdleVehicle(idleVehicleIndex);
                if (vehicle)
                {
                    currentnrOfVehicles++;
                    int index = vehicle.GetIndex();
                    waypointManager.SetTargetWaypoint(index, waypointIndex);
                    trafficVehicles.ActivateVehicle(vehicle, waypointManager.GetTargetPosition(index), waypointManager.GetTargetRotation(index));
                    DensityEvents.TriggerVehicleAddedEvent(index, waypointManager.GetTargetPosition(index), trafficVehicles.GetMaxSpeed(index), trafficVehicles.GetPowerStep(index), trafficVehicles.GetBrakeStep(index));
                }
            }
        }
#endif
#endif
    }

}