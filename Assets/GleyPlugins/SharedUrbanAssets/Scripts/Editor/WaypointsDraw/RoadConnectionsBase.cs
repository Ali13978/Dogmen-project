using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class RoadConnectionsBase : Editor
    {
        List<ConnectionPool> connectionPools;

        protected abstract List<RoadBase> LoadAllRoads();
        protected abstract ConnectionPool GetConnectionPool();
        protected abstract void GenerateConnectorWaypoints(ConnectionPool connections, int index, float waypointDistance);

        internal abstract void DeleteConnection(ConnectionCurve connectingCurve);


        internal RoadConnectionsBase Initialize()
        {
            LoadAllConnections();
            return this;
        }


        internal List<ConnectionPool> ConnectionPools
        {
            get
            {
                return connectionPools;
            }
        }


        internal void MakeConnection(ConnectionPool connectionPool, RoadBase fromRoad, int fromIndex, RoadBase toRoad, int toIndex, float waypointDistance)
        {
            Vector3 offset = Vector3.zero;
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                if (GleyPrefabUtilities.IsInsidePrefab(fromRoad.gameObject) && GleyUrbanAssets.GleyPrefabUtilities.GetInstancePrefabRoot(fromRoad.gameObject) == GleyUrbanAssets.GleyPrefabUtilities.GetInstancePrefabRoot(toRoad.gameObject))
                {
                    connectionPool = GetConnectionPool();
                    offset = fromRoad.positionOffset;
                }
                else
                {
                    connectionPool = GetConnectionPool();
                    offset = fromRoad.positionOffset;
                }
            }
            connectionPool.AddConnection(fromRoad.lanes[fromIndex].laneEdges.outConnector, toRoad.lanes[toIndex].laneEdges.inConnector, fromRoad, fromIndex, toRoad, toIndex, offset);
            GenerateConnectorWaypoints(connectionPool, connectionPool.connectionCurves.Count - 1, waypointDistance);

            EditorUtility.SetDirty(connectionPool);
            AssetDatabase.SaveAssets();
            LoadAllConnections();
        }


        internal void GenerateSelectedConnections(float waypointDistance)
        {
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                int nrOfConnections = ConnectionPools[i].GetNrOfConnections();
                for (int j = 0; j < nrOfConnections; j++)
                {
                    if (ConnectionPools[i].connectionCurves[j].draw)
                    {
                        if (GleyUtilities.IsPointInsideView(ConnectionPools[i].GetInConnector(j).transform.position) ||
                            GleyUtilities.IsPointInsideView(ConnectionPools[i].GetOutConnector<WaypointSettingsBase>(j).transform.position))
                        {
                            GenerateConnectorWaypoints(ConnectionPools[i], j, waypointDistance);
                        }
                    }
                }
            }
        }


        private void LoadAllConnections()
        {
            connectionPools = new List<ConnectionPool>();
            List<RoadBase> allRoads = LoadAllRoads();
            for (int i = 0; i < allRoads.Count; i++)
            {
                if (allRoads[i].isInsidePrefab && !GleyPrefabUtilities.EditingInsidePrefab())
                {
                    continue;
                }
                ConnectionPool connectionsScript = allRoads[i].transform.parent.GetComponent<ConnectionPool>();
                if (!connectionPools.Contains(connectionsScript))
                {
                    connectionPools.Add(connectionsScript);
                }
            }
        }
    }
}
