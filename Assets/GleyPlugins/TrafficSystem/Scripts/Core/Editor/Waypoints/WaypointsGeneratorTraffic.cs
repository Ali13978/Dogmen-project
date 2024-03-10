using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class WaypointGeneratorTraffic : WaypointsGenerator
    {
        internal override Transform CreateWaypoint(Transform parent, Vector3 waypointPosition, string name, List<int> allowedCars, int maxSpeed, ConnectionCurve connection)
        {
            Transform waypointTransform = base.CreateWaypoint(parent, waypointPosition, name, allowedCars, maxSpeed, connection);
            WaypointSettings waypointScript = waypointTransform.gameObject.AddComponent<WaypointSettings>();
            waypointScript.EditorSetup();
            waypointScript.connection = connection;
            waypointScript.allowedCars = allowedCars.Cast<VehicleTypes>().ToList();
            waypointScript.maxSpeed = maxSpeed;
            return waypointTransform;
        }
    }
}
