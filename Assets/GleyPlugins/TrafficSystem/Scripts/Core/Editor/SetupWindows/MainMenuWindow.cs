using GleyUrbanAssets;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class MainMenuWindow : SetupWindowBase
    {
        private const int scrollAdjustment = 103;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            return base.Initialize(windowProperties, window);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.Space();

            if (GUILayout.Button("Import Required Packages"))
            {
                window.SetActiveWindow(typeof(ImportPackagesWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Scene Setup"))
            {
                window.SetActiveWindow(typeof(SceneSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Road Setup"))
            {
                window.SetActiveWindow(typeof(RoadSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Intersection Setup"))
            {
                window.SetActiveWindow(typeof(IntersectionSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Waypoint Setup"))
            {
                window.SetActiveWindow(typeof(WaypointSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Speed Routes Setup"))
            {
                window.SetActiveWindow(typeof(SpeedRoutesSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Vehicle Routes Setup"))
            {
                window.SetActiveWindow(typeof(VehicleRoutesSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("External Tools"))
            {
                window.SetActiveWindow(typeof(ExternalToolsWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Debug"))
            {
                window.SetActiveWindow(typeof(DebugWindow), true);
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
            base.ScrollPart(width, height);
        }


        protected override void BottomPart()
        {
            if (GUILayout.Button("Apply Settings"))
            {
                if (LayerOperations.LoadOrCreateLayers<LayerSetup>(Constants.layerPath).edited == false)
                {
                    Debug.LogWarning("Layers are not configured. Go to Window->Gley->Traffic System->Scene Setup->Layer Setup");
                }

                if (GridEditor.ApplySettings(CurrentSceneData.GetSceneInstance()) == false)
                {
                    return;
                }

                if (!File.Exists(Application.dataPath + "/GleyPlugins/TrafficSystem/Resources/VehicleTypes.cs"))
                {
                    FileCreator.CreateVehicleTypesFile<VehicleTypes>(null, Gley.Common.Constants.USE_GLEY_TRAFFIC, Constants.trafficNamespace, Constants.agentTypesPath);
                }

                Gley.Common.PreprocessorDirective.AddToCurrent(Gley.Common.Constants.USE_GLEY_TRAFFIC, false);
            }
            EditorGUILayout.Space();

            base.BottomPart();
        }
    }
}
