using GleyUrbanAssets;
using System.Collections.Generic;

namespace GleyTrafficSystem
{
    public class ShowAllWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            save = settingsLoader.LoadAllWaypointsSave();
            return this;
        }


        public override void DrawInScene()
        {
            waypointDrawer.DrawAllWaypoints(roadColors.waypointColor, save.showConnections, roadColors.waypointColor, save.showSpeed, roadColors.speedColor, save.showCars, roadColors.carsColor, save.showOtherLanes, roadColors.laneChangeColor);
            base.DrawInScene();
        }


        public override void DestroyWindow()
        {
            settingsLoader.SaveAllWaypointsSettings(save, roadColors);
            base.DestroyWindow();
        }


        protected override List<WaypointSettingsBase> GetWaypointsOfIntereset()
        {
            return null;
        }
    }
}
