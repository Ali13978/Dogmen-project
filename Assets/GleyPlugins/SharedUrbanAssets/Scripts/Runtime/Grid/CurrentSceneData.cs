using UnityEngine;

namespace GleyUrbanAssets
{
    /// <summary>
    /// Stores all references 
    /// </summary>
    //TODO this class should be just for storing values
    public partial class CurrentSceneData : MonoBehaviour
    {
        public int gridCellSize = 50;
        public Vector3 gridCorner;
        public GridRow[] grid;



        /// <summary>
        /// Get scene data object from active scene 
        /// </summary>
        /// <returns></returns>
        public static CurrentSceneData GetSceneInstance()
        {
            CurrentSceneData[] allSceneGrids = FindObjectsOfType<CurrentSceneData>();
            if (allSceneGrids.Length > 1)
            {
                Debug.LogError("Multiple Grid components exists in scene. Just one is required, delete extra components before continuing.");
                for (int i = 0; i < allSceneGrids.Length; i++)
                {
                    Debug.LogWarning("Grid component exists on: " + allSceneGrids[i].name, allSceneGrids[i]);
                }
            }

            if (allSceneGrids.Length == 0)
            {
                GameObject go = new GameObject(Constants.gleyTrafficHolderName);
                CurrentSceneData grid = go.AddComponent<CurrentSceneData>();
                return grid;
            }
            return allSceneGrids[0];
        }

        /// <summary>
        /// Convert position to Grid cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GridCell GetCell(Vector3 position)
        {
            return GetCell(position.x, position.z);
        }


        /// <summary>
        /// Convert indexes to Grid cell
        /// </summary>
        /// <param name="xPoz"></param>
        /// <param name="zPoz"></param>
        /// <returns></returns>
        public GridCell GetCell(float xPoz, float zPoz)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.z - zPoz) / gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.x - xPoz) / gridCellSize));
            return grid[rowIndex].row[columnIndex];
        }
    }
}
