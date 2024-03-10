using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class AgentRoutesSetupWindowBase<T> : SetupWindowBase where T : WaypointSettingsBase
    {
        private WaypointDrawerBase<T> waypointDrawer;
        private SettingsLoader settingsLoader;
        private CarRoutesSave save;
        private float scrollAdjustment = 112;
        private int nrOfCars;

        protected abstract SettingsLoader LoadSettingsLoader();
        protected abstract WaypointDrawerBase<T> SetWaypointDrawer();
        protected abstract int GetNrOfDifferentAgents();
        protected abstract string ConvertIndexToEnumName(int i);
        protected abstract void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointDrawer = SetWaypointDrawer();
            waypointDrawer.Initialize();
            nrOfCars = GetNrOfDifferentAgents();
            settingsLoader = LoadSettingsLoader();
            save = settingsLoader.LoadCarRoutes();

            if (save.routesColor.Count < nrOfCars)
            {
                for (int i = save.routesColor.Count; i < nrOfCars; i++)
                {
                    save.routesColor.Add(Color.white);
                    save.active.Add(true);
                }
            }
            waypointDrawer.onWaypointClicked += WaypointClicked;
            return this;
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < nrOfCars; i++)
            {
                if (save.active[i])
                {
                    waypointDrawer.DrawWaypointsForCar(i, save.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("Car Routes: ");
            for (int i = 0; i < nrOfCars; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ConvertIndexToEnumName(i), GUILayout.MaxWidth(150));
                save.routesColor[i] = EditorGUILayout.ColorField(save.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (save.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View", GUILayout.MaxWidth(BUTTON_DIMENSION)))
                {
                    save.active[i] = !save.active[i];
                    SceneView.RepaintAll();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();
            }

            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            settingsLoader.SaveCarRoutes(save);
            base.DestroyWindow();
        }
    }
}