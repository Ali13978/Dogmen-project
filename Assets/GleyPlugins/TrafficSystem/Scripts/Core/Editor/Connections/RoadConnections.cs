using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace GleyTrafficSystem
{
    public class RoadConnections : RoadConnectionsBase
    {
        protected override ConnectionPool GetConnectionPool()
        {
            return RoadCreator.GetRoadWaypointsHolder(Constants.trafficWaypointsHolderName).GetComponent<ConnectionPool>();
        }


        protected override List<RoadBase> LoadAllRoads()
        {
            return RoadsLoader.Initialize().LoadAllRoads<Road>().Cast<RoadBase>().ToList();
        }


        protected override void GenerateConnectorWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            CreateInstance<TrafficConnectionWaypoints>().GenerateConnectorWaypoints(connections, index, waypointDistance);
        }

        internal override void DeleteConnection(ConnectionCurve connectingCurve)
        {
            CreateInstance<TrafficConnectionWaypoints>().RemoveConnectionHolder(connectingCurve.holder);
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                if (ConnectionPools[i].connectionCurves != null)
                {
                    if (ConnectionPools[i].connectionCurves.Contains(connectingCurve))
                    {
                        ConnectionPools[i].RemoveConnection(connectingCurve);
                        EditorUtility.SetDirty(ConnectionPools[i]);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}