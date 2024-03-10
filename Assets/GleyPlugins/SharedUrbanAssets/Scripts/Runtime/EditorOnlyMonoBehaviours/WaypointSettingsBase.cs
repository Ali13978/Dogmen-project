using System.Collections.Generic;
using UnityEngine;
namespace GleyUrbanAssets
{
    public class WaypointSettingsBase : MonoBehaviour
    {
        public List<WaypointSettingsBase> neighbors;
        public List<WaypointSettingsBase> prev;
        public List<WaypointSettingsBase> otherLanes;
        public ConnectionCurve connection;
        public bool stop;
        public bool draw = true;

        public void EditorSetup()
        {
            neighbors = new List<WaypointSettingsBase>();
            prev = new List<WaypointSettingsBase>();
            otherLanes = new List<WaypointSettingsBase>();
        }

        public void Initialize()
        {
        }
    }

}