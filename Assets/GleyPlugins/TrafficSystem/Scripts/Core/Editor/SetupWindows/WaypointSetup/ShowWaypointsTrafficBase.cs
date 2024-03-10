using GleyUrbanAssets;

namespace GleyTrafficSystem
{
    public abstract class ShowWaypointsTrafficBase : ShowWaypointsBase
    {
        protected WaypointDrawer waypointDrawer;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            waypointDrawer = CreateInstance<WaypointDrawer>();
            waypointDrawer.Initialize();
            waypointDrawer.onWaypointClicked += WaipointClicked;
            base.Initialize(windowProperties, window);
            return this;
        }


        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override void OpenEditWindow(int index)
        {
            SettingsWindow.SetSelectedWaypoint((WaypointSettings)waypointsOfInterest[index]);
            GleyUtilities.TeleportSceneCamera(waypointsOfInterest[index].transform.position);
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected virtual void WaipointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaipointClicked;
            base.DestroyWindow();
        }
    }
}