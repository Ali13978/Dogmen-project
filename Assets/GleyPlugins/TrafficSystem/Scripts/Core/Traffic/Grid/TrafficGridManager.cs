using GleyTrafficSystem;
using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
#if  USE_GLEY_TRAFFIC
using Unity.Mathematics;
#endif

namespace GleyUrbanAssets
{
    public partial class GridManager : MonoBehaviour
    {
#if USE_GLEY_TRAFFIC
        private List<GenericIntersection> activeIntersections = new List<GenericIntersection>();

        private SpawnWaypointSelector spawnWaypointSelector;
        private SpawnWaypointSelector SpawnWaypointSelector
        {
            get
            {
                if (spawnWaypointSelector == null)
                {
                    spawnWaypointSelector = GetBestNeighbor.GetRandomSpawnWaypoint;
                }
                return spawnWaypointSelector;
            }
        }

        /// <summary>
        /// Should be overriden in derived class
        /// </summary>
        /// <param name="neighbors">cell neighbors</param>
        /// <param name="playerPosition">position of the player</param>
        /// <param name="playerDirection">heading of the player</param>
        /// <param name="carType">type of car to instantiate</param>
        /// <returns></returns>
        protected int ApplyNeighborSelectorMethod(List<Vector2Int> neighbors, Vector3 playerPosition, Vector3 playerDirection, VehicleTypes carType)
        {
            try
            {
                return SpawnWaypointSelector(neighbors, playerPosition, playerDirection, carType);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Your neighbor selector method has the following error: " + e.Message);
                return GetBestNeighbor.GetRandomSpawnWaypoint(neighbors, playerPosition, playerDirection, carType);
            }
        }


        /// <summary>
        /// Set the default waypoint generating method
        /// </summary>
        /// <param name="spawnWaypointSelector"></param>
        internal void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelector)
        {
            this.spawnWaypointSelector = spawnWaypointSelector;
        }


        

        #region Intersections
#if USE_GLEY_TRAFFIC
        /// <summary>
        /// Get active all intersections 
        /// </summary>
        /// <returns></returns>
        internal List<GenericIntersection> GetActiveIntersections()
        {
            return activeIntersections;
        }


        /// <summary>
        /// Create a list of active intersections
        /// </summary>
        partial void UpdateActiveIntersections()
        {
            List<int> intersectionIndexes = new List<int>();
            for (int i = 0; i < activeCells.Count; i++)
            {
                intersectionIndexes.AddRange(GetCell(activeCells[i]).intersectionsInCell.Except(intersectionIndexes));
            }

            List<GenericIntersection> result = new List<GenericIntersection>();
            for (int i = 0; i < intersectionIndexes.Count; i++)
            {
                switch (currentSceneData.allIntersections[intersectionIndexes[i]].type)
                {
                    case IntersectionType.TrafficLights:
                        result.Add(currentSceneData.allLightsIntersections[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                    case IntersectionType.Priority:
                        result.Add(currentSceneData.allPriorityIntersections[currentSceneData.allIntersections[intersectionIndexes[i]].index]);
                        break;
                }
            }

            if (activeIntersections.Count == result.Count && activeIntersections.All(result.Contains))
            {

            }
            else
            {
                activeIntersections = result;
                IntersectionEvents.TriggetActiveIntersectionsChangedEvent(activeIntersections);
            }
        }


        /// <summary>
        /// Return all intersections
        /// </summary>
        /// <returns></returns>
        internal GenericIntersection[] GetAllIntersections()
        {
            GenericIntersection[] result = new GenericIntersection[currentSceneData.allIntersections.Length];
            for (int i = 0; i < currentSceneData.allIntersections.Length; i++)
            {
                switch (currentSceneData.allIntersections[i].type)
                {
                    case IntersectionType.TrafficLights:
                        result[i] = currentSceneData.allLightsIntersections[currentSceneData.allIntersections[i].index];
                        break;
                    case IntersectionType.Priority:
                        result[i] = currentSceneData.allPriorityIntersections[currentSceneData.allIntersections[i].index];
                        break;
                }
            }
            return result;
        }
#endif
        #endregion

        internal int GetNeighborCellWaypoint(int row, int column, int depth, VehicleTypes carType, Vector3 playerPosition, Vector3 playerDirection)
        {
            //get all cell neighbors for the specified depth
            List<Vector2Int> neighbors = GetCellNeighbors(row, column, depth, true);

            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (currentSceneData.grid[neighbors[i].x].row[neighbors[i].y].spawnWaypoints.Count == 0)
                {
                    neighbors.RemoveAt(i);
                }
            }

            //if neighbors exists
            if (neighbors.Count > 0)
            {
                return ApplyNeighborSelectorMethod(neighbors, playerPosition, playerDirection, carType);
            }
            return -1;
        }

        internal List<SpawnWaypoint> GetSpawnWaypointsForCell(Vector2Int cellIndex, VehicleTypes agentType)
        {
            List<SpawnWaypoint> spawnWaypoints = currentSceneData.grid[cellIndex.x].row[cellIndex.y].spawnWaypoints;

            return currentSceneData.grid[cellIndex.x].row[cellIndex.y].spawnWaypoints.Where(cond1 => cond1.allowedVehicles.Contains((int)agentType)).ToList();
        }
#endif
    }
}
