using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    public delegate SpecialDriveActionTypes EnvironmentInteraction();

    public delegate void TrafficLightsBehaviour(TrafficLightsColor currentRoadColor, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, string name);

    public delegate int SpawnWaypointSelector(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes carType);
}
