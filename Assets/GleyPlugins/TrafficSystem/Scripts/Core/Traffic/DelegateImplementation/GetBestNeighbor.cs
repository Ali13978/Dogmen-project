using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class GetBestNeighbor
    {
        static GridManager gridManager;


        /// <summary>
        /// The default behavior, a random square is chosen from the available ones 
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static int GetRandomSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType)
        {
#if USE_GLEY_TRAFFIC
            if (gridManager == null)
            {
                gridManager = UrbanManager.urbanManagerInstance.GetGridManager();
            }

            Vector2Int selectedNeighbor = neighbors[Random.Range(0, neighbors.Count)];

            ////get a random waypoint that supports the current vehicle
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetSpawnWaypointsForCell(selectedNeighbor, vehicleType);

            if (possibleWaypoints.Count > 0)
            {
                return possibleWaypoints[Random.Range(0, possibleWaypoints.Count)].waypointIndex;
            }
#endif

            return -1;
        }


        /// <summary>
        /// The square in front of the player is chosen
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static int GetForwardSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType)
        {
#if USE_GLEY_TRAFFIC
            if (gridManager == null)
            {
                gridManager = UrbanManager.urbanManagerInstance.GetGridManager();
            }

            Vector2Int selectedNeighbor = Vector2Int.zero;
            float angle = 180;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Vector3 cellDirection = gridManager.GetCellPosition(neighbors[i]) - position;
                float newAngle = Vector3.Angle(cellDirection, direction);
                if (newAngle < angle)
                {
                    selectedNeighbor = neighbors[i];
                    angle = newAngle;
                }
            }
            ////get a random waypoint that supports the current vehicle
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetSpawnWaypointsForCell(selectedNeighbor, vehicleType);
            if (possibleWaypoints.Count > 0)
            {
                return possibleWaypoints[Random.Range(0, possibleWaypoints.Count)].waypointIndex;
            }
#endif
            return -1;
        }
    }
}