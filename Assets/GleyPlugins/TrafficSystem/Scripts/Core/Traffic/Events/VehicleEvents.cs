namespace GleyTrafficSystem
{
    public static class VehicleEvents
    {
        /// <summary>
        /// Triggered when a new object enters vehicle trigger
        /// </summary>
        /// <param name="isInTrigger">object is in or out of trigger</param>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="vehicleHit">another vehicle is n trigger</param>
        /// <param name="collidingVehicleIndex">index of the colliding vehicle (-1 for other objects)</param>
        /// <param name="reverse">force reverse action</param>
        /// <param name="theySeeEachOther">other vehicle sees the current vehicles with the trigger</param>
        /// <param name="stop">force stop action</param>
        public delegate void NewObjectInTrigger(bool isInTrigger, int vehicleIndex, bool vehicleHit, int collidingVehicleIndex, bool reverse, bool theySeeEachOther, EnvironmentInteraction environmentInteraction);
        public static event NewObjectInTrigger onNewObjectInTrigger;
        //TODO Break this event in multiple small ones
        public static void TriggeerNewObjectInTriggerEvent(bool isInTrigger, int vehicleIndex, bool vehicleHit, int collidingVehicleIndex, bool reverse, bool theySeeEachOther, EnvironmentInteraction environmentInteraction)
        {
            if (onNewObjectInTrigger != null)
            {
                onNewObjectInTrigger(isInTrigger, vehicleIndex, vehicleHit, collidingVehicleIndex, reverse, theySeeEachOther, environmentInteraction);
            }
        }


        /// <summary>
        /// Trigger when 2 vehicle crash
        /// </summary>
        /// <param name="vehicle1Index">first vehicle index</param>
        /// <param name="vehicle2Index">second vehicle index</param>
        /// <param name="addAction">add or remove the action</param>
        public delegate void VehicleCrash(int vehicle1Index, int vehicle2Index, bool addAction);
        public static event VehicleCrash onVehicleCrash;
        public static void TriggerVehicleCrashEvent(int vehicle1Index, int vehicle2Index, bool addAction)
        {
            if (onVehicleCrash != null)
            {
                onVehicleCrash(vehicle1Index, vehicle2Index, addAction);
            }
        }
    }
}
