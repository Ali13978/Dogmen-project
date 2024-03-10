using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class GridSetupWindowBase : SetupWindowBase
    {
        protected CurrentSceneData grid;
        private Color oldColor;
        protected bool viewGrid;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            grid = CurrentSceneData.GetSceneInstance();
            return base.Initialize(windowProperties, window);
        }


       


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("The grid is used to improve the performance. Moving agents are generated in the cells adjacent to player cell.\n\n" +
                "The cell size should be smaller if your player speed is low and should increase if your speed is high.\n\n" +
                "You can experiment with this settings until you get the result you want.");
        }


        protected override void ScrollPart(float width, float height)
        {
            grid.gridCellSize = EditorGUILayout.IntField("Grid Cell Size: ", grid.gridCellSize);
            if (GUILayout.Button("Regenerate Grid"))
            {
                GridEditor.GenerateGrid(grid);
            }
            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            if (viewGrid == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Grid"))
            {
                viewGrid = !viewGrid;
                SceneView.RepaintAll();
            }
            GUI.backgroundColor = oldColor;
            base.ScrollPart(width, height);
        }
    }
}
