using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Used to check if a vehicle can be instantiated in a given position
    /// </summary>
    public class PositionValidator : MonoBehaviour
    {
        private LayerMask trafficLayer;
        private LayerMask buildingsLayers;
        private Transform[] activeCameras;
        private float minDistanceToAdd;
        private bool debugDensity;

        private Vector3 debugPosition;
        private Matrix4x4 matrix;
        private float vehicleLength;
        private float vehicleWidth;
        private float vehicleHeight;


        /// <summary>
        /// Setup dependencies
        /// </summary>
        /// <param name="activeCameras"></param>
        /// <param name="trafficLayer"></param>
        /// <param name="buildingsLayers"></param>
        /// <param name="minDistanceToAdd"></param>
        /// <param name="debugDensity"></param>
        /// <returns></returns>
        public PositionValidator Initialize(Transform[] activeCameras, LayerMask trafficLayer, LayerMask buildingsLayers, float minDistanceToAdd, bool debugDensity)
        {
            UpdateCamera(activeCameras);
            this.trafficLayer = trafficLayer;
            this.minDistanceToAdd = minDistanceToAdd * minDistanceToAdd;
            this.buildingsLayers = buildingsLayers;
            this.debugDensity = debugDensity;
            return this;
        }


        /// <summary>
        /// Checks if a vehicle can be instantiated in a given position
        /// </summary>
        /// <param name="position">position to check</param>
        /// <param name="halfVehicleLength"></param>
        /// <param name="halfVehicleHeight"></param>
        /// <param name="ignoreLineOfSight">validate position eve if it is in view</param>
        /// <returns></returns>
        public bool IsValid(Vector3 position, float halfVehicleLength, float halfVehicleHeight, float halfVehicleWidth, bool ignoreLineOfSight, float frontWheelOffset, Quaternion rotation)
        {
            debugPosition = Vector3.zero;
            position -= rotation * new Vector3(0, 0, frontWheelOffset);
            vehicleLength = halfVehicleLength * 2;
            vehicleHeight = halfVehicleHeight * 2;
            vehicleWidth = halfVehicleWidth * 2;

            for (int i = 0; i < activeCameras.Length; i++)
            {
                //if position if far enough from the player
                if (Vector3.SqrMagnitude(activeCameras[i].position - position) > minDistanceToAdd)
                {
                    //check if there is no other vehicle in that area
                    if (Physics.OverlapBox(position, new Vector3(halfVehicleWidth, halfVehicleHeight, vehicleLength), rotation, trafficLayer).Length > 0)
                    {
                        if (debugDensity)
                        {
                            matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                            debugPosition = position;
                            Debug.DrawLine(activeCameras[i].position, position, Color.blue, 0.1f);
                        }
                        return false;
                    }
                    else
                    {
                        Debug.DrawLine(activeCameras[i].position, position, Color.green, 0.1f);
                    }
                }
                else
                {
                    //if it is close to the player
                    //check if it is obstructed by any other environment elements
                    if (!ignoreLineOfSight)
                    {
                        if (!Physics.Linecast(position, activeCameras[i].position, buildingsLayers))
                        {
                            if (debugDensity)
                            {
                                Debug.DrawLine(activeCameras[i].position, position, Color.red, 0.1f);
                            }
                            return false;
                        }
                        else
                        {
                            if (debugDensity)
                            {
                                Debug.DrawLine(activeCameras[i].position, position, Color.green, 0.1f);
                            }
                        }
                    }
                }
            }

            //check if the final position is free 
            if (Physics.OverlapBox(position, new Vector3(halfVehicleWidth, halfVehicleHeight, vehicleLength), rotation, trafficLayer).Length > 0)
            {
                if (debugDensity)
                {
                    matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                    debugPosition = position;
                }
                return false;
            }


            return true;
        }


        /// <summary>
        /// Update player camera transform
        /// </summary>
        /// <param name="activeCameras"></param>
        public void UpdateCamera(Transform[] activeCameras)
        {
            this.activeCameras = activeCameras;
        }

        //debug
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugPosition != Vector3.zero)
            {
                Handles.matrix = matrix;
                Handles.DrawWireCube(Vector3.zero, new Vector3(vehicleWidth, vehicleHeight, 2 * vehicleLength));
            }
        }
#endif
    }
}
