namespace GleyUrbanAssets
{
    public static class WaypointEvents
    {
        /// <summary>
        /// Triggered to change the stop value of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        public delegate void TrafficLightChanged(int waypointIndex, bool stop);
        public static event TrafficLightChanged onTrafficLightChanged;
        public static void TriggerTrafficLightChangedEvent(int waypointIndex, bool stop)
        {
            if (onTrafficLightChanged != null)
            {
                onTrafficLightChanged(waypointIndex, stop);
            }
        }


        /// <summary>
        /// Triggered to notify vehicle about stop state and give way state of the waypoint
        /// </summary>
        /// <param name="index">vehicle index</param>
        /// <param name="stopState">stop in point needed</param>
        /// <param name="giveWayState">give way needed</param>
        public delegate void StopStateChanged(int index, bool stopState);
        public static event StopStateChanged onStopStateChanged;
        public static void TriggerStopStateChangedEvent(int agentIndex, bool stopState)
        {
            if (onStopStateChanged != null)
            {
                onStopStateChanged(agentIndex, stopState);
            }
        }

        public delegate void GiveWayStateChanged(int index,bool giveWayState);
        public static event GiveWayStateChanged onGiveWayStateChanged;
        public static void TriggerGiveWayStateChangedEvent(int index, bool giveWayState)
        {
            if (onGiveWayStateChanged != null)
            {
                onGiveWayStateChanged(index, giveWayState);
            }
        }
    }
}
