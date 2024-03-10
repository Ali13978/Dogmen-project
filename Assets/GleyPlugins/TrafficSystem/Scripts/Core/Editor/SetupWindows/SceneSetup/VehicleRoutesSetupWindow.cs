using GleyUrbanAssets;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class VehicleRoutesSetupWindow : AgentRoutesSetupWindowBase<WaypointSettings>
    {
        protected override int GetNrOfDifferentAgents()
        {
            return System.Enum.GetValues(typeof(VehicleTypes)).Length;
        }


        protected override WaypointDrawerBase<WaypointSettings> SetWaypointDrawer()
        {
            return CreateInstance<WaypointDrawer>();
        }


        protected override SettingsLoader LoadSettingsLoader()
        {
            return new SettingsLoader(Constants.windowSettingsPath);
        }


        protected override void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected override string ConvertIndexToEnumName(int i)
        {
            return ((VehicleTypes)i).ToString();
        }
    }
    
}