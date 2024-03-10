#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    public abstract partial class GenericIntersectionSettings : MonoBehaviour
    {
        public abstract int GetNumberOfRoads(int nrOfPeopleCrossings);

        public abstract bool CheckAssignements(int roadsToIgnore);

        public abstract List<IntersectionStopWaypointsSettings> GetAssignedWaypoints();

        public abstract List<WaypointSettings> GetExitWaypoints();
    }
}
#endif