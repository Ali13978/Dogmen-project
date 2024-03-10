using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GleyTrafficSystem
{
    public class NewRoadWindow : NewRoadWindowBase
    {
        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override List<RoadBase> LoadAllRoads()
        {
            return RoadsLoader.Initialize().LoadAllRoads<Road>().Cast<RoadBase>().ToList();
        }


        protected override void CreateRoad()
        {
            Road selectedRoad = new RoadCreator().Create<Road>(firstClick, Constants.trafficWaypointsHolderName, settingsLoader.LoadRoadDefaultsSave());
            selectedRoad.CreatePath(firstClick, secondClick);
            selectedRoad.SetRoadProperties(settingsLoader.LoadEditRoadSave().maxSpeed, System.Enum.GetValues(typeof(VehicleTypes)).Length);
            SettingsWindow.SetSelectedRoad(selectedRoad);
            window.SetActiveWindow(typeof(EditRoadWindow), false);
            base.CreateRoad();
        }


        protected override void SetTopText()
        {
            EditorGUILayout.LabelField("Press SHIFT + Left Click to add a road point");
            EditorGUILayout.LabelField("Press SHIFT + Right Click to remove a road point");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("If you are not able to draw, make sure your ground/road is on the layer marked as Road inside Layer Setup");
        }
    }
}