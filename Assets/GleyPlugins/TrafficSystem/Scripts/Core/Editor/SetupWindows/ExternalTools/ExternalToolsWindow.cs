using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class ExternalToolsWindow : SetupWindowBase
    {
        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.Space();
            if (GUILayout.Button("Easy Roads"))
            {
                window.SetActiveWindow(typeof(EasyRoadsSetup), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Cidy 2"))
            {
                window.SetActiveWindow(typeof(CidySetup), true);
            }
            EditorGUILayout.Space();
        }
    }
}
