using GleyUrbanAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class ConnectRoadsWindow : ConnectRoadsWindowBase
    {
        DrawRoadConnectors drawRoadConnectors;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            drawRoadConnectors = (DrawRoadConnectors)CreateInstance<DrawRoadConnectors>().Initialize();
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            allowedCarIndex = new bool[nrOfCars];
            for (int i = 0; i < allowedCarIndex.Length; i++)
            {
                allowedCarIndex[i] = true;
            }

            return this;
        }


        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override RoadConnectionsBase LoadRoadConnections()
        {
            return CreateInstance<RoadConnections>().Initialize();
        }


        protected override List<RoadBase> LoadAllRoads()
        {
            return RoadsLoader.Initialize().LoadAllRoads<Road>().Cast<RoadBase>().ToList();
        }


        protected override void GenerateSelectedConnections()
        {
            CreateInstance<RoadConnections>().Initialize().GenerateSelectedConnections(save.waypointDistance);
            base.GenerateSelectedConnections();
        }


        protected override void MakeClickConnection(List<RoadBase> allRoads, List<ConnectionPool> allConnections, Color connectorLaneColor, Color anchorPointColor, Color roadConnectorColor, Color selectedRoadConnectorColor, Color disconnectedColor, float waypointDistance, Color textColor)
        {
            drawRoadConnectors.MakeCnnections(allRoads, allConnections, connectorLaneColor, anchorPointColor, roadConnectorColor, selectedRoadConnectorColor, disconnectedColor, waypointDistance, textColor);

            base.MakeClickConnection(allRoads, allConnections, connectorLaneColor, anchorPointColor, roadConnectorColor, selectedRoadConnectorColor, disconnectedColor, waypointDistance, textColor);
        }
    }
}
