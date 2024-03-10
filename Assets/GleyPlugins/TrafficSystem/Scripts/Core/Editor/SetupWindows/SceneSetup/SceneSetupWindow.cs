using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class SceneSetupWindow : SetupWindowBase
    {
        protected override void ScrollPart(float width, float height)
        {
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button("Layer Setup"))
            {
                window.SetActiveWindow(typeof(LayerSetupWindow), true);
            }
            EditorGUILayout.Space();


            if (GUILayout.Button("Grid Setup"))
            {
                window.SetActiveWindow(typeof(GridSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Car Type Setup"))
            {
                window.SetActiveWindow(typeof(VehicleTypesWindow), true);
            }
            EditorGUILayout.Space();

            base.ScrollPart(width, height);
        }
    }
}
