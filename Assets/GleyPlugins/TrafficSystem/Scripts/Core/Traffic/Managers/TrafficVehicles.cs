using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Keeps track of all traffic vehicles
    /// </summary>
    public class TrafficVehicles : MonoBehaviour
    {
        private List<VehicleComponent> allVehicles = new List<VehicleComponent>();
        private List<VehicleComponent> idleVehicles = new List<VehicleComponent>();
        private Transform trafficHolder;
        private float masterVolume;
        private float realtimeSinceStartup;


        /// <summary>
        /// Loads all traffic vehicles
        /// </summary>
        /// <param name="vehiclePool"></param>
        /// <param name="nrOfVehicles"></param>
        /// <param name="buildingLayers"></param>
        /// <param name="obstacleLayers"></param>
        /// <param name="playerLayers"></param>
        /// <param name="masterVolume"></param>
        /// <returns></returns>
        public TrafficVehicles Initialize(VehiclePool vehiclePool, int nrOfVehicles, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, float masterVolume)
        {
            if (trafficHolder == null)
            {
                trafficHolder = new GameObject(GleyUrbanAssets.Constants.trafficHolderName).transform;
            }

            this.masterVolume = masterVolume;

            //transform percent into numbers;
            float sum = 0;
            List<float> thresholds = new List<float>();
            for (int i = 0; i < vehiclePool.trafficCars.Length; i++)
            {
                sum += vehiclePool.trafficCars[i].percent;
                thresholds.Add(sum);
            }
            float perCarValue = sum / nrOfVehicles;

            //load cars
            int vehicleIndex = 0;
            for (int i = 0; i < nrOfVehicles; i++)
            {
                while ((i + 1) * perCarValue > thresholds[vehicleIndex])
                {
                    vehicleIndex++;
                    if (vehicleIndex >= vehiclePool.trafficCars.Length)
                    {
                        vehicleIndex = vehiclePool.trafficCars.Length - 1;
                        break;
                    }
                }

                LoadCar(vehiclePool.trafficCars[vehicleIndex].vehiclePrefab, buildingLayers, obstacleLayers, playerLayers);
            }
            return this;
        }


        /// <summary>
        /// Get entire vehicle list
        /// </summary>
        /// <returns></returns>
        public List<VehicleComponent> GetVehicleList()
        {
            return allVehicles;
        }


        /// <summary>
        /// Set reverse lights if required on a specific vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public void SetReverseLights(int index, bool active)
        {
            allVehicles[index].SetReverseLights(active);
        }


        /// <summary>
        /// Set brake lights if required on a specific vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public void SetBrakeLights(int index, bool active)
        {
            allVehicles[index].SetBrakeLights(active);
        }


        /// <summary>
        /// Get an available vehicle to be instantiated
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public VehicleComponent GetIdleVehicle(int vehicleIndex)
        {
            VehicleComponent vehicle = idleVehicles[vehicleIndex];
            idleVehicles.RemoveAt(vehicleIndex);
            return vehicle;
        }


        /// <summary>
        /// Get a random index of an idle vehicle
        /// </summary>
        /// <returns></returns>
        public int GetIdleVehicleIndex()
        {
            if (idleVehicles.Count > 0)
            {
                return Random.Range(0, idleVehicles.Count);
            }
            return -1;
        }

        public int GetIdleVehicleIndexOfType(VehicleTypes type)
        {
            for (int i = 0; i < idleVehicles.Count; i++)
            {
                if (idleVehicles[i].vehicleType == type)
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Get the vehicle type of a given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public VehicleTypes GetVehicleType(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].vehicleType;
        }


        public float GetFrontWheelOffset(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].frontTrigger.localPosition.z;
        }




        /// <summary>
        /// Remove the given vehicle from scene
        /// </summary>
        /// <param name="index"></param>
        public void DisableVehicle(int index)
        {
            idleVehicles.Add(allVehicles[index]);
            allVehicles[index].DeactivateVehicle();
        }


        /// <summary>
        /// Check if the given vehicle can be removed from scene
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CanBeRemoved(int index)
        {
            return allVehicles[index].CanBeRemoved();
        }


        /// <summary>
        /// Get the current velocity of the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(int index)
        {
            return allVehicles[index].GetVelocity();
        }


        /// <summary>
        /// Get the speed of the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetCurrentSpeed(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            return allVehicles[index].GetCurrentSpeed();
        }


        /// <summary>
        /// Get the length of the given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public float GetHalfVehicleLength(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].length / 2;
        }


        public float GetVehicleLength(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].length;
        }


        /// <summary>
        /// Get the height of the given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public float GetVehicleHeight(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].coliderHeight;
        }


        public float GetVehicleWidth(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].colliderWidth / 2;
        }


        /// <summary>
        /// Activate a vehicle on scene
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void ActivateVehicle(VehicleComponent vehicle, Vector3 position, Quaternion rotation)
        {
            vehicle.ActivateVehicle(position, rotation, masterVolume);
        }


        /// <summary>
        /// Get the vehicle collider
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Collider GetCollider(int index)
        {
            return allVehicles[index].GetCollider();
        }


        /// <summary>
        /// Get the vehicle moving direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetHeading(int index)
        {
            return allVehicles[index].GetHeading();
        }


        /// <summary>
        /// Get the vehicles forward direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetForwardVector(int index)
        {
            return allVehicles[index].GetForwardVector();
        }


        /// <summary>
        /// Set a new action for a vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="currentAction"></param>
        public void SetCurrentAction(int index, SpecialDriveActionTypes currentAction)
        {
            allVehicles[index].SetCurrentAction(currentAction);
        }


        /// <summary>
        /// The give vehicle has stopped reversing
        /// </summary>
        /// <param name="index"></param>
        public void CurrentVehicleActionDone(int index)
        {
            allVehicles[index].VehicleStoppedReversing();
        }


        /// <summary>
        /// Get the current action for the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public SpecialDriveActionTypes GetCurrentAction(int index)
        {
            return allVehicles[index].GetCurrentAction();
        }


        /// <summary>
        /// Get the vehicles max speed
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetMaxSpeed(int index)
        {
            return allVehicles[index].GetMaxSpeed();
        }


        /// <summary>
        /// ???
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetPossibleMaxSpeed(int index)
        {
            return allVehicles[index].maxPossibleSpeed;
        }


        /// <summary>
        /// Set the corresponding blinker for the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="blinkType"></param>
        public void SetBlinkLights(int index, BlinkType blinkType)
        {
            allVehicles[index].SetBlinker(blinkType);
        }


        /// <summary>
        /// Get the minimum distance to change the lane
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetSafeDistance(int index)
        {
            return allVehicles[index].GetSafeDistance();
        }


        /// <summary>
        /// get the minimum safe distance to overtake
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetOvertakeDistance(int index)
        {
            return allVehicles[index].GetOvertakeDistance();
        }


        /// <summary>
        /// Load car in scene
        /// </summary>
        /// <param name="carPrefab"></param>
        /// <param name="buildingLayers"></param>
        /// <param name="obstacleLayers"></param>
        /// <param name="playerLayers"></param>
        private void LoadCar(GameObject carPrefab, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers)
        {
            VehicleComponent car = Instantiate(carPrefab, Vector3.zero, Quaternion.identity, trafficHolder).GetComponent<VehicleComponent>().Initialize(buildingLayers, obstacleLayers, playerLayers);
            car.SetIndex(allVehicles.Count);
            car.name += allVehicles.Count;
            allVehicles.Add(car);
            DisableVehicle(car.GetIndex());
        }


        /// <summary>
        /// Get the spring force of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetSpringForce(int index)
        {
            return allVehicles[index].GetSpringForce();
        }


        /// <summary>
        /// Get the power step (acceleration) of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetPowerStep(int index)
        {
            return allVehicles[index].GetPowerStep();
        }


        /// <summary>
        /// Get the brake power step of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetBrakeStep(int index)
        {
            return allVehicles[index].GetBrakeStep();
        }


        /// <summary>
        /// Get ground orientation vector
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetGroundDirection(int index)
        {
            return allVehicles[index].GetGroundDirection();
        }


        /// <summary>
        /// Update additional components from the vehicle if needed
        /// </summary>
        /// <param name="index"></param>
        public void UpdateVehicleScripts(int index)
        {
            realtimeSinceStartup += Time.deltaTime;
            allVehicles[index].UpdateEngineSound(masterVolume);
            allVehicles[index].UpdateLights(realtimeSinceStartup);
        }


        /// <summary>
        /// Update main lights of the vehicle
        /// </summary>
        /// <param name="on"></param>
        public void UpdateVehicleLights(bool on)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].SetMainLights(on);
            }
        }


        /// <summary>
        /// Update engine volume of the vehicle
        /// </summary>
        /// <param name="volume"></param>
        public void UpdateMasterVolume(float volume)
        {
            this.masterVolume = volume;
        }

        public void SetPlayerInteractionDelegate(EnvironmentInteraction playerInteraction)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].SetPlayerInteractionDelegate(playerInteraction);
            }
        }

        public void SetBuildingInteractionDelegate(EnvironmentInteraction buildingInteraction)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].SetBuildingInteractionDelegate(buildingInteraction);
            }
        }

        public void SetDynamicObjectInteractionDelegate(EnvironmentInteraction dynamicObjectInteraction)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].SetDynamicObjectInteractionDelegate(dynamicObjectInteraction);
            }
        }

        internal void TriggerColliderRemovedEvent(Collider collider)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].ColliderRemoved(collider);
            }
        }
    }
}
