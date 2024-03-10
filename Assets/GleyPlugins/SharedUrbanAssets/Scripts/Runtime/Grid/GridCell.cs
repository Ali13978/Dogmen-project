using System.Collections.Generic;
using UnityEngine;

namespace GleyUrbanAssets
{
    [System.Serializable]
    public class GridCell
    {
        public List<int> waypointsInCell;
        public List<int> pedestriansWaypointsInCell;
        public List<SpawnWaypoint> spawnWaypoints;
        public List<SpawnWaypoint> pedestrianSpawnWaypoints;
        public List<int> intersectionsInCell;
        public Vector3 center;
        public Vector3 size;
        public int row;
        public int column;


        /// <summary>
        /// Create a grid cell
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="center"></param>
        /// <param name="cellSize"></param>
        public GridCell(int column, int row, Vector3 center, int cellSize)
        {
            this.row = row;
            this.column = column;
            this.center = center;
            size = new Vector3(cellSize, 0, cellSize);
            ClearReferences();
            ClearPedestrianReferences();
        }


        /// <summary>
        /// Reset references
        /// </summary>
        public void ClearReferences()
        {
            waypointsInCell = new List<int>();         
            spawnWaypoints = new List<SpawnWaypoint>();         
            intersectionsInCell = new List<int>();
        }

        public void ClearPedestrianReferences()
        {
            pedestriansWaypointsInCell = new List<int>();
            pedestrianSpawnWaypoints = new List<SpawnWaypoint>();
        }


        /// <summary>
        /// Add a waypoint to grid cell
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <param name="name"></param>
        /// <param name="allowedCars"></param>
        public void AddWaypoint(int waypointIndex, string name, List<int> allowedCars, bool isInIntersection)
        {
            waypointsInCell.Add(waypointIndex);
            if (!name.Contains("Connect") && isInIntersection == false)
            {
                spawnWaypoints.Add(new SpawnWaypoint(waypointIndex, allowedCars));
            }
        }


        /// <summary>
        /// Add a waypoint to grid cell
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <param name="name"></param>
        /// <param name="allowedCars"></param>
        public void AddPedestrianWaypoint(int waypointIndex, string name, List<int> allowedPedestrians)
        {
            pedestriansWaypointsInCell.Add(waypointIndex);
            pedestrianSpawnWaypoints.Add(new SpawnWaypoint(waypointIndex, allowedPedestrians));
        }


        /// <summary>
        /// Add an intersection to grid cell
        /// </summary>
        /// <param name="intersection"></param>
        public void AddIntersection(int intersection)
        {
            if (!intersectionsInCell.Contains(intersection))
            {
                intersectionsInCell.Add(intersection);
            }
        }
    }
}
