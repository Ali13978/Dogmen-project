using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class LayerSetupWindow : SetupWindowBase
    {
        private LayerSetup layerSetup;
        private string[] allLayers;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            layerSetup = LayerOperations.LoadOrCreateLayers<LayerSetup>(Constants.layerPath);
            allLayers = CreateDefaultLayers();
            return base.Initialize(windowProperties, window);
        }


        private string[] CreateDefaultLayers()
        {
            List<string> allLayers = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);

                if (layerName == "" || string.IsNullOrEmpty(layerName))
                {
                    allLayers.Add(i.ToString());
                }
                else
                {
                    allLayers.Add(layerName);
                }
            }
            return allLayers.ToArray();
        }


        protected override void TopPart()
        {
            layerSetup.roadLayers = LayerMaskField(new GUIContent("Road Layers", "Vehicle wheels will collide only with these layers"), layerSetup.roadLayers);
            layerSetup.trafficLayers = LayerMaskField(new GUIContent("Traffic Layers", "All traffic vehicles should be on this layer"), layerSetup.trafficLayers);
            layerSetup.buildingsLayers = LayerMaskField(new GUIContent("Buildings Layers", "Vehicles will try to avoid objects on these layers"), layerSetup.buildingsLayers);
            layerSetup.obstaclesLayers = LayerMaskField(new GUIContent("Obstacle Layers", "Vehicles will stop when objects on these layers are seen"), layerSetup.obstaclesLayers);
            layerSetup.playerLayers = LayerMaskField(new GUIContent("Player Layers", "Vehicles will stop when objects on these layers are seen"), layerSetup.playerLayers);

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Tags and Layers Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Tags and Layers");
            }

            base.TopPart();
        }


        private LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            layerMask.value = EditorGUILayout.MaskField(label, layerMask.value, allLayers);

            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((layerMask.value & (1 << i)) > 0)
                    mask |= (1 << i);
            }
            layerMask.value = mask;

            return layerMask;
        }


        public override void DestroyWindow()
        {
            layerSetup.edited = true;
            EditorUtility.SetDirty(layerSetup);
            AssetDatabase.SaveAssets();
            SettingsWindow.UpdateLayers();
            base.DestroyWindow();
        }
    }
}