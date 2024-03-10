using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class RoadSetupWindowBase : SetupWindowBase
    {
        protected string createRoad;
        protected string connectRoads;
        protected string viewRoads;

        protected abstract void CreateRoad();
        protected abstract void ViewRoads();
        protected abstract void ConnectRoads();
        protected abstract void SetTexts();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            SetTexts();
            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Select action:");
            EditorGUILayout.Space();

            if (GUILayout.Button(createRoad))
            {
                CreateRoad();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button(connectRoads))
            {
                ConnectRoads();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button(viewRoads))
            {
                ViewRoads();
            }
        }
    }
}