using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class TrafficConnectionWaypoints : ConnectionWaypoints
    {
        protected override void AddLaneConnectionWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            string roadName = connections.GetOriginRoad(index).name;
            List<int> allowedCars = connections.GetOutConnector<WaypointSettings>(index).allowedCars.Cast<int>().ToList();
            int maxSpeed = connections.GetOutConnector<WaypointSettings>(index).maxSpeed;

            Path curve = connections.GetCurve(index);

            Vector3[] p = curve.GetPointsInSegment(0, connections.GetOffset(index));
            float estimatedCurveLength = Vector3.Distance(p[0], p[3]);
            float nrOfWaypoints = estimatedCurveLength / waypointDistance;
            if (nrOfWaypoints < 1.5f)
            {
                nrOfWaypoints = 1.5f;
            }
            float step = 1 / nrOfWaypoints;
            float t = 0;
            int nr = 0;
            List<Transform> connectorWaypoints = new List<Transform>();
            while (t < 1)
            {
                t += step;
                if (t < 1)
                {
                    string waypointName = roadName + "-" + GleyUrbanAssets.Constants.laneNamePrefix + connections.GetLane(index) + "-" + GleyUrbanAssets.Constants.connectionWaypointName + (++nr);
                    connectorWaypoints.Add(CreateInstance<WaypointGeneratorTraffic>().CreateWaypoint(connections.GetHolder(index), BezierCurve.CalculateCubicBezierPoint(t, p[0], p[1], p[2], p[3]), waypointName, allowedCars, maxSpeed, connections.GetLaneConnection(index)));
                }
            }

            WaypointSettingsBase currentWaypoint;
            WaypointSettingsBase connectedWaypoint;

            //set names
            connectorWaypoints[0].name = roadName + "-" + GleyUrbanAssets.Constants.laneNamePrefix + connections.GetLane(index) + "-" + GleyUrbanAssets.Constants.connectionEdgeName + nr;
            connectorWaypoints[connectorWaypoints.Count - 1].name = roadName + "-" + GleyUrbanAssets.Constants.laneNamePrefix + connections.GetLane(index) + "-" + GleyUrbanAssets.Constants.connectionEdgeName + (connectorWaypoints.Count - 1);

            //link middle waypoints
            for (int j = 0; j < connectorWaypoints.Count - 1; j++)
            {
                currentWaypoint = connectorWaypoints[j].GetComponent<WaypointSettingsBase>();
                connectedWaypoint = connectorWaypoints[j + 1].GetComponent<WaypointSettingsBase>();
                currentWaypoint.neighbors.Add(connectedWaypoint);
                connectedWaypoint.prev.Add(currentWaypoint);
            }

            //link first waypoint
            connectedWaypoint = connectorWaypoints[0].GetComponent<WaypointSettingsBase>();
            currentWaypoint = connections.GetOutConnector<WaypointSettingsBase>(index);
            currentWaypoint.neighbors.Add(connectedWaypoint);
            connectedWaypoint.prev.Add(currentWaypoint);
            EditorUtility.SetDirty(currentWaypoint);
            EditorUtility.SetDirty(connectedWaypoint);

            //link last waypoint
            connectedWaypoint = connections.GetInConnector(index);
            currentWaypoint = connectorWaypoints[connectorWaypoints.Count - 1].GetComponent<WaypointSettingsBase>();
            currentWaypoint.neighbors.Add(connectedWaypoint);
            connectedWaypoint.prev.Add(currentWaypoint);
            EditorUtility.SetDirty(currentWaypoint);
            EditorUtility.SetDirty(connectedWaypoint);
        }
    }
}