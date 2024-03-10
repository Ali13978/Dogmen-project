using GleyUrbanAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class VehicleTypesWindow : SetupWindowBase
    {
        private float scrollAdjustment = 212;
        private string errorText;
        private List<string> carCategories = new List<string>();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            errorText = "";
            LoadCars();
            return base.Initialize(windowProperties, window);
        }


        private void LoadCars()
        {
            var allCarTypes = Enum.GetValues(typeof(VehicleTypes)).Cast<VehicleTypes>();
            foreach (VehicleTypes car in allCarTypes)
            {
                carCategories.Add(car.ToString());
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Car types are used to limit vehicle movement.\n" +
                "You can use different car types to restrict the access of different type of vehicles in some areas.");
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            for (int i = 0; i < carCategories.Count; i++)
            {
                GUILayout.BeginHorizontal();
                carCategories[i] = EditorGUILayout.TextField(carCategories[i]);
                carCategories[i] = Regex.Replace(carCategories[i], @"^[\d-]*\s*", "");
                carCategories[i] = carCategories[i].Replace(" ", "");
                carCategories[i] = carCategories[i].Trim();
                if (GUILayout.Button("Remove"))
                {
                    carCategories.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add car category"))
            {
                carCategories.Add("");
            }

            GUILayout.EndScrollView();
        }


        protected override void BottomPart()
        {
            GUILayout.Label(errorText);
            if (GUILayout.Button("Save"))
            {
                if (CheckForNull() == false)
                {
                    errorText = "Success";
                    Save();
                }
            }
            EditorGUILayout.Space();
            base.BottomPart();
        }


        private void Save()
        {
            FileCreator.CreateVehicleTypesFile<VehicleTypes>(carCategories, Gley.Common.Constants.USE_GLEY_TRAFFIC, Constants.trafficNamespace, Constants.agentTypesPath);
        }


        private bool CheckForNull()
        {
            for (int i = 0; i < carCategories.Count - 1; i++)
            {
                for (int j = i + 1; j < carCategories.Count; j++)
                {
                    if (carCategories[i] == carCategories[j])
                    {
                        errorText = carCategories[i] + " Already exists. No duplicates allowed";
                        return true;
                    }
                }
            }
            for (int i = 0; i < carCategories.Count; i++)
            {
                if (string.IsNullOrEmpty(carCategories[i]))
                {
                    errorText = "Car category cannot be empty! Please fill all of them";
                    return true;
                }
            }
            return false;
        }
    }
}
