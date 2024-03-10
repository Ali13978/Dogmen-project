using GleyUrbanAssets;
using System.Collections.Generic;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores waypoint properties
    /// </summary>
    public class WaypointSettings : WaypointSettingsBase
    {
        public List<VehicleTypes> allowedCars;
        public int maxSpeed;

        public bool giveWay;
        public bool enter;
        public bool exit;
        public bool speedLocked;
        public bool carsLocked;
    }
}