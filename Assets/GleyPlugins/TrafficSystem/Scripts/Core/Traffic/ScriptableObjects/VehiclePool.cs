using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores the vehicle prefabs used in scene
    /// </summary>
    [CreateAssetMenu(fileName = "CarPool", menuName = "TrafficSystem/Vehicle Pool", order = 1)]
    public class VehiclePool : ScriptableObject
    {
        public CarType[] trafficCars;

        public VehiclePool()
        {
            CarType carType = new CarType();
            trafficCars = new CarType[] { carType };
        }
    }

    [System.Serializable]
    public class CarType
    {
        public GameObject vehiclePrefab;
        [Range(1, 100)]
        public int percent;

        public CarType()
        {
            percent = 1;
        }
    }
}