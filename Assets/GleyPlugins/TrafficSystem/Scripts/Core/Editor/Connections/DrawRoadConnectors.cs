using GleyUrbanAssets;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class DrawRoadConnectors : DrawRoadConnectorsBase
    {
        protected override void ConnectorClicked(ConnectionPool connectionPool, RoadBase fromRoad, int fromIndex, RoadBase toRoad, int toIndex, float waypointDistance)
        {
            CreateInstance<RoadConnections>().Initialize().MakeConnection(connectionPool, fromRoad, fromIndex, toRoad, toIndex, waypointDistance);
            base.ConnectorClicked(connectionPool, fromRoad, fromIndex, toRoad, toIndex, waypointDistance);
        }


        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override void LogWarning(RoadBase road)
        {
            Debug.LogWarning("Road " + road.name + " is not correctly generated. Please regenerate it from Traffic System Settings -> Road Setup -> Edit ->" + road.name);
        }
    }
}