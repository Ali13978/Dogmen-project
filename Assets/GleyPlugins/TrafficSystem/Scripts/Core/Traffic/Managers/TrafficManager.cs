#if USE_GLEY_TRAFFIC
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using GleyUrbanAssets;
using System;

namespace GleyTrafficSystem
{
    /// <summary>
    /// This is the core class of the system, it controls everything else
    /// </summary>
    public class TrafficManager : UrbanManager
    {
        #region Variables
        //additional components
        private TrafficVehicles trafficVehicles;
        private DensityManager densityManager;
        private IntersectionManager intersectionManager;
        private WaypointManager waypointManager;
        private DrivingAI drivingAI;
        private VehiclePositioningSystem vehiclePositioningSystem;




        //transforms to update
        private TransformAccessArray vehicleTrigger;
        private TransformAccessArray wheelsOrigin;
        private TransformAccessArray wheelsGraphics;

        private NativeArray<float3> activeCameraPositions;

        //properties for each vehicle
        private NativeArray<SpecialDriveActionTypes> vehicleSpecialDriveAction;
        private NativeArray<VehicleTypes> vehicleType;
        private Rigidbody[] vehicleRigidbody;

        private NativeArray<float3> vehicleDownDirection;
        private NativeArray<float3> vehicleForwardDirection;
        private NativeArray<float3> vehicleRightDirection;
        private NativeArray<float3> vehicleTargetWaypointPosition;
        private NativeArray<float3> vehiclePosition;
        private NativeArray<float3> vehicleGroundDirection;
        private NativeArray<float3> vehicleBodyForce;
        private NativeArray<float3> vehicleVelocity;
        private NativeArray<float> vehicleSpringForce;
        private NativeArray<float> vehicleMaxSteer;
        private NativeArray<float> vehicleRotationAngle;
        private NativeArray<float> vehiclePowerStep;
        private NativeArray<float> vehicleBrakeStep;
        private NativeArray<float> vehicleActionValue;
        private NativeArray<float> distanceBetweenVehicles;
        private NativeArray<float> vehicleDrag;
        private NativeArray<float> vehicleMaxSpeed;
        private NativeArray<float> vehicleWheelDistance;
        private NativeArray<float> vehicleSteeringStep;
        private NativeArray<int> vehicleStartWheelIndex;//start index for the wheels of car i (dim nrOfCars)
        private NativeArray<int> vehicleEndWheelIndex; //number of wheels that car with index i has (nrOfCars)
        private NativeArray<int> vehicleListIndex;
        private NativeArray<int> vehicleGear;
        private NativeArray<bool> vehicleReadyToRemove;
        private NativeArray<bool> vehicleIsBraking;
        private NativeArray<bool> vehicleNeedWaypoint;
        private NativeArray<bool> ignoreVehicle;

        //properties for each wheel
        private NativeArray<RaycastHit> wheelRaycatsResult;
        private NativeArray<RaycastCommand> wheelRaycastCommand;
        private NativeArray<float3> wheelPosition;
        private NativeArray<float3> wheelNormalDirection;
        private NativeArray<float3> wheelSuspensionForce;
        private NativeArray<float> wheelRotation;
        private NativeArray<float> wheelRadius;
        private NativeArray<float> wheelRaycatsDistance;
        private NativeArray<float> wheelMaxSuspension;
        private NativeArray<int> wheelSign;
        private NativeArray<int> wheelAssociatedCar; //index of the car that contains the wheel
        private NativeArray<bool> wheelCanSteer;

        //properties that should be on each wheel
        private NativeArray<float> turnAngle;
        private NativeArray<float> raycastLengths;
        private NativeArray<float> wCircumferences;
        private NativeArray<float> targetSuspensionCompression;

        //jobs
        private UpdateWheelJob updateWheelJob;
        private UpdateTriggerJob updateTriggerJob;
        private DriveJob driveJob;
        private WheelJob wheelJob;
        private JobHandle raycastJobHandle;
        private JobHandle updateWheelJobHandle;
        private JobHandle updateTriggerJobHandle;
        private JobHandle driveJobHandle;
        private JobHandle wheelJobHandle;

        //additional properties
        private LayerMask roadLayers;
        private Transform[] activeCameras;
        private Vector3 forward;
        private Vector3 up;
        private float distanceToRemove;
        private float minDistanceToAdd;
        private int nrOfVehicles;
        private int nrOfJobs;
        private int indexToRemove;
        private int totalWheels;
        private int activeSquaresLevel;
        private int activeCameraIndex;
        private bool initialized;
#pragma warning disable 0649
        private bool drawBodyForces;
        private bool drawRaycasts;
        private bool debugDensity;
        private bool debugWaypoints;
        private bool debugDisabledWaypoints;
        private bool debug;
        private bool debugSpeed;
        private bool debugAI;
        private bool debugIntersections;
        private bool stopIntersectionUpdate;
#pragma warning restore 0649
        #endregion

        private static TrafficManager instance;
        public static TrafficManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject(Constants.trafficManager).AddComponent<TrafficManager>();
                    urbanManagerInstance = instance;
                }
                return instance;
            }
        }

       


        #region TrafficInitialization
        /// <summary>
        /// Initialize the traffic 
        /// </summary>
        /// <param name="activeCameras">camera that follows the player</param>
        /// <param name="nrOfVehicles">max number of traffic vehicles active in the same time</param>
        /// <param name="carPool">available vehicles asset</param>
        /// <param name="minDistanceToAdd">min distance from the player to add new vehicle</param>
        /// <param name="distanceToRemove">distance at which traffic vehicles can be removed</param>
        /// <param name="masterVolume">[-1,1] used to control the engine sound from your master volume</param>
        /// <param name="greenLightTime">roads green light duration in seconds</param>
        /// <param name="yelloLightTime">roads yellow light duration in seconds</param>
        public void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool vehiclePool, float minDistanceToAdd, float distanceToRemove, float masterVolume, float greenLightTime = -1, float yellowLightTime = -1)
        {
            //safety checks
            LayerSetup layerSetup = Resources.Load<LayerSetup>(Constants.layerSetupData);
            if (layerSetup == null)
            {
                Debug.LogError("Layers are not configured. Go to Window->Gley->Traffic System->Scene Setup->Layer Setup");
                return;
            }

            CurrentSceneData currentSceneData = CurrentSceneData.GetSceneInstance();
            if (currentSceneData.grid == null)
            {
                Debug.LogError("Scene data is null. Go to Window->Gley->Traffic System and Apply Settings");
                return;
            }

            if (currentSceneData.grid.Length == 0)
            {
                Debug.LogError("Scene grid is not set up correctly. Go to Window->Gley->Traffic System->Scene Setup->Grid Setup");
                return;
            }

            if (currentSceneData.allWaypoints.Length <= 0)
            {
                Debug.LogError("No waypoints found. Go to Window->Gley->Traffic System->Road Setup and create a road or make sure your settings are applied");
            }


            this.nrOfVehicles = nrOfVehicles;
            this.activeCameras = activeCameras;
            this.distanceToRemove = distanceToRemove * distanceToRemove;
            this.minDistanceToAdd = minDistanceToAdd;
            activeSquaresLevel = 1;

            roadLayers = layerSetup.roadLayers;
            up = Vector3.up;

            //compute total wheels
            trafficVehicles = gameObject.AddComponent<TrafficVehicles>().Initialize(vehiclePool, nrOfVehicles, layerSetup.buildingsLayers, layerSetup.obstaclesLayers, layerSetup.playerLayers, masterVolume);
            List<VehicleComponent> traffic = trafficVehicles.GetVehicleList();
            for (int i = 0; i < traffic.Count; i++)
            {
                totalWheels += traffic[i].allWheels.Length;
            }

            //initialize arrays
            wheelPosition = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelNormalDirection = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelRaycatsDistance = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelRadius = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelAssociatedCar = new NativeArray<int>(totalWheels, Allocator.Persistent);
            wheelCanSteer = new NativeArray<bool>(totalWheels, Allocator.Persistent);
            wheelSuspensionForce = new NativeArray<float3>(totalWheels, Allocator.Persistent);
            wheelMaxSuspension = new NativeArray<float>(totalWheels, Allocator.Persistent);
            wheelRaycatsResult = new NativeArray<RaycastHit>(totalWheels, Allocator.Persistent);
            wheelRaycastCommand = new NativeArray<RaycastCommand>(totalWheels, Allocator.Persistent);

            vehicleTrigger = new TransformAccessArray(nrOfVehicles);
            vehicleSpecialDriveAction = new NativeArray<SpecialDriveActionTypes>(nrOfVehicles, Allocator.Persistent);
            vehicleType = new NativeArray<VehicleTypes>(nrOfVehicles, Allocator.Persistent);
            vehicleBodyForce = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehiclePosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleGroundDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleDownDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleRightDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleForwardDirection = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleTargetWaypointPosition = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);
            vehicleVelocity = new NativeArray<float3>(nrOfVehicles, Allocator.Persistent);

            wheelRotation = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            turnAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleDrag = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleSteeringStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleMaxSpeed = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleWheelDistance = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehiclePowerStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleBrakeStep = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleSpringForce = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            raycastLengths = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            wCircumferences = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleRotationAngle = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleMaxSteer = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            vehicleActionValue = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            distanceBetweenVehicles = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            targetSuspensionCompression = new NativeArray<float>(nrOfVehicles, Allocator.Persistent);
            wheelSign = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleListIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleEndWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleStartWheelIndex = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleReadyToRemove = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleNeedWaypoint = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleIsBraking = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            ignoreVehicle = new NativeArray<bool>(nrOfVehicles, Allocator.Persistent);
            vehicleGear = new NativeArray<int>(nrOfVehicles, Allocator.Persistent);
            vehicleRigidbody = new Rigidbody[nrOfVehicles];

            //initialize debug settings for editor
