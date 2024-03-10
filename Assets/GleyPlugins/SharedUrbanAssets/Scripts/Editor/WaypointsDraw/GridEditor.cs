using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public partial class GridEditor : Editor
    {
        public static bool ApplySettings(CurrentSceneData currentSceneData)
        {
#if USE_GLEY_PEDESTRIANS
            if (AssignPedestrianWaypoints(currentSceneData) == false)
                return false;
#endif

#if USE_GLEY_TRAFFIC
            if (AssignTrafficWaypoints(currentSceneData) == false)
                return false;
#endif
            return true;
        }

        public static void GenerateGrid(CurrentSceneData currentSceneData)
        {
            System.DateTime startTime = System.DateTime.Now;
            int nrOfColumns;
            int nrOfRows;
            Bounds b = new Bounds();
            foreach (Renderer r in FindObjectsOfType<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }
            nrOfColumns = Mathf.CeilToInt(b.size.x / currentSceneData.gridCellSize);
            nrOfRows = Mathf.CeilToInt(b.size.z / currentSceneData.gridCellSize);
            if (nrOfRows == 0 || nrOfColumns == 0)
            {
                Debug.LogError("Your scene seems empty. Please add some geometry inside your scene before setting up traffic");
                return;
            }
            Debug.Log("Center: " + b.center + " size: " + b.size + " nrOfColumns " + nrOfColumns + " nrOfRows " + nrOfRows);
            Vector3 corner = new Vector3(b.center.x - b.size.x / 2 + currentSceneData.gridCellSize / 2, 0, b.center.z - b.size.z / 2 + currentSceneData.gridCellSize / 2);
            int nr = 0;
            currentSceneData.grid = new GridRow[nrOfRows];
            for (int row = 0; row < nrOfRows; row++)
            {
                currentSceneData.grid[row] = new GridRow(nrOfColumns);
                for (int column = 0; column < nrOfColumns; column++)
                {
                    nr++;
                    currentSceneData.grid[row].row[column] = new GridCell(column, row, new Vector3(corner.x + column * currentSceneData.gridCellSize, 0, corner.z + row * currentSceneData.gridCellSize), currentSceneData.gridCellSize);
                }
            }
            currentSceneData.gridCorner = currentSceneData.grid[0].row[0].center - currentSceneData.grid[0].row[0].size / 2;
            EditorUtility.SetDirty(currentSceneData);
            Debug.Log("Done generate grid in " + (System.DateTime.Now - startTime));
        }

        private static void SetTags()
        {
            ConnectionPool[] allWaypointHolders = FindObjectsOfType<ConnectionPool>();
            for (int i = 0; i < allWaypointHolders.Length; i++)
            {
                allWaypointHolders[i].gameObject.SetTag(Constants.editorTag);
            }
        }
    }
}
