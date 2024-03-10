using GleyUrbanAssets;

namespace GleyTrafficSystem
{
    public class GridSetupWindow : GridSetupWindowBase
    {
        public override void DrawInScene()
        {
            if (viewGrid)
            {
                DrawGrid.Draw(grid.grid);
            }
            base.DrawInScene();
        }
    }
}
