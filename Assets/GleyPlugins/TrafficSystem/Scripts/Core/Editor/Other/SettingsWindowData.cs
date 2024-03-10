
using GleyTrafficSystem;
using System.Collections.Generic;
using UnityEngine;

namespace GleyUrbanAssets
{
    public partial class SettingsWindowData : ScriptableObject
    {
        public EditRoadSave editRoadSave;
        public ViewWaypointsSettings speedEditedWaypointsSettings;
        public ViewWaypointsSettings giveWayWaypointsSettings;



        public ViewWaypointsSettings stopWaypointsSettings;

        public SpeedRoutesSave speedRoutesSave;
        public IntersectionSave intersectionSave;
    }


    [System.Serializable]
    public class EditRoadSave
    {
        public ViewRoadsSettings viewRoadsSettings;
        public MoveTools moveTool = MoveTools.Move2D;
        public int maxSpeed = 50;
        public List<VehicleTypes> globalCarList = new List<VehicleTypes>();
    }


    [System.Serializable]
    public class SpeedRoutesSave
    {
        public List<Color> routesColor = new List<Color> { Color.white };
        public List<bool> active = new List<bool> { true };
    }


    [System.Serializable]
    public class IntersectionSave
    {
        public bool showAll;
        public bool showExit = true;
        public Color priorityColor = Color.green;
        public Color lightsColor = Color.cyan;
        public Color stopWaypointsColor = Color.red;
        public Color exitWaypointsColor = Color.green;
    }
}