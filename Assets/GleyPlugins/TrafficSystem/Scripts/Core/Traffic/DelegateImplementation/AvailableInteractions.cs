namespace GleyTrafficSystem
{
    public class AvailableInteractions
    {
        public static SpecialDriveActionTypes PlayerInteraction()
        {
            return SpecialDriveActionTypes.StopInDistance;
        }

        public static SpecialDriveActionTypes BuildingInteraction()
        {
            return SpecialDriveActionTypes.AvoidReverse;
        }

        public static SpecialDriveActionTypes DynamicObjectInteraction()
        {
            return SpecialDriveActionTypes.StopInDistance;
        }
    }
}
