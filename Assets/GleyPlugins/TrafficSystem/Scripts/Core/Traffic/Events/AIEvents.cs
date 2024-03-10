using UnityEngine;

namespace GleyTrafficSystem
{
    public static class AIEvents
    {
        /// <summary>
        /// Triggered when the driving action of a vehicle changed
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="action">new action</param>
        /// <param name="actionValue">action time</param>
        public delegate void ChangeDrivingState(int vehicleIndex, SpecialDriveActionTypes action, float actionValue);
        public static event ChangeDrivingState onChangeDrivingState;
        public static void TriggetChangeDrivingStateEvent(int vehicleIndex, SpecialDriveActionTypes action, float actionValue)
        {
            if (onChangeDrivingState != null)
            {
                onChangeDrivingState(vehicleIndex, action, actionValue);
            }
        }


        /// <summary>
        /// Triggered when a vehicle changed waypoint
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="targetWaypointPosition">new waypoint position</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="blinkType">blinking required</param>
        public delegate void ChangeDestination(int vehicleIndex);
        public static ChangeDestination onChangeDestination;
        public static void TriggerChangeDestinationEvent(int vehicleIndex)
        {
            if (onChangeDestination != null)
            {
                onChangeDestination(vehicleIndex);
            }
        }


        /// <summary>
        /// Triggered when a vehicle changes his state to notify other vehicles about this 
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="collider">collider of the vehicle</param>
        /// <param name="action">new action</param>
        public delegate void VehicleChangedState(int vehicleIndex, Collider collider, SpecialDriveActionTypes action);
        public static VehicleChangedState onVehicleChangedState;
        public static void TriggerVehicleChangedStateEvent(int vehicleIndex, Collider collider, SpecialDriveActionTypes action)
        {
            if (onVehicleChangedState != null)
            {
                onVehicleChangedState(vehicleIndex, collider, action);
            }
        }
    }
}