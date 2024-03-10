namespace GleyUrbanAssets
{
    /// <summary>
    /// Draw grid in editor
    /// </summary>
    public partial class DrawGrid
    {
        public static void Draw(GridRow[] grid)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].row.Length; j++)
                {
                    DrawGridCell.Draw(grid[i].row[j], grid[i].row[j].waypointsInCell.Count != 0);
                }
            }
        }
    }
}