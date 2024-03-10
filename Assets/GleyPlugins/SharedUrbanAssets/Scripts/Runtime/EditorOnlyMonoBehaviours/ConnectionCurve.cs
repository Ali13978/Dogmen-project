using UnityEngine;
namespace GleyUrbanAssets
{
    /// <summary>
    /// Store connection curve parameters
    /// </summary>
    [System.Serializable]
    public class ConnectionCurve
    {
        [HideInInspector]
        public string name;
        public Transform holder;
        public Path curve;
        public RoadBase fromRoad;
        public int fromIndex;
        public RoadBase toRoad;
        public int toIndex;
        public bool draw;
        public bool drawWaypoints;

        public ConnectionCurve(Path curve, RoadBase fromRoad, int fromIndex, RoadBase toRoad, int toIndex, bool draw, Transform holder)
        {
            name = holder.name;
            this.curve = curve;
            this.fromRoad = fromRoad;
            this.fromIndex = fromIndex;
            this.toRoad = toRoad;
            this.toIndex = toIndex;
            this.draw = draw;
            this.holder = holder;
        }
    }
}