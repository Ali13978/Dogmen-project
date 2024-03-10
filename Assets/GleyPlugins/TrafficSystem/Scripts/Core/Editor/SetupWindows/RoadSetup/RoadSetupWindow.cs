using GleyUrbanAssets;

namespace GleyTrafficSystem
{
    public class RoadSetupWindow : RoadSetupWindowBase
    {
        protected override void ViewRoads()
        {
            window.SetActiveWindow(typeof(ViewRoadsWindow), true);
        }


        protected override void ConnectRoads()
        {
            window.SetActiveWindow(typeof(ConnectRoadsWindow), true);
        }


        protected override void CreateRoad()
        {
            window.SetActiveWindow(typeof(NewRoadWindow), true);
        }


        protected override void SetTexts()
        {
            createRoad = "Create Road";
            connectRoads = "Connect Roads";
            viewRoads = "View Roads";
        }
    }
}