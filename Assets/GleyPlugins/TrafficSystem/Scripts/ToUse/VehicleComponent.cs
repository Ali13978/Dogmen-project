using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Add this script on a vehicle prefab and configure the required parameters
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleComponent : MonoBehaviour
    {
        [Header("Object References")]
        [Tooltip("RigidBody of the vehicle, make sure drag is not 0")]
        public Rigidbody rb;
        [Tooltip("Empty GameObject used to rotate the vehicle from the correct point")]
        public Transform carHolder;
        [Tooltip("All vehicle wheels and their properties")]
        public Wheel[] allWheels;
        [Tooltip("Front trigger used to detect obstacle. It is automatically generated")]
        public Transform frontTrigger;
        [Tooltip("Assign this object if you need a hard shadow on your vehicle, leave it black otherwise")]
        public Transform shadowHolder;

        [Header("Car Properties")]
        [Tooltip("Vehicle type used for making custom paths")]
        public VehicleTypes vehicleType;
        [Tooltip("If suspension is set to 0, the value of suspension will be half of the wheel radius")]
        public float maxSuspension = 0f;
        [Tooltip("[0,1] where the idle suspension should be. Default is 0.5 at the middle of the suspension distance")]
        public float suspensionPosition = 0.5f;
        [Tooltip("Max wheel turn amount in degrees")]
        public float maxSteer = 30;
        [Tooltip("Min vehicle speed. Actual vehicle speed is picked random between min and max")]
        public int minPossibleSpeed = 40;
        [Tooltip("Max vehicle speed")]
        public int maxPossibleSpeed = 90;
        [Tooltip("Time in seconds to reach max speed (acceleration)")]
        public float accelerationTime = 10;
        [Tooltip("Time to full stop the vehicle (brake power)")]
        public float brakeTime = 3;

        [Header("Automatically Computed")]
        [Tooltip("Minimum safe distance to change a lane")]
        public float safeDistance = 24;
        [Tooltip("Minimum safe distance to overtake another vehicle")]
        public float overtakeDistance = 8;
        [Tooltip("Vehicle length")]
        public float length = 0;
        [Tooltip("Collider height")]
        public float coliderHeight = 0;
        [Tooltip("Distance between wheels, used for turning")]
        public float wheelDistance;
        [Tooltip("Audio for car accecleration")]
        public AudioClip accelerationAudio;

        [HideInInspector]
        public VisibilityScript visibilityScript;

        private Collider[] allColliders;
        private List<Collider> obstacleList;
        private Transform frontAxle;
        private LayerMask buildingLayers;
        private LayerMask obstacleLayers;
        private LayerMask playerLayers;
        private EngineSoundComponent engineSound;
        private VehicleLightsComponent vehicleLights;
        private SpecialDriveActionTypes currentAction;
        private float springForce;
        internal float colliderWidth;
        private int maxSpeed;
        private int listIndex;
        private bool isInTrigger;
        private bool lightsOn;

        private EnvironmentInteraction playerInteraction;
        private EnvironmentInteraction buildingInteraction;
        private EnvironmentInteraction dynamicObjectInteraction;

        private EnvironmentInteraction PlayerInteraction
        {
            get
            {
                if (playerInteraction == null)
                {
                    playerInteraction = AvailableInteractions.PlayerInteraction;
                }
                return playerInteraction;
            }
        }
        private EnvironmentInteraction BuildingInteraction
        {
            get
            {
                if (buildingInteraction == null)
                {
                    buildingInteraction = AvailableInteractions.BuildingInteraction;
                }
                return buildingInteraction;
            }
        }
        private EnvironmentInteraction DynamicObjectInteraction
        {
            get
            {
                if (dynamicObjectInteraction == null)
                {
                    dynamicObjectInteraction = AvailableInteractions.DynamicObjectInteraction;
                }
                return dynamicObjectInteraction;
            }
        }

        /// <summary>
        /// Initialize vehicle
        /// </summary>
        /// <param name="buildingLayers">static colliders to interact with</param>
        /// <param name="obstacleLayers">dynamic colliders to interact with</param>
        /// <param name="playerLayers">player colliders to interact with</param>
        /// <returns>the vehicle</returns>
        public virtual VehicleComponent Initialize(LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers)
        {
            this.buildingLayers = buildingLayers;
            this.obstacleLayers = obstacleLayers;
            this.playerLayers = playerLayers;
            allColliders = GetComponentsInChildren<Collider>();
            springForce = ((rb.mass * -Physics.gravity.y) / allWheels.Length);
            colliderWidth = frontTrigger.GetChild(0).GetComponent<BoxCollider>().size.x;
            DeactivateVehicle();

            //compute center of mass based on the wheel position
            Vector3 centerOfMass = Vector3.zero;
            for (int i = 0; i < allWheels.Length; i++)
            {
                allWheels[i].wheelTransform.Translate(Vector3.up * (allWheels[i].maxSuspension / 2 + allWheels[i].wheelRadius));
                centerOfMass += allWheels[i].wheelTransform.position;
            }
            rb.centerOfMass = centerOfMass / allWheels.Length;

            //set additional components
            engineSound = GetComponent<EngineSoundComponent>();
            if (engineSound)
            {
                engineSound.Initialize();
            }

            vehicleLights = GetComponent<VehicleLightsComponent>();
            if (vehicleLights)
            {
                vehicleLights.Initialize();
            }
            return this;
        }


        public void SetPlayerInteractionDelegate(EnvironmentInteraction playerInteraction)
        {
            this.playerInteraction = playerInteraction;
        }

        public void SetBuildingInteractionDelegate(EnvironmentInteraction buildingInteraction)
        {
            this.buildingInteraction = buildingInteraction;
        }

        public void SetDynamicObjectInteractionDelegate(EnvironmentInteraction dynamicObjectInteraction)
        {
            this.dynamicObjectInteraction = dynamicObjectInteraction;
        }

        /// <summary>
        /// Add a vehicle on scene
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="masterVolume"></param>
        public virtual void ActivateVehicle(Vector3 position, Quaternion rotation, float masterVolume)
        {
            maxSpeed = Random.Range(minPossibleSpeed, maxPossibleSpeed);
#if DEBUG_TRAFFIC
            gameObject.transform.SetPositionAndRotation(position, rotation *= Quaternion.Euler(transform.up * -90));
#else
            gameObject.transform.SetPositionAndRotation(position, rotation);
#endif

            //position vehicle with front wheels on the waypoint
            float distance = Vector3.Distance(position, frontTrigger.transform.position);
            transform.Translate(-transform.forward * distance, Space.World);
            gameObject.SetActive(true);


            if (engineSound)
            {
                engineSound.Play(masterVolume);
            }

            SetMainLights(lightsOn);

            AIEvents.onVehicleChangedState += AVehicleChengedState;
        }




        /// <summary>
        /// Remove a vehicle from scene
        /// </summary>
        public virtual void DeactivateVehicle()
        {
            isInTrigger = false;
            obstacleList = new List<Collider>();
            gameObject.SetActive(false);
            visibilityScript.Reset();
            if (engineSound)
            {
                engineSound.Stop();
            }

            if (vehicleLights)
            {
                vehicleLights.DeactivateLights();
            }
            AIEvents.onVehicleChangedState -= AVehicleChengedState;
        }


        /// <summary>
        /// Compute the ground direction vector used to apply forces, and update the shadow
        /// </summary>
        /// <returns>ground direction</returns>
        public Vector3 GetGroundDirection()
        {
            Vector3 frontPoint = Vector3.zero;
            int nrFront = 0;
            Vector3 backPoint = Vector3.zero;
            int nrBack = 0;
            for (int i = 0; i < allWheels.Length; i++)
            {
                if (allWheels[i].wheelPosition == Wheel.WheelPosition.Front)
                {
                    nrFront++;
                    frontPoint += allWheels[i].wheelGraphics.position;
                }
                else
                {
                    nrBack++;
                    backPoint += allWheels[i].wheelGraphics.position;
                }
            }
            Vector3 groundDirection = (frontPoint / nrFront - backPoint / nrBack).normalized;
            if (shadowHolder)
            {
                Vector3 centerPoint = (frontPoint / nrFront + backPoint / nrBack) / 2 - transform.up * (allWheels[0].wheelRadius - 0.1f);
                shadowHolder.rotation = Quaternion.LookRotation(groundDirection);
                shadowHolder.position = new Vector3(shadowHolder.position.x, centerPoint.y, shadowHolder.position.z);

            }
            return groundDirection;
        }


        /// <summary>
        /// Computes the acceleration per frame
        /// </summary>
        /// <returns></returns>
        public float GetPowerStep()
        {
            int nrOfFrames = (int)(accelerationTime / Time.fixedDeltaTime);
            float targetSpeedMS = maxSpeed / 3.6f;
            return targetSpeedMS / nrOfFrames;
        }


        /// <summary>
        /// Computes steering speed per frame
        /// </summary>
        /// <returns></returns>
        public float GetSteeringStep()
        {
            return maxSteer * Time.fixedDeltaTime;
        }


        /// <summary>
        /// Computes brake step per frame
        /// </summary>
        /// <returns></returns>
        public float GetBrakeStep()
        {
            int nrOfFrames = (int)(brakeTime / Time.fixedDeltaTime);
            float targetSpeedMS = maxSpeed / 3.6f;
            return targetSpeedMS / nrOfFrames;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>max vehicle speed</returns>
        public float GetMaxSpeed()
        {
            return maxSpeed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>spring force</returns>
        public float GetSpringForce()
        {
            return springForce;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Max RayCast length</returns>
        public float GetRaycastLength()
        {
            return allWheels[0].raycastLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current vehicle action</returns>
        public SpecialDriveActionTypes GetCurrentAction()
        {
            return currentAction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Wheel circumference</returns>
        public float GetWheelCircumference()
        {
            return allWheels[0].wheelCircumference;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Vehicle velocity vector</returns>
        public Vector3 GetVelocity()
        {
            return rb.velocity;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Current speed in kmh</returns>
        public float GetCurrentSpeed()
        {
            return GetVelocity().magnitude * 3.6f;
        }


        /// <summary>
        /// Used to verify is the current collider is included in other vehicle trigger
        /// </summary>
        /// <returns>first collider from collider list</returns>
        public Collider GetCollider()
        {
            return allColliders[0];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Trigger orientation</returns>
        public Vector3 GetHeading()
        {
            return frontTrigger.forward;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>vehicle orientation</returns>
        public Vector3 GetForwardVector()
        {
            return transform.forward;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>max steer</returns>
        public float GetMaxSteer()
        {
            return maxSteer;
        }


        /// <summary>
        /// Set the list index for current vehicle
        /// </summary>
        /// <param name="index">new list index</param>
        public void SetIndex(int index)
        {
            listIndex = index;
        }


        /// <summary>
        /// Get list index of the current vehicle
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            return listIndex;
        }


        /// <summary>
        /// Check if the vehicle is not in view
        /// </summary>
        /// <returns></returns>
        public bool CanBeRemoved()
        {
            return visibilityScript.IsNotInView();
        }


        /// <summary>
        /// A vehicle stopped reversing check for new action 
        /// </summary>
        public void VehicleStoppedReversing()
        {
            if (isInTrigger)
            {
                if (obstacleList.Count > 0)
                {
                    Collider other = obstacleList[0];
                    bool carHit = other.gameObject.layer == gameObject.layer;
                    int collidingVehicleIndex = -1;
                    if (carHit)
                    {
                        collidingVehicleIndex = other.attachedRigidbody.GetComponent<VehicleComponent>().GetIndex();
                    }
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, carHit, collidingVehicleIndex, false, false, DynamicObjectInteraction);
                }
                else
                {
                    isInTrigger = false;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>suspension position</returns>
        public float GetTargetCompression()
        {
            return suspensionPosition;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>current vehicle type</returns>
        public VehicleTypes GetVehicleType()
        {
            return vehicleType;
        }


        /// <summary>
        /// Creates a GameObject that is used to reach waypoints 
        /// </summary>
        /// <returns>the front wheel position of the vehicle</returns>
        public Transform GetFrontAxle()
        {
            if (frontAxle == null)
            {
                frontAxle = new GameObject("FrontAxle").transform;
                frontAxle.transform.SetParent(frontTrigger.parent);
                frontAxle.transform.position = frontTrigger.position;
            }
            return frontAxle;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>number of vehicle wheels</returns>
        public int GetNrOfWheels()
        {
            return allWheels.Length;
        }


        /// <summary>
        /// Set the new vehicle action
        /// </summary>
        /// <param name="currentAction"></param>
        public void SetCurrentAction(SpecialDriveActionTypes currentAction)
        {
            this.currentAction = currentAction;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>safe distance to change the lane</returns>
        public float GetSafeDistance()
        {
            return safeDistance;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>save distance to overtake</returns>
        public float GetOvertakeDistance()
        {
            return overtakeDistance;
        }


        /// <summary>
        /// CHeck trigger objects
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
            {
                bool carHit = other.gameObject.layer == gameObject.layer;
                int collidingVehicleIndex = -1;
                bool theySeeEachOther = false;
                //if a car was hit
                if (carHit)
                {
                    //if the obstacle is on the same layer but is not a car -> stop
                    if (other.attachedRigidbody.GetComponent<VehicleComponent>() == null)
                    {
                        NewColliderHit(other);
                        VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, false, collidingVehicleIndex, false, theySeeEachOther, DynamicObjectInteraction);
                        return;
                    }

                    //if the collider is a car set the object parameters and trigger new object event
                    NewColliderHit(other);
                    VehicleComponent otherVehicle = other.attachedRigidbody.GetComponent<VehicleComponent>();
                    collidingVehicleIndex = otherVehicle.GetIndex();
                    if (otherVehicle.AlreadyCollidingWith(allColliders))
                    {
                        theySeeEachOther = true;
                    }
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, carHit, collidingVehicleIndex, false, theySeeEachOther, null);
                }
                else
                {
                    //trigger the corresponding event based on object layer
                    if (buildingLayers == (buildingLayers | (1 << other.gameObject.layer)))
                    {
                        NewColliderHit(other);
                        VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, carHit, collidingVehicleIndex, false, theySeeEachOther, BuildingInteraction);
                    }
                    else
                    {
                        if (obstacleLayers == (obstacleLayers | (1 << other.gameObject.layer)))
                        {
                            NewColliderHit(other);
                            VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, carHit, collidingVehicleIndex, false, theySeeEachOther, DynamicObjectInteraction);
                        }
                        else
                        {
                            if (playerLayers == (playerLayers | (1 << other.gameObject.layer)))
                            {
                                NewColliderHit(other);
                                VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, carHit, collidingVehicleIndex, false, theySeeEachOther, PlayerInteraction);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Every time a new collider is hit it is added inside the list
        /// </summary>
        /// <param name="other"></param>
        void NewColliderHit(Collider other)
        {
            isInTrigger = true;
            if (!obstacleList.Contains(other))
            {
                obstacleList.Add(other);
            }
        }


        /// <summary>
        /// Remove a collider from the list
        /// </summary>
        /// <param name="other"></param>
        public virtual void OnTriggerExit(Collider other)
        {
            if (!other.isTrigger)
            {
                //TODO this should only trigger if objects of interest are doing trigger exit
                obstacleList.Remove(other);
                if (obstacleList.Count == 0)
                {
                    isInTrigger = false;
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(isInTrigger, listIndex, false, -1, false, false, null);
                }
            }
        }


        /// <summary>
        /// Check for collisions
        /// </summary>
        /// <param name="collision"></param>
        public virtual void OnCollisionEnter(Collision collision)
        {
            Rigidbody otherRb = collision.collider.attachedRigidbody;
            if (otherRb != null)
            {
                if (otherRb.gameObject.layer == gameObject.layer)
                {
                    VehicleComponent otherScript = otherRb.GetComponent<VehicleComponent>();
                    if (otherScript != null)
                    {
                        //Debug.Log("COLLISION " + listIndex + " " + otherScript.listIndex);
                        //Debug.Break();
                        VehicleEvents.TriggerVehicleCrashEvent(listIndex, otherScript.listIndex, true);
                    }
                }
            }
        }


        /// <summary>
        /// Check if current collider is from a new object
        /// </summary>
        /// <param name="colliders"></param>
        /// <returns></returns>
        private bool AlreadyCollidingWith(Collider[] colliders)
        {
            for (int i = 0; i < obstacleList.Count; i++)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (obstacleList[i] == colliders[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// When another vehicle changes his state, check if the current vehicle is affected and respond accordingly
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="collider"></param>
        /// <param name="newAction"></param>
        private void AVehicleChengedState(int vehicleIndex, Collider collider, SpecialDriveActionTypes newAction)
        {
            //if that vehicle is in the bot trigger
            if (obstacleList.Contains(collider))
            {
                if (newAction == SpecialDriveActionTypes.Destroyed)
                {
                    obstacleList.Remove(collider);
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(false, listIndex, false, vehicleIndex, false, false, null);
                }

                if (newAction == SpecialDriveActionTypes.Reverse || newAction == SpecialDriveActionTypes.AvoidReverse)
                {
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(true, listIndex, true, vehicleIndex, true, false, null);
                }

                if (newAction == SpecialDriveActionTypes.StopInDistance || newAction == SpecialDriveActionTypes.StopInPoint)
                {
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(true, listIndex, true, vehicleIndex, false, false, null);
                }

                if (newAction == SpecialDriveActionTypes.Follow)
                {
                    VehicleEvents.TriggeerNewObjectInTriggerEvent(true, listIndex, true, vehicleIndex, false, false, null);
                }
            }
        }

        public void ColliderRemoved(Collider collider)
        {
            if (obstacleList.Contains(collider))
            {
                OnTriggerExit(collider);
            }
        }


        //update the lights component if required
        #region Lights
        internal void SetMainLights(bool on)
        {
            if (on != lightsOn)
            {
                lightsOn = on;
            }
            if (vehicleLights)
            {
                vehicleLights.SetMainLights(on);
            }
        }

        public void SetReverseLights(bool active)
        {
            if (vehicleLights)
            {
                vehicleLights.SetReverseLights(active);
            }
        }

        public void SetBrakeLights(bool active)
        {
            if (vehicleLights)
            {
                vehicleLights.SetBrakeLights(active);
            }
        }

        public virtual void SetBlinker(BlinkType blinkType)
        {
            if (vehicleLights)
            {
                vehicleLights.SetBlinker(blinkType);
            }
        }

        public void UpdateLights(float realtimeSinceStartup)
        {
            if (vehicleLights)
            {
                vehicleLights.UpdateLights(realtimeSinceStartup);
            }
        }
        #endregion


        //update the sound component if required
        #region Sound
        public void UpdateEngineSound(float masterVolume)
        {
            if (engineSound)
            {
                engineSound.UpdateEngineSound(GetCurrentSpeed(), maxSpeed, masterVolume);
            }
        }
        #endregion


        /// <summary>
        /// Removes active events
        /// </summary>
        private void OnDestroy()
        {
            AIEvents.onVehicleChangedState -= AVehicleChengedState;
        }
    }
}