using UnityEngine;

namespace GleyTrafficSystem
{
    public static class DensityEvents
    {
        /// <summary>
        /// Triggered when a new vehicle is enabled
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="targetWaypointPosition">instantiate position</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="powerStep">power step(acceleration)</param>
        /// <param name="brakeStep">brake power step</param>
        public delegate void VehicleAdded(int vehicleIndex);
        public static VehicleAdded onVehicleAdded;
        public static void TriggerVehicleAddedEvent(int vehicleIndex)
        {
            if (onVehicleAdded != null)
            {
                onVehicleAdded(vehicleIndex);
            }
        }
    }
}