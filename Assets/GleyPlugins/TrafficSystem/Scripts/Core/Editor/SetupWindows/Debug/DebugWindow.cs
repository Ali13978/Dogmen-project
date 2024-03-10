using GleyUrbanAssets;
using UnityEditor;

namespace GleyTrafficSystem
{
    public class DebugWindow : SetupWindowBase
    {
        DebugSettings save;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            save = DebugOptions.LoadOrCreateDebugSettings();
            return base.Initialize(windowProperties,window);
        }


        protected override void TopPart()
        {
            save.debug = EditorGUILayout.Toggle("Debug Vehicle Actions", save.debug);
            if(save.debug == false)
            {
                save.debugSpeed = false;
                save.debugAI = false;

            }
            save.debugSpeed = EditorGUILayout.Toggle("Debug Vehicle Speed", save.debugSpeed);
            if(save.debugSpeed==true)
            {
                save.debug = true;
            }

            save.debugAI = EditorGUILayout.Toggle("Debug Vehicle AI", save.debugAI);
            if (save.debugAI == true)
            {
                save.debug = true;
            }

            save.debugIntersections = EditorGUILayout.Toggle("Debug Intersections", save.debugIntersections);
            save.stopIntersectionUpdate = EditorGUILayout.Toggle("Stop Intersection Update", save.stopIntersectionUpdate);
            save.debugWaypoints = EditorGUILayout.Toggle("Debug Waypoints", save.debugWaypoints);
            save.debugDisabledWaypoints = EditorGUILayout.Toggle("Disabled Waypoints", save.debugDisabledWaypoints);
            save.drawBodyForces = EditorGUILayout.Toggle("Draw Body Force", save.drawBodyForces);
            save.drawRaycasts = EditorGUILayout.Toggle("Draw Raycasts", save.drawRaycasts);
            save.debugDesnity = EditorGUILayout.Toggle("Debug Density", save.debugDesnity);

            //This is for testing purpose only
//#if !DEBUG_TRAFFIC
//            if (GUILayout.Button("Performance Debug"))
//            {
//                PreprocessorDirective.AddToCurrent(Gley.Common.Constants.DEBUG_TRAFFIC, false);
//            }
//#else
//            if (GUILayout.Button("End Debug"))
//            {
//                PreprocessorDirective.AddToCurrent(Gley.Common.Constants.DEBUG_TRAFFIC, true);
//            }
//#endif

            base.TopPart();
        }


        public override void DestroyWindow()
        {
            base.DestroyWindow();
            EditorUtility.SetDirty(save);
        }
    }
}
