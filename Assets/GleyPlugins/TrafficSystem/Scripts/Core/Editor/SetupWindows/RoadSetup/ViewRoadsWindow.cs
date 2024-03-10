using GleyUrbanAssets;
using System.Collections.Generic;
using System.Linq;

namespace GleyTrafficSystem
{
    public class ViewRoadsWindow : ViewRoadsWindowBase
    {
        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override List<RoadBase> LoadAllRoads()
        {
            return RoadsLoader.Initialize().LoadAllRoads<Road>().Cast<RoadBase>().ToList();
        }


        protected override void SelectWaypoint(RoadBase road)
        {
            SettingsWindow.SetSelectedRoad((Road)road);
            window.SetActiveWindow(typeof(EditRoadWindow), true);
        }


        protected override void DeleteCurrentRoad(RoadBase road)
        {
            DeletRoad<RoadConnections>(road);
        }


        protected override void SetTexts()
        {
            drawAllRoadsText = "Draw All Roads";
        }
    }
}
