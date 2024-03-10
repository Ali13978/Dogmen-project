using GleyUrbanAssets;
namespace GleyTrafficSystem
{
    public static class AllWindowsData
    {
        const string urbanNamespace = "GleyUrbanAssets";
        static WindowProperties[] allWindows =
        {
            //main menu
            new WindowProperties(Constants.trafficNamespace,nameof(MainMenuWindow),"Traffic Settings",false,true,false,true,true,false,"https://youtube.com/playlist?list=PLKeb94eicHQtyL7nYgZ4De1htLs8lmz9C"),
            new WindowProperties(urbanNamespace,nameof(ImportPackagesWindow),"Import Packages",true,true,true,false,true,false,"https://youtu.be/hjKXg6HtWPI"),
            new WindowProperties(Constants.trafficNamespace,nameof(RoadSetupWindow),"Road Setup",true,true,true,false,false,false,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Constants.trafficNamespace,nameof(WaypointSetupWindow),"Waypoint Setup",true,true,false,true,true,false,"https://youtu.be/mKfnm5_QW8s"),
            new WindowProperties(Constants.trafficNamespace,nameof(SceneSetupWindow), "Scene Setup",true,true,false,true,false,false,"https://youtu.be/203UgxPlfNo"),
            new WindowProperties(Constants.trafficNamespace,nameof(ExternalToolsWindow), "External Tools",true,true,true,false,false,false,"https://youtu.be/203UgxPlfNo"),
            new WindowProperties(Constants.trafficNamespace,nameof(DebugWindow), "Debug",true,true,true,false,false,false,"https://youtu.be/Bg-70Tum380"),

            //Road Setup
            new WindowProperties(Constants.trafficNamespace,nameof(NewRoadWindow), "Create Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Constants.trafficNamespace,nameof(ConnectRoadsWindow), "Connect Roads",true,true,true,true,true,true,"https://youtu.be/EKTVqvYQ01A"),
            new WindowProperties(Constants.trafficNamespace,nameof(ViewRoadsWindow), "View Roads",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Constants.trafficNamespace,nameof(EditRoadWindow), "Edit Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),

            //Waypoint Setup
             new WindowProperties(Constants.trafficNamespace,nameof(ShowAllWaypoints), "All Waypoints",true,true,true,false,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowVehicleTypeEditedWaypoints), "Vehicle Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowDisconnectedWaypoints), "Disconnected Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowGiveWayWaypoints), "Give Way Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowSpeedEditedWaypoints), "Speed Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowStopWaypoints), "Stop Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(ShowVehiclePathProblems), "Path Problems",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Constants.trafficNamespace,nameof(EditWaypointWindow), "Edit Waypoint",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),

             //Scene Setup
             new WindowProperties(Constants.trafficNamespace,nameof(GridSetupWindow), "Grid Setup",true,true,true,true,true,true,"https://youtu.be/203UgxPlfNo"),
             new WindowProperties(Constants.trafficNamespace,nameof(SpeedRoutesSetupWindow), "Speed Routes",true,true,false,true,true,true,"https://youtu.be/WqrADi8mUcI"),
             new WindowProperties(Constants.trafficNamespace,nameof(VehicleRoutesSetupWindow), "Vehicle Routes",true,true,false,true,true,true,"https://youtu.be/JNVwL9hcodw"),
             new WindowProperties(Constants.trafficNamespace,nameof(LayerSetupWindow), "Layer Setup",true,true,true,false,true,false,"https://youtu.be/203UgxPlfNo"),

             //Intersection
             new WindowProperties(Constants.trafficNamespace,nameof(IntersectionSetupWindow), "Intersection Setup",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(Constants.trafficNamespace,nameof(PriorityIntersectionWindow), "Priority Intersection",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(Constants.trafficNamespace,nameof(TrafficLightsIntersectionWindow), "Traffic Lights Intersection",true,true,true,true,true,true,"https://youtu.be/8tOnYiIYxeU"),

             //Car setup
             new WindowProperties(Constants.trafficNamespace,nameof(VehicleTypesWindow), "Vehicle Types",true,true,true,true,true,false,"https://youtu.be/203UgxPlfNo"),

             //External Tools
             new WindowProperties(Constants.trafficNamespace,nameof(EasyRoadsSetup), "Easy Roads Setup",true,true,true,true,false,false,"https://youtu.be/203UgxPlfNo"),
             new WindowProperties(Constants.trafficNamespace,nameof(CidySetup), "Cidy Setup",true,true,true,true,false,false,"https://youtu.be/203UgxPlfNo"),
        };


        internal static WindowProperties[] GetWindowsData()
        {
            return allWindows;
        }
    }
}
