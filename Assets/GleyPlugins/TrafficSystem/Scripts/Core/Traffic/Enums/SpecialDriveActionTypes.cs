namespace GleyTrafficSystem
{
    public enum SpecialDriveActionTypes
    {
        Forward = 0,
        Continue = 10, //do not change current state
        Follow = 12, //
        Overtake = 13,
        GiveWay = 15,
        StopInPoint = 20, //traffic light, other road signs
        AvoidForward = 30, // mirror wheel rotation, keep direction
        TempStop = 60,
        StopInDistance = 70, // stop in front trigger dimension
        StopNow = 90, //stop instantly
        Reverse = 80, //keep wheels as they are
        AvoidReverse = 100, //change wheel direction
        NoWaypoint = 1000,
        Destroyed = 10000
    }
}