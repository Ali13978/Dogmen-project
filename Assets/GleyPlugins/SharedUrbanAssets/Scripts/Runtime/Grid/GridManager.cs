using System.Collections.Generic;
using Unity.Collections;
#if USE_GLEY_PEDESTRIANS || USE_GLEY_TRAFFIC
using Unity.Mathematics;
#endif
using UnityEngine;

namespace GleyUrbanAssets
{
    public partial class GridManager : MonoBehaviour
    {
#if USE_GLEY_PEDESTRIANS || USE_GLEY_TRAFFIC
        protected CurrentSceneData currentSceneData;
        protected List<Vector2Int> activeCells;

        private List<Vector2Int> currentCells;

        private NativeArray<float3> activeCameraPositions;






        /// <summary>
        /// Initialize grid
        /// </summary>
        /// <typeparam name="T">Type of class that extends the grid manager</typeparam>
        /// <param name="currentSceneData">all waypoint information</param>
        /// <param name="activeCameraPositions">all active cameras</param>
        /// <returns></returns>
        internal GridManager Initialize(CurrentSceneData currentSceneData, NativeArray<float3> activeCameraPositions)
        {
            this.currentSceneData = currentSceneData;
            this.activeCameraPositions = activeCameraPositions;
            currentCells = new List<Vector2Int>();
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                currentCells.Add(new Vector2Int());
            }
            UpdateActiveCells(activeCameraPositions, 1);
            return this;
        }


#if USE_GLEY_TRAFFIC
        /// <summary>
        /// Update active grid cells. Only active grid cells are allowed to perform additional operations like updating traffic lights  
        /// </summary>
        internal void UpdateGrid(int level, NativeArray<float3> activeCameraPositions)
        {
            UpdateActiveCells(activeCameraPositions, level);
        }
#endif





        /// <summary>
        /// Get all specified neighbors for the specified depth
        /// </summary>
        /// <param name="row">current row</param>
        /// <param name="column">current column</param>
        /// <param name="depth">how far the cells should be</param>
        /// <param name="justEdgeCells">ignore middle cells</param>
        /// <returns>Returns the neighbors of the given cells</returns>
        internal List<Vector2Int> GetCellNeighbors(int row, int column, int depth, bool justEdgeCells)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            int rowMinimum = row - depth;
            if (rowMinimum < 0)
            {
                rowMinimum = 0;
            }

            int rowMaximum = row + depth;
            if (rowMaximum >= currentSceneData.grid.Length)
            {
                rowMaximum = currentSceneData.grid.Length - 1;
            }


            int columnMinimum = column - depth;
            if (columnMinimum < 0)
            {
                columnMinimum = 0;
            }

            int columnMaximum = column + depth;
            if (columnMaximum >= currentSceneData.grid[row].row.Length)
            {
                columnMaximum = currentSceneData.grid[row].row.Length - 1;
            }

            for (int i = rowMinimum; i <= rowMaximum; i++)
            {
                for (int j = columnMinimum; j <= columnMaximum; j++)
                {
                    if (justEdgeCells)
                    {
                        if (i == row + depth || i == row - depth || j == column + depth || j == column - depth)
                        {
                            result.Add(new Vector2Int(i, j));
                        }
                    }
                    else
                    {
                        result.Add(new Vector2Int(i, j));
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Convert indexes to Grid cell
        /// </summary>
        /// <param name="xPoz"></param>
        /// <param name="zPoz"></param>
        /// <returns></returns>
        internal GridCell GetCell(float xPoz, float zPoz)
        {
            return currentSceneData.GetCell(xPoz, zPoz);
        }

        /// <summary>
        /// Convert position to Grid cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal GridCell GetCell(Vector3 position)
        {
            return currentSceneData.GetCell(position);
        }

        /// <summary>
        /// Convert cell index to Grid cell
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        internal GridCell GetCell(Vector2Int cellIndex)
        {
            return currentSceneData.grid[cellIndex.x].row[cellIndex.y];
        }


        /// <summary>
        /// Get active cell for the active camera position
        /// </summary>
        /// <param name="activeCameraIndex"></param>
        /// <returns></returns>
        internal GridCell GetCell(int activeCameraIndex)
        {
            return GetCell(activeCameraPositions[activeCameraIndex].x, activeCameraPositions[activeCameraIndex].z);
        }

        /// <summary>
        /// Get position of the cell at index
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        internal Vector3 GetCellPosition(Vector2Int cellIndex)
        {
            return GetCell(cellIndex).center;
        }


        /// <summary>
        /// Convert position to cell index
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal Vector2Int GetCellIndex(Vector3 position)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((currentSceneData.gridCorner.z - position.z) / currentSceneData.gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((currentSceneData.gridCorner.x - position.x) / currentSceneData.gridCellSize));
            return new Vector2Int(currentSceneData.grid[rowIndex].row[columnIndex].row, currentSceneData.grid[rowIndex].row[columnIndex].column);
        }


        /// <summary>
        /// Update active cells based on player position
        /// </summary>
        /// <param name="activeCameraPositions">position to check</param>
        private void UpdateActiveCells(NativeArray<float3> activeCameraPositions, int level)
        {
            this.activeCameraPositions = activeCameraPositions;

            if (currentCells.Count != activeCameraPositions.Length)
            {
                currentCells = new List<Vector2Int>();
                for (int i = 0; i < activeCameraPositions.Length; i++)
                {
                    currentCells.Add(new Vector2Int());
                }
            }

            bool changed = false;
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                Vector2Int temp = GetCellIndex(activeCameraPositions[i]);
                if (currentCells[i] != temp)
                {
                    currentCells[i] = temp;
                    changed = true;
                }
            }

            if (changed)
            {
                activeCells = new List<Vector2Int>();
                for (int i = 0; i < activeCameraPositions.Length; i++)
                {
                    activeCells.AddRange(GetCellNeighbors(currentCells[i].x, currentCells[i].y, level, false));
                }
                UpdateActiveIntersections();
            }
        }

        partial void UpdateActiveIntersections();
#endif
    }
}
