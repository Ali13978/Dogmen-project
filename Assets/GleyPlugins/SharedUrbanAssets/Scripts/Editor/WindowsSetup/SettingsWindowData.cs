using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GleyUrbanAssets
{
    public partial class SettingsWindowData : ScriptableObject
    {
        public bool drawWaypoints = true;
        public bool drawLaneChange = true;
        public CreateRoadSave createRoadSave;
        public ViewRoadsSave viewRoadsSave;
        public ConnectRoadsSave connectRoadsSave;
        public RoadDefaults roadDefaults;
        public RoadColors roadColors;
        public CarRoutesSave carRoutesSave;
        public ViewWaypointsSettings allWaypointsSettings;
        public ViewWaypointsSettings disconnectedWaypointsSettings;
        public ViewWaypointsSettings carEditedWaypointsSettings;
        public ViewWaypointsSettings pathProblemsWaypointsSettings;
    }


    [System.Serializable]
    public class ViewRoadsSettings
    {
        public bool viewLanes;
        public bool viewWaypoints;
        public bool viewLaneChanges;
    }

    [System.Serializable]
    public class ViewWaypointsSettings
    {
        public bool showConnections;
        public bool showOtherLanes;
        public bool showSpeed;
        public bool showCars;
    }


    [System.Serializable]
    public class CreateRoadSave
    {
        public bool viewOtherRoads;
        public ViewRoadsSettings viewRoadsSettings;
    }


    [System.Serializable]
    public class ViewRoadsSave
    {
        public ViewRoadsSettings viewRoadsSettings;
    }


    [System.Serializable]
    public class ConnectRoadsSave
    {
        public ViewRoadsSettings viewRoadsSettings;
        public float waypointDistance = 4;
    }


    [System.Serializable]
    public class RoadDefaults
    {
        public int nrOfLanes;
        public float laneWidth;
        public float waypointDistance;

        public RoadDefaults(int nrOfLanes, float laneWidth, float waypointDistance)
        {
            this.nrOfLanes = nrOfLanes;
            this.laneWidth = laneWidth;
            this.waypointDistance = waypointDistance;
        }
    }


    [System.Serializable]
    public class RoadColors
    {
        public Color textColor = Color.white;

        public Color roadColor = Color.green;
        public Color laneColor = Color.blue;
        public Color laneChangeColor = Color.magenta;
        public Color connectorLaneColor = Color.cyan;

        public Color anchorPointColor = Color.white;
        public Color controlPointColor = Color.red;
        public Color roadConnectorColor = Color.cyan;
        public Color selectedRoadConnectorColor = Color.green;

        public Color waypointColor = Color.blue;
        public Color selectedWaypointColor = Color.green;
        public Color disconnectedColor = Color.red;
        public Color prevWaypointColor = Color.yellow;

        public Color speedColor = Color.white;
        public Color carsColor = Color.green;
    }


    [System.Serializable]
    public class CarRoutesSave
    {
        public List<Color> routesColor = new List<Color> { Color.white };
        public List<bool> active = new List<bool> { true };
    }
}