#if UNITY_EDITOR
            DebugSettings debugSettings = DebugOptions.LoadOrCreateDebugSettings();
            drawBodyForces = debugSettings.drawBodyForces;
            drawRaycasts = debugSettings.drawRaycasts;
            debugDensity = debugSettings.debugDesnity;
            debugWaypoints = debugSettings.debugWaypoints;
            debugDisabledWaypoints = debugSettings.debugDisabledWaypoints;
            debug = debugSettings.debug;
            debugSpeed = debugSettings.debugSpeed;
            debugAI = debugSettings.debugAI;
            debugIntersections = debugSettings.debugIntersections;
            stopIntersectionUpdate = debugSettings.stopIntersectionUpdate;
#endif

            //initialize other managers
            PositionValidator positionValidator = gameObject.AddComponent<PositionValidator>().Initialize(this.activeCameras, layerSetup.trafficLayers, layerSetup.buildingsLayers, this.minDistanceToAdd, debugDensity);
            waypointManager = gameObject.AddComponent<WaypointManager>().Initialize(currentSceneData.allWaypoints, nrOfVehicles, debugWaypoints, debugDisabledWaypoints);
            vehiclePositioningSystem = gameObject.AddComponent<VehiclePositioningSystem>().Initialize(nrOfVehicles, waypointManager);
            drivingAI = gameObject.AddComponent<DrivingAI>().Initialize(nrOfVehicles, waypointManager, trafficVehicles, vehiclePositioningSystem, debug, debugSpeed, debugAI);

            //initialize all vehicles
            Transform[] tempWheelOrigin = new Transform[totalWheels];
            Transform[] tempWheelGraphic = new Transform[totalWheels];
            int wheelIndex = 0;
            for (int i = 0; i < nrOfVehicles; i++)
            {
                VehicleComponent vehicle = traffic[i];
                vehiclePositioningSystem.AddCar(vehicle.GetFrontAxle());
                vehicleTrigger.Add(vehicle.frontTrigger);
                vehicleRigidbody[i] = vehicle.rb;
                vehicleSteeringStep[i] = vehicle.GetSteeringStep();
                vehicleWheelDistance[i] = vehicle.wheelDistance;
                vehicleDrag[i] = vehicle.rb.drag;
                vehicleSpringForce[i] = vehicle.GetSpringForce();
                raycastLengths[i] = vehicle.GetRaycastLength();
                wCircumferences[i] = vehicle.GetWheelCircumference();
                vehicleMaxSteer[i] = vehicle.GetMaxSteer();
                vehicleStartWheelIndex[i] = wheelIndex;
                vehicleEndWheelIndex[i] = vehicleStartWheelIndex[i] + vehicle.GetNrOfWheels();

                for (int j = 0; j < vehicle.GetNrOfWheels(); j++)
                {
                    tempWheelOrigin[wheelIndex] = vehicle.allWheels[j].wheelTransform;
                    tempWheelGraphic[wheelIndex] = vehicle.allWheels[j].wheelTransform.GetChild(0);
                    wheelCanSteer[wheelIndex] = vehicle.allWheels[j].wheelPosition == Wheel.WheelPosition.Front;
                    wheelRadius[wheelIndex] = vehicle.allWheels[j].wheelRadius;
                    wheelMaxSuspension[wheelIndex] = vehicle.allWheels[j].maxSuspension;
                    wheelAssociatedCar[wheelIndex] = i;
                    wheelIndex++;
                }

                vehicleListIndex[i] = vehicle.GetIndex();
                vehicleType[i] = vehicle.GetVehicleType();
                targetSuspensionCompression[i] = vehicle.GetTargetCompression();
            }

            wheelsOrigin = new TransformAccessArray(tempWheelOrigin);
            wheelsGraphics = new TransformAccessArray(tempWheelGraphic);

            //set the number of jobs based on processor count
            if (SystemInfo.processorCount != 0)
            {
                nrOfJobs = totalWheels / SystemInfo.processorCount + 1;
            }
            else
            {
                nrOfJobs = nrOfVehicles / 4;
            }

            //add events
            AIEvents.onChangeDrivingState += UpdateDrivingState;
            AIEvents.onChangeDestination += DestinationChanged;
            DensityEvents.onVehicleAdded += NewVehicleAdded;

            //initialize the remaining managers
            activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
            for (int i = 0; i < activeCameraPositions.Length; i++)
            {
                activeCameraPositions[i] = activeCameras[i].position;
            }
            AddGridManager(currentSceneData, activeCameraPositions);
            densityManager = gameObject.AddComponent<DensityManager>().Initialize(trafficVehicles, waypointManager, gridManager, positionValidator, activeCameraPositions, nrOfVehicles, activeCameras[0].position, activeCameras[0].forward);

            intersectionManager = gameObject.AddComponent<IntersectionManager>().Initialize(gridManager.GetAllIntersections(), gridManager.GetActiveIntersections(), waypointManager, greenLightTime, yellowLightTime, debugIntersections, stopIntersectionUpdate);
            initialized = true;
        }

        internal List<VehicleComponent> GetVehicleList()
        {
            return trafficVehicles.GetVehicleList();
        }

        internal void AddVehicle(Vector3 position, VehicleTypes type)
        {
            densityManager.AddVehicleAtPosition(position, type);
        }

        internal void SetIntersectionRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain)
        {
            intersectionManager.SetRoadToGreen(intersectionName, roadIndex, doNotChangeAgain);
        }

        internal void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            intersectionManager.SetTrafficLightsBehaviour(trafficLightsBehaviour);
        }

        internal void TriggerColliderRemovedEvent(Collider collider)
        {
            trafficVehicles.TriggerColliderRemovedEvent(collider);
        }


        #endregion


        #region API Methods
        /// <summary>
        /// Removes the vehicles on a given circular area
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void ClearTrafficOnArea(Vector3 center, float radius)
        {
            if (!initialized)
                return;

            float sqrRadius = radius * radius;
            for (int i = 0; i < vehiclePosition.Length; i++)
            {
                if (vehicleRigidbody[i].gameObject.activeSelf)
                {
                    //uses math because of the float3 array
                    if (math.distancesq(center, vehiclePosition[i]) < sqrRadius)
                    {
                        RemoveVehicle(i);
                    }
                }
            }
        }


        /// <summary>
        /// Remove a specific vehicle from the scene
        /// </summary>
        /// <param name="index">index of the vehicle to remove</param>
        public void RemoveVehicle(int index)
        {
            if (!initialized)
                return;

            intersectionManager.RemoveCarFromIntersection(vehicleListIndex[index]);
            densityManager.RemoveVehicle(vehicleListIndex[index]);
        }

        /// <summary>
        /// Remove a specific vehicle from the scene
        /// </summary>
        /// <param name="index">index of the vehicle to remove</param>
        public void RemoveVehicle(GameObject vehicle)
        {
            if (!initialized)
                return;

            int index = GetVehicleIndex(vehicle);
            if (index != -1)
            {
                RemoveVehicle(index);
            }
            else
            {
                Debug.Log("Vehicle not found");
            }
        }


        /// <summary>
        /// Disable all waypoints on the specified area to stop vehicles from going in a certain area based on some app design events
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void DisableAreaWaypoints(Vector3 center, float radius)
        {
            if (!initialized)
                return;

            float sqrRadius = radius * radius;
            densityManager.DisableAreaWaypoints(center, sqrRadius);
        }


        /// <summary>
        /// Enable all disabled area waypoints
        /// </summary>
        public void EnableAllWaypoints()
        {
            if (!initialized)
                return;

            waypointManager.EnableAllWaypoints();
        }


        /// <summary>
        /// Modify the max number of active vehicles
        /// </summary>
        /// <param name="nrOfVehicles">new max number of vehicles, needs to be less than the initialization max number of vehicles</param>
        public void SetTrafficDensity(int nrOfCars)
        {
            if (!initialized)
                return;

            densityManager.UpdateMaxCars(nrOfCars);
        }


        /// <summary>
        /// Turn all vehicle lights on or off
        /// </summary>
        /// <param name="on">if true, lights are on</param>
        public void UpdateVehicleLights(bool on)
        {
            if (!initialized)
                return;

            trafficVehicles.UpdateVehicleLights(on);
        }


        /// <summary>
        /// Control the engine volume from your master volume
        /// </summary>
        /// <param name="volume">current engine AudioSource volume</param>
        public void SetMasterVolume(float volume)
        {
            if (!initialized)
                return;

            trafficVehicles.UpdateMasterVolume(volume);
        }


        /// <summary>
        /// Update active camera that is used to remove vehicles when are not in view
        /// </summary>
        /// <param name="activeCamera">represents the camera or the player prefab</param>
        public void UpdateCamera(Transform[] activeCameras)
        {
            if (!initialized)
                return;

            if (activeCameras.Length != activeCameraPositions.Length)
            {
                activeCameraPositions = new NativeArray<float3>(activeCameras.Length, Allocator.Persistent);
            }

            this.activeCameras = activeCameras;
            densityManager.UpdateCameraPositions(activeCameras);

        }


        public void SetActiveSquaresLevel(int activeSquaresLevel)
        {
            if (!initialized)
                return;

            this.activeSquaresLevel = activeSquaresLevel;
        }


        internal void SetSpawnWaypointSelectorDelegate(SpawnWaypointSelector spawnWaypointSelector)
        {
            if (!initialized)
                return;

            gridManager.SetSpawnWaypointSelector(spawnWaypointSelector);
        }

        public void SetPlayerInteractionDelegate(EnvironmentInteraction playerInteraction)
        {
            if (!initialized)
                return;

            trafficVehicles.SetPlayerInteractionDelegate(playerInteraction);
        }

        public void SetBuildingInteractionDelegate(EnvironmentInteraction buildingInteraction)
        {
            if (!initialized)
                return;

            trafficVehicles.SetBuildingInteractionDelegate(buildingInteraction);
        }

        public void SetDynamicObjectInteractionDelegate(EnvironmentInteraction dynamicObjectInteraction)
        {
            if (!initialized)
                return;

            trafficVehicles.SetDynamicObjectInteractionDelegate(dynamicObjectInteraction);
        }

        public void StopVehicleDriving(GameObject vehicle)
        {
            if (!initialized)
                return;

            for (int i = 0; i < vehicleRigidbody.Length; i++)
            {
                if (vehicleRigidbody[i].gameObject == vehicle)
                {
                    ignoreVehicle[i] = true;
                }
            }
        }


        int GetVehicleIndex(GameObject vehicle)
        {
            for (int i = 0; i < vehicleRigidbody.Length; i++)
            {
                if (vehicleRigidbody[i].gameObject == vehicle)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion


        #region EventHandlers
        /// <summary>
        /// Called every time a new vehicle is enabled
        /// </summary>
        /// <param name="vehicleIndex">index of the vehicle</param>
        /// <param name="targetWaypointPosition">target position</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="powerStep">acceleration power</param>
        /// <param name="brakeStep">brake power</param>
        private void NewVehicleAdded(int vehicleIndex)
        {
            //set new vehicle parameters
            vehicleTargetWaypointPosition[vehicleIndex] = waypointManager.GetTargetWaypointPosition(vehicleIndex);

            vehiclePowerStep[vehicleIndex] = trafficVehicles.GetPowerStep(vehicleIndex);
            vehicleBrakeStep[vehicleIndex] = trafficVehicles.GetBrakeStep(vehicleIndex);

            vehicleIsBraking[vehicleIndex] = false;
            vehicleNeedWaypoint[vehicleIndex] = false;
            ignoreVehicle[vehicleIndex] = false;
            vehicleGear[vehicleIndex] = 1;
            turnAngle[vehicleIndex] = 0;

            //reset AI
            drivingAI.VehicleActivated(vehicleIndex);
            vehicleMaxSpeed[vehicleIndex] = drivingAI.GetMaxSpeedMS(vehicleIndex);
            //set initial velocity
            vehicleRigidbody[vehicleIndex].velocity = vehiclePositioningSystem.GetForwardVector(vehicleIndex) * vehicleMaxSpeed[vehicleIndex] / 2;
        }


        /// <summary>
        /// Called every time a vehicle state changes
        /// </summary>
        /// <param name="vehicleIndex">vehicle index</param>
        /// <param name="action">new action</param>
        /// <param name="actionValue">time to execute the action</param>
        private void UpdateDrivingState(int vehicleIndex, SpecialDriveActionTypes action, float actionValue)
        {
            trafficVehicles.SetCurrentAction(vehicleIndex, action);
            vehicleSpecialDriveAction[vehicleIndex] = action;
            vehicleActionValue[vehicleIndex] = actionValue;
            if (action == SpecialDriveActionTypes.AvoidReverse)
            {
                wheelSign[vehicleIndex] = (int)Mathf.Sign(turnAngle[vehicleIndex]);
            }
        }


        /// <summary>
        /// Called when waypoint changes
        /// </summary>
        /// <param name="vehicleIndex">vehicle index</param>
        /// <param name="targetWaypointPosition">new waypoint position</param>
        /// <param name="maxSpeed">new possible speed</param>
        /// <param name="blinkType">blinking is required or not</param>
        private void DestinationChanged(int vehicleIndex)
        {
            vehicleNeedWaypoint[vehicleIndex] = false;
            vehicleTargetWaypointPosition[vehicleIndex] = waypointManager.GetTargetWaypointPosition(vehicleIndex);
            vehicleMaxSpeed[vehicleIndex] = drivingAI.GetMaxSpeedMS(vehicleIndex);
            trafficVehicles.SetBlinkLights(vehicleIndex, drivingAI.GetBlinkType(vehicleIndex));
        }
        #endregion


        private void FixedUpdate()
        {
            if (!initialized)
                return;

            #region Suspensions

            //for each wheel check where the ground is by performing a RayCast downwards using job system
            for (int i = 0; i < totalWheels; i++)
            {
                wheelPosition[i] = wheelsOrigin[i].position;
            }

            for (int i = 0; i < nrOfVehicles; i++)
            {
                vehicleVelocity[i] = vehicleRigidbody[i].velocity;
                vehicleDownDirection[i] = -vehiclePositioningSystem.GetUpVector(i);
                forward = vehiclePositioningSystem.GetForwardVector(i);
                forward.y = 0;
                vehicleForwardDirection[i] = forward;
                vehicleRightDirection[i] = vehiclePositioningSystem.GetRightVector(i);
                vehiclePosition[i] = vehiclePositioningSystem.GetPosition(i);
                vehicleGroundDirection[i] = trafficVehicles.GetGroundDirection(i);
            }

            for (int i = 0; i < totalWheels; i++)
            {
                wheelRaycastCommand[i] = new RaycastCommand(wheelPosition[i], vehicleDownDirection[wheelAssociatedCar[i]], raycastLengths[wheelAssociatedCar[i]], roadLayers);
            }
            raycastJobHandle = RaycastCommand.ScheduleBatch(wheelRaycastCommand, wheelRaycatsResult, nrOfJobs, default);
            raycastJobHandle.Complete();

            for (int i = 0; i < totalWheels; i++)
            {
                wheelRaycatsDistance[i] = wheelRaycatsResult[i].distance;
                wheelNormalDirection[i] = wheelRaycatsResult[i].normal;
            }
            #endregion

            #region Driving

            //execute job for wheel turn and driving
            wheelJob = new WheelJob()
            {
                wheelSuspensionForce = wheelSuspensionForce,
                springForces = vehicleSpringForce,
                wheelMaxSuspension = wheelMaxSuspension,
                wheelRaycastDistance = wheelRaycatsDistance,
                wheelRadius = wheelRadius,
                wheelNormalDirection = wheelNormalDirection,
                targetCompression = targetSuspensionCompression,
                nrOfCarWheels = vehicleEndWheelIndex,
                startWheelIndex = vehicleStartWheelIndex,
                wheelAssociatedCar = wheelAssociatedCar
            };

            driveJob = new DriveJob()
            {
                wheelCircumferences = wCircumferences,
                carVelocity = vehicleVelocity,
                fixedDeltaTime = Time.fixedDeltaTime,
                targetWaypointPosition = vehicleTargetWaypointPosition,
                allBotsPosition = vehiclePosition,
                maxSteer = vehicleMaxSteer,
                forwardDirection = vehicleForwardDirection,
                worldUp = up,
                wheelRotation = wheelRotation,
                turnAngle = turnAngle,
                vehicleRotationAngle = vehicleRotationAngle,
                readyToRemove = vehicleReadyToRemove,
                needsWaypoint = vehicleNeedWaypoint,
                distanceToRemove = distanceToRemove,
                cameraPositions = activeCameraPositions,
                bodyForce = vehicleBodyForce,
                downDirection = vehicleDownDirection,
                rightDirection = vehicleRightDirection,
                powerStep = vehiclePowerStep,
                brakeStep = vehicleBrakeStep,
                specialDriveAction = vehicleSpecialDriveAction,
                actionValue = vehicleActionValue,
                distanceBetweenVehicles = distanceBetweenVehicles,
                wheelSign = wheelSign,
                isBraking = vehicleIsBraking,
                drag = vehicleDrag,
                maxSpeed = vehicleMaxSpeed,
                gear = vehicleGear,
                groundDirection = vehicleGroundDirection,
                steeringStep = vehicleSteeringStep,
                wheelDistance = vehicleWheelDistance,
            };

            wheelJobHandle = wheelJob.Schedule(totalWheels, nrOfJobs);
            driveJobHandle = driveJob.Schedule(nrOfVehicles, nrOfJobs);
            wheelJobHandle.Complete();
            driveJobHandle.Complete();

            //store job values
            wheelSuspensionForce = wheelJob.wheelSuspensionForce;
            wheelRotation = driveJob.wheelRotation;
            turnAngle = driveJob.turnAngle;
            vehicleRotationAngle = driveJob.vehicleRotationAngle;
            vehicleReadyToRemove = driveJob.readyToRemove;
            vehicleNeedWaypoint = driveJob.needsWaypoint;
            vehicleBodyForce = driveJob.bodyForce;
            vehicleActionValue = driveJob.actionValue;
            vehicleIsBraking = driveJob.isBraking;
            vehicleGear = driveJob.gear;


            //make vehicle actions based on job results
            for (int i = 0; i < nrOfVehicles; i++)
            {
                if (!vehicleRigidbody[i].IsSleeping())
                {
                    int groundedWheels = 0;
                    for (int j = vehicleStartWheelIndex[i]; j < vehicleEndWheelIndex[i]; j++)
                    {
                        if (wheelRaycatsDistance[j] != 0)
                        {
                            //if wheel is grounded apply suspension force
                            groundedWheels++;
                            vehicleRigidbody[i].AddForceAtPosition(wheelSuspensionForce[j], wheelPosition[j]);
                        }
                        else
                        {
                            //if the wheel is not grounded apply additional gravity to stabilize the vehicle for a more realistic movement
                            vehicleRigidbody[i].AddForceAtPosition(Physics.gravity * vehicleRigidbody[i].mass / (vehicleEndWheelIndex[i] - vehicleStartWheelIndex[i]), wheelPosition[j]);
                        }
                    }
                    if (ignoreVehicle[i] == true)
                        continue;
                    //apply traction and rotation forces
                    if (groundedWheels != 0)
                    {
                        vehicleRigidbody[i].AddForce(vehicleBodyForce[i] * ((float)groundedWheels / (vehicleEndWheelIndex[i] - vehicleStartWheelIndex[i])), ForceMode.VelocityChange);
                        vehicleRigidbody[i].MoveRotation(vehicleRigidbody[i].rotation * Quaternion.Euler(0, vehicleRotationAngle[i], 0));
                    }
                    //request new waypoint if needed
                    if (vehicleNeedWaypoint[i] == true)
                    {
                        drivingAI.WaypointRequested(i, vehicleType[i]);
                    }


                    //adapt speed to the front vehicle
                    if (vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.Overtake || vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.Follow)
                    {
                        vehicleMaxSpeed[i] = drivingAI.GetMaxSpeedMS(i);
                        if (vehicleMaxSpeed[i] < 2)
                        {
                            drivingAI.NewDriveActionArrived(i, SpecialDriveActionTypes.StopInDistance, true);
                        }
                        distanceBetweenVehicles[i] = drivingAI.GetStopDistance(i);
                    }

                    //if current action is finished set a new action
                    if (vehicleActionValue[i] < 0)
                    {
                        //TODO This if should be inside Driving AI
                        if (vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.StopNow)
                        {
                            VehicleEvents.TriggerVehicleCrashEvent(i, i, false);
                        }
                        if (vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.Reverse)
                        {
                            drivingAI.ReverseDone(i);
                            trafficVehicles.CurrentVehicleActionDone(i);
                        }

                        if (vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.TempStop)
                        {
                            drivingAI.TempStopDone(i);
                            trafficVehicles.CurrentVehicleActionDone(i);
                        }

                        if (vehicleSpecialDriveAction[i] == SpecialDriveActionTypes.Follow)
                        {
                            drivingAI.Overtake(i);
                        }
                    }
                    //update reverse lights
                    if (vehicleGear[i] < 0)
                    {
                        trafficVehicles.SetReverseLights(i, true);
                    }
                    else
                    {
                        trafficVehicles.SetReverseLights(i, false);
                    }

                    //update engine and lights components
                    trafficVehicles.UpdateVehicleScripts(i);
                }
            }
            #endregion
        }


        private void Update()
        {
            if (!initialized)
                return;

            //update brake lights
            for (int i = 0; i < nrOfVehicles; i++)
            {
                trafficVehicles.SetBrakeLights(i, vehicleIsBraking[i]);
            }

            #region WheelUpdate
            //update wheel graphics
            for (int i = 0; i < totalWheels; i++)
            {
                wheelPosition[i] = wheelsOrigin[i].position;
            }

            updateWheelJob = new UpdateWheelJob()
            {
                wheelsOrigin = wheelPosition,
                downDirection = vehicleDownDirection,
                wheelRotation = wheelRotation,
                turnAngle = turnAngle,
                wheelRadius = wheelRadius,
                maxSuspension = wheelMaxSuspension,
                raycastDistance = wheelRaycatsDistance,
                nrOfCars = nrOfVehicles,
                canSteer = wheelCanSteer,
                carIndex = wheelAssociatedCar
            };
            updateWheelJobHandle = updateWheelJob.Schedule(wheelsGraphics);
            updateWheelJobHandle.Complete();
            #endregion

            #region TriggerUpdate
            //update trigger orientation
            updateTriggerJob = new UpdateTriggerJob()
            {
                turnAngle = turnAngle,
                specialDriveAction = vehicleSpecialDriveAction
            };
            updateTriggerJobHandle = updateTriggerJob.Schedule(vehicleTrigger);
            updateTriggerJobHandle.Complete();
            #endregion

            #region RemoveVehicles
            //remove vehicles that are too far away and not in view
            indexToRemove++;
            if (indexToRemove == nrOfVehicles)
            {
                indexToRemove = 0;
            }
            activeCameraIndex = UnityEngine.Random.Range(0, activeCameraPositions.Length);
            densityManager.UpdateVehicleDensity(activeCameras[0].position, activeCameras[0].forward, activeCameraIndex);

            if (vehicleReadyToRemove[indexToRemove] == true)
            {
                if (vehicleRigidbody[indexToRemove].gameObject.activeSelf)
                {
                    if (trafficVehicles.CanBeRemoved(vehicleListIndex[indexToRemove]) == true)
                    {
                        RemoveVehicle(indexToRemove);
                        vehicleReadyToRemove[indexToRemove] = false;
                    }
                }
            }
            #endregion

            //update additional managers
            for (int i = 0; i < activeCameras.Length; i++)
            {
                activeCameraPositions[i] = activeCameras[i].transform.position;
            }
            intersectionManager.UpdateIntersections();
            gridManager.UpdateGrid(activeSquaresLevel, activeCameraPositions);

            #region Debug
#if UNITY_EDITOR
            //draw debug forces if requested
            if (drawBodyForces)
            {
                for (int i = 0; i < nrOfVehicles; i++)
                {
                    Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), trafficVehicles.GetVelocity(i), Color.red);
                    Debug.DrawRay(vehicleRigidbody[i].transform.TransformPoint(vehicleRigidbody[i].centerOfMass), vehicleBodyForce[i], Color.green, Time.deltaTime, false);
                }

                for (int j = 0; j < totalWheels; j++)
                {
                    Debug.DrawRay(wheelPosition[j], wheelSuspensionForce[j] / trafficVehicles.GetSpringForce(wheelAssociatedCar[j]), Color.yellow);
                }
            }

            if (drawRaycasts)
            {
                for (int i = 0; i < totalWheels; i++)
                {
                    Debug.DrawRay(wheelPosition[i], wheelRaycatsResult[i].normal, Color.green);
                    Debug.DrawRay(wheelPosition[i], vehicleDownDirection[wheelAssociatedCar[i]] * wheelRaycatsResult[i].distance, Color.blue);
                }
            }
#endif
            #endregion
        }


        #region Cleanup
        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            vehicleSpringForce.Dispose();
            raycastLengths.Dispose();
            wCircumferences.Dispose();
            wheelRadius.Dispose();
            vehicleVelocity.Dispose();
            vehicleMaxSteer.Dispose();
            wheelsOrigin.Dispose();
            wheelsGraphics.Dispose();
            wheelPosition.Dispose();
            wheelRotation.Dispose();
            turnAngle.Dispose();
            wheelRaycatsResult.Dispose();
            wheelRaycastCommand.Dispose();
            wheelCanSteer.Dispose();
            wheelAssociatedCar.Dispose();
            vehicleEndWheelIndex.Dispose();
            vehicleStartWheelIndex.Dispose();
            vehicleDownDirection.Dispose();
            vehicleForwardDirection.Dispose();
            vehicleRotationAngle.Dispose();
            vehicleRightDirection.Dispose();
            vehicleTargetWaypointPosition.Dispose();
            vehiclePosition.Dispose();
            vehicleGroundDirection.Dispose();
            vehicleReadyToRemove.Dispose();
            vehicleListIndex.Dispose();
            vehicleNeedWaypoint.Dispose();
            wheelRaycatsDistance.Dispose();
            wheelNormalDirection.Dispose();
            wheelMaxSuspension.Dispose();
            targetSuspensionCompression.Dispose();
            wheelSuspensionForce.Dispose();
            vehicleBodyForce.Dispose();
            vehicleSteeringStep.Dispose();
            vehicleGear.Dispose();
            vehicleDrag.Dispose();
            vehicleMaxSpeed.Dispose();
            vehicleWheelDistance.Dispose();
            vehiclePowerStep.Dispose();
            vehicleBrakeStep.Dispose();
            vehicleTrigger.Dispose();
            vehicleSpecialDriveAction.Dispose();
            vehicleType.Dispose();
            vehicleActionValue.Dispose();
            distanceBetweenVehicles.Dispose();
            wheelSign.Dispose();
            vehicleIsBraking.Dispose();
            ignoreVehicle.Dispose();
            activeCameraPositions.Dispose();

            AIEvents.onChangeDrivingState -= UpdateDrivingState;
            AIEvents.onChangeDestination -= DestinationChanged;
            DensityEvents.onVehicleAdded -= NewVehicleAdded;
        }
        #endregion
    }
}
#endif