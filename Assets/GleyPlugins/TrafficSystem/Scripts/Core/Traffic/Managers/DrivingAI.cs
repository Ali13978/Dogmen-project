using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GleyTrafficSystem
{
    /// <summary>
    /// Decides what the vehicle will do next based on received information
    /// </summary>
    public class DrivingAI : MonoBehaviour
    {
        private WaypointManager waypointManager;
        private TrafficVehicles trafficVehicles;
        private VehiclePositioningSystem vehiclePositioningSystem;
        private DriveActions[] driveActions;
        private BlinkType[] blinkTypes;
        private int[] vehicleToFollow;
        private float[] waypointSpeed;
        private float[] maxVehicleSpeed;
        private bool actionsDebug;
        private bool speedDebug;
        private bool aiDebug;

#if UNITY_EDITOR
        private string[] newObjDebugText;
        private string[] triggerProcessText;
        private string[] collisionProcessText;
#endif

        //all available vehicle actions
        SpecialDriveActionTypes[] currentActiveAction;
        SpecialDriveActionTypes[] movingActions = new SpecialDriveActionTypes[]
        {
            SpecialDriveActionTypes.AvoidForward,
            SpecialDriveActionTypes.AvoidReverse,
            SpecialDriveActionTypes.Reverse,
            SpecialDriveActionTypes.StopInDistance,
            SpecialDriveActionTypes.TempStop,
            SpecialDriveActionTypes.Follow,
            SpecialDriveActionTypes.Overtake
        };

        SpecialDriveActionTypes[] waypointActions = new SpecialDriveActionTypes[]
        {
            SpecialDriveActionTypes.StopInPoint,
            SpecialDriveActionTypes.GiveWay
        };

        SpecialDriveActionTypes[] collisionActions = new SpecialDriveActionTypes[]
        {
            SpecialDriveActionTypes.StopNow,
            SpecialDriveActionTypes.Follow,
            SpecialDriveActionTypes.Overtake
        };


        /// <summary>
        /// Initialize Driving AI
        /// </summary>
        /// <param name="nrOfVehicles"></param>
        /// <param name="waypointManager"></param>
        /// <param name="trafficVehicles"></param>
        /// <param name="vehiclePositioningSystem"></param>
        /// <param name="actionsDebug"></param>
        /// <param name="speedDebug"></param>
        /// <param name="aiDebug"></param>
        /// <returns></returns>
        public DrivingAI Initialize(int nrOfVehicles, WaypointManager waypointManager, TrafficVehicles trafficVehicles, VehiclePositioningSystem vehiclePositioningSystem, bool actionsDebug, bool speedDebug, bool aiDebug)
        {
            this.waypointManager = waypointManager;
            this.trafficVehicles = trafficVehicles;
            this.vehiclePositioningSystem = vehiclePositioningSystem;
            this.actionsDebug = actionsDebug;
            this.speedDebug = speedDebug;
            this.aiDebug = aiDebug;

            driveActions = new DriveActions[nrOfVehicles];

            vehicleToFollow = new int[nrOfVehicles];
            waypointSpeed = new float[nrOfVehicles];
            maxVehicleSpeed = new float[nrOfVehicles];
            blinkTypes = new BlinkType[nrOfVehicles];

#if UNITY_EDITOR
            newObjDebugText = new string[nrOfVehicles];
            triggerProcessText = new string[nrOfVehicles];
            collisionProcessText = new string[nrOfVehicles];
#endif

            currentActiveAction = new SpecialDriveActionTypes[nrOfVehicles];


            VehicleEvents.onNewObjectInTrigger += NewObjectInTriggerHandler;
            VehicleEvents.onVehicleCrash += VehicleCrashHandler;
            GleyUrbanAssets.WaypointEvents.onStopStateChanged += StopStateChanged;
            GleyUrbanAssets.WaypointEvents.onGiveWayStateChanged += GiveWayStateChanged;


            return this;
        }


        /// <summary>
        /// Reset all pending actions, used when a vehicle is respawned
        /// </summary>
        /// <param name="index"></param>
        public void VehicleActivated(int index)
        {
#if UNITY_EDITOR
            newObjDebugText[index] = "";
            triggerProcessText[index] = "";
            collisionProcessText[index] = "";
#endif
            waypointSpeed[index] = waypointManager.GetMaxSpeed(index);
            vehicleToFollow[index] = -1;
            maxVehicleSpeed[index] = trafficVehicles.GetMaxSpeed(index);

            driveActions[index].activeActions = new List<SpecialDriveActionTypes>();
            currentActiveAction[index] = SpecialDriveActionTypes.Forward;
            blinkTypes[index] = BlinkType.Stop;
            AIEvents.TriggetChangeDrivingStateEvent(index, currentActiveAction[index], GetActionValue(currentActiveAction[index]));
        }


        /// <summary>
        /// Called by vehicle every time a new object enters or exits from trigger
        /// </summary>
        /// <param name="isInTrigger"></param>
        /// <param name="index"></param>
        /// <param name="isVehicle"></param>
        /// <param name="collidingVehicleIndex"></param>
        /// <param name="reverse"></param>
        /// <param name="theySeeEachOther"></param>
        private void NewObjectInTriggerHandler(bool isInTrigger, int index, bool isVehicle, int collidingVehicleIndex, bool reverse, bool theySeeEachOther, EnvironmentInteraction environmentInteraction)
        {
#if UNITY_EDITOR
            newObjDebugText[index] = "Trigger->";
#endif
            if (isInTrigger)
            {
                if (isVehicle)
                {
#if UNITY_EDITOR
                    newObjDebugText[index] += "Is vehicle->";
#endif
                    if (!theySeeEachOther)
                    {

                        //followSpeed[index] = GetFollowSpeed(collidingVehicleIndex);
#if UNITY_EDITOR
                        newObjDebugText[index] += "Don`t see each other->Follow index" + collidingVehicleIndex;
#endif
                        NewDriveActionArrived(index, GetTriggerAction(index, collidingVehicleIndex, reverse), true);

                    }
                    else
                    {
                        NewDriveActionArrived(index, SpecialDriveActionTypes.StopNow, true);
                    }
                }
                else
                {
                    if (environmentInteraction != null)
                    {
                        NewDriveActionArrived(index, environmentInteraction(), true);
                    }
                }
            }
            else
            {
#if UNITY_EDITOR
                newObjDebugText[index] += "Removed";
#endif
                NewDriveActionArrived(index, SpecialDriveActionTypes.Continue, false);
            }
        }


        /// <summary>
        /// Based on position, heading, and speed decide what is the next action of the vehicle
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="otherIndex"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        SpecialDriveActionTypes GetTriggerAction(int myIndex, int otherIndex, bool reverse)
        {
            //if reverse is true, means that other car is reversing so I have to reverse
            if (reverse)
            {
                return SpecialDriveActionTypes.Reverse;
            }

            bool sameOrientation = vehiclePositioningSystem.IsSameOrientation(trafficVehicles.GetHeading(myIndex), trafficVehicles.GetHeading(otherIndex));

#if UNITY_EDITOR
            triggerProcessText[myIndex] = "has same orientation(front trigger) with " + otherIndex + " " + sameOrientation;
#endif

            //if other car is stationary
            if (trafficVehicles.GetCurrentAction(otherIndex) == SpecialDriveActionTypes.StopInDistance || trafficVehicles.GetCurrentAction(otherIndex) == SpecialDriveActionTypes.StopInPoint || trafficVehicles.GetCurrentAction(otherIndex) == SpecialDriveActionTypes.GiveWay)
            {
                if (sameOrientation)
                {
                    //if the orientation is the same I stop too
                    return SpecialDriveActionTypes.StopInDistance;
                }
            }
            else
            {
                bool sameHeading = vehiclePositioningSystem.IsSameHeading(trafficVehicles.GetForwardVector(otherIndex), trafficVehicles.GetForwardVector(myIndex));
                bool otherIsGoingForward = vehiclePositioningSystem.IsGoingForward(trafficVehicles.GetVelocity(otherIndex), trafficVehicles.GetHeading(otherIndex));
#if UNITY_EDITOR
                triggerProcessText[myIndex] += "\nhas same heading (velocity) with " + otherIndex + " " + sameHeading +
                    "\nother is going forward " + otherIsGoingForward;
#endif


                if (sameOrientation == false && sameHeading == false)
                {
                    //not same orientation -> going in opposite direction-> try to avoid it
                    return SpecialDriveActionTypes.AvoidForward;
                }
                else
                {
                    //same orientation but different moving direction 
                    if (otherIsGoingForward == false)
                    {
                        // other car is going in reverse so I should also
                        return SpecialDriveActionTypes.Reverse;
                    }
                }

                if (sameHeading == false)
                {
                    //going back and hit something -> wait
                    return SpecialDriveActionTypes.TempStop;
                }
                else
                {
                    //follow the car in front
                    if (trafficVehicles.GetVelocity(myIndex).sqrMagnitude > 5 && trafficVehicles.GetVelocity(otherIndex).sqrMagnitude > 5)
                    {
                        vehicleToFollow[myIndex] = otherIndex;
                        return SpecialDriveActionTypes.Follow;
                    }
                }
                //if nothing worked, stop in distance
                return SpecialDriveActionTypes.StopInDistance;
            }
            //continue forward
            return SpecialDriveActionTypes.Forward;
        }


        /// <summary>
        /// Called when 2 vehicles hit each other
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="otherIndex"></param>
        /// <param name="addAction">if false resume driving, else check possibilities</param>
        private void VehicleCrashHandler(int myIndex, int otherIndex, bool addAction)
        {
            if (addAction)
            {
                //Debug.Log("my index " + currentActiveAction[myIndex] + " otherindex " + currentActiveAction[otherIndex]);
                //string text = "Actions " + myIndex;
                //for (int i = 0; i < driveActions[myIndex].activeActions.Count; i++)
                //{
                //    text += " " + driveActions[myIndex].activeActions[i];
                //}
                //Debug.Log(text);
                //text = "Actions " + otherIndex;
                //for (int i = 0; i < driveActions[otherIndex].activeActions.Count; i++)
                //{
                //    text += " " + driveActions[otherIndex].activeActions[i];
                //}
                //Debug.Log(text);
                //determine relative position and moving directions
                int inFront = vehiclePositioningSystem.IsInFront(myIndex, otherIndex);
                bool sameOrientation = vehiclePositioningSystem.IsSameOrientation(trafficVehicles.GetHeading(myIndex), trafficVehicles.GetHeading(otherIndex));
                bool sameHeading = vehiclePositioningSystem.IsSameHeading(trafficVehicles.GetForwardVector(otherIndex), trafficVehicles.GetForwardVector(myIndex));
                bool goingForward = vehiclePositioningSystem.IsGoingForward(trafficVehicles.GetVelocity(myIndex), trafficVehicles.GetHeading(myIndex));

#if UNITY_EDITOR
                collisionProcessText[myIndex] = "Collision:";
                collisionProcessText[myIndex] += "\nis in front of " + otherIndex + "? -> " + inFront;
                collisionProcessText[myIndex] += "\nhas same orientation(front trigger) with " + otherIndex + " " + sameOrientation;
                collisionProcessText[myIndex] += "\nhas same heading with(forward vector) " + otherIndex + " " + sameHeading;
                collisionProcessText[myIndex] += "\nis going forward " + goingForward;
#endif

                if (inFront == 2)
                {
                    //I am behind
                    if (goingForward == true)
                    {
                        //I am going forward
                        if (sameOrientation == true)
                        {
                            //the other vehicle is oriented forward
                            if (sameHeading == true)
                            {
                                //other vehicle is going forward
                                //-> I have hit him from behind so I should stop
                                NewDriveActionArrived(myIndex, SpecialDriveActionTypes.StopNow, true);
                            }
                            else
                            {
                                //if other vehicle is going in reverse i should also
                                NewDriveActionArrived(myIndex, SpecialDriveActionTypes.Reverse, true);
                            }
                        }
                        else
                        {
                            //I am on the wrong way -> reverse
                            NewDriveActionArrived(myIndex, SpecialDriveActionTypes.Reverse, true);
                        }
                    }
                    else
                    {
                        //I am going backwards so I should stop because I hit something
                        NewDriveActionArrived(myIndex, SpecialDriveActionTypes.StopNow, true);
                    }
                }
                else
                {
                    if (inFront == 1)
                    {
                        //I am in front
                        if (goingForward == true)
                        {
                            // I am going forward
                            if (sameOrientation == false)
                            {
                                // I am on the wrong way and I hit something -> reverse
                                NewDriveActionArrived(myIndex, SpecialDriveActionTypes.Reverse, true);
                            }
                        }
                        else
                        {
                            //I am going backwards and I hit something -> stop 
                            NewDriveActionArrived(myIndex, SpecialDriveActionTypes.StopNow, true);
                        }
                    }
                    else
                    {
                        //it is not clear who is in front
                        if (sameHeading)
                        {
                            //if we are going in the same direction I should stop
                            NewDriveActionArrived(myIndex, SpecialDriveActionTypes.StopNow, true);
                        }
                        else
                        {
                            //I am on the wrong way -> reverse
                            NewDriveActionArrived(myIndex, SpecialDriveActionTypes.Reverse, true);
                        }
                    }
                }
            }
            else
            {
                //remove stop action
                NewDriveActionArrived(myIndex, SpecialDriveActionTypes.StopNow, false);
            }
        }


        /// <summary>
        /// Deciding how important is the new action compared with the current action
        /// </summary>
        /// <param name="index"></param>
        /// <param name="newAction"></param>
        /// <param name="active">true -. add action to queue, false-> remove action</param>
        internal void NewDriveActionArrived(int index, SpecialDriveActionTypes newAction, bool active)
        {
            if (active == true)
            {
                //if the new action is not already in the list-> add it in the required position based on priority
                if (!driveActions[index].activeActions.Contains(newAction))
                {
                    bool added = false;
                    for (int i = 0; i < driveActions[index].activeActions.Count; i++)
                    {
                        if (driveActions[index].activeActions[i] < newAction)
                        {
                            driveActions[index].activeActions.Insert(i, newAction);
                            added = true;
                            break;
                        }
                    }
                    if (added == false)
                    {
                        driveActions[index].activeActions.Add(newAction);
                    }
                }
            }
            else
            {
                //car is out of trigger -> remove current action
                if (newAction == SpecialDriveActionTypes.Continue)
                {
                    // remove all active actions
                    driveActions[index].activeActions.RemoveAll(cond => movingActions.Contains(cond));
                }
                else
                {
                    //remove just current action
                    driveActions[index].activeActions.Remove(newAction);
                }
            }

            //remove duplicates -> this is a safety measure
            for (int i = 0; i < driveActions[index].activeActions.Count - 1; i++)
            {
                //moving actions
                if (movingActions.Contains(driveActions[index].activeActions[i]))
                {
                    for (int j = driveActions[index].activeActions.Count - 1; j >= i + 1; j--)
                    {
                        if (movingActions.Contains(driveActions[index].activeActions[j]))
                        {
                            driveActions[index].activeActions.RemoveAt(j);
                        }
                    }
                }

                //waypoint actions
                if (waypointActions.Contains(driveActions[index].activeActions[i]))
                {
                    for (int j = driveActions[index].activeActions.Count - 1; j >= i + 1; j--)
                    {
                        if (waypointActions.Contains(driveActions[index].activeActions[j]))
                        {
                            driveActions[index].activeActions.RemoveAt(j);
                        }
                    }
                }

                //collision Actions
                if (collisionActions.Contains(driveActions[index].activeActions[i]))
                {
                    for (int j = driveActions[index].activeActions.Count - 1; j >= i + 1; j--)
                    {
                        if (collisionActions.Contains(driveActions[index].activeActions[j]))
                        {
                            driveActions[index].activeActions.RemoveAt(j);
                        }
                    }
                }
            }

            ApplyAction(index);
        }


        /// <summary>
        /// Apply the first action from list
        /// </summary>
        /// <param name="index"></param>
        void ApplyAction(int index)
        {
#if DEBUG_TRAFFIC
            currentActiveAction[index] = SpecialDriveActionTypes.Forward;
            return;
#endif
            //if trigger is true, other vehicles needs to be alerted that the current action changed
            bool trigger = false;
            if (driveActions[index].activeActions.Count == 0)
            {
                //if list is empty, go forward by default 
                currentActiveAction[index] = SpecialDriveActionTypes.Forward;
                trigger = true;
            }
            else
            {
                if (currentActiveAction[index] != driveActions[index].activeActions[0])
                {
                    //if action is different from the current action and it is not follow or overtake, change action and alert other vehicles 
                    if (currentActiveAction[index] == SpecialDriveActionTypes.Follow || currentActiveAction[index] == SpecialDriveActionTypes.Overtake)
                    {
                        if (waypointActions.Contains(driveActions[index].activeActions[0]))
                        {
                            if (waypointManager.IsSameTarget(index, vehicleToFollow[index]))
                            {
                                return;
                            }
                        }
                    }
                    trigger = true;
                    currentActiveAction[index] = driveActions[index].activeActions[0];
                }
            }

            //if (currentActiveAction[index] != SpecialDriveActionTypes.Follow && currentActiveAction[index] != SpecialDriveActionTypes.Overtake)
            //{
            //    //reset follow speed if no longer follow a vehicle
            //    Debug.Log("RESET FOLLWO SPEED " + index);
            //    vehicleToFollow[index] = -1;
                AIEvents.TriggerChangeDestinationEvent(index);
            //}

            if (trigger)
            {
                //trigger corresponding events based on new action

                if (currentActiveAction[index] == SpecialDriveActionTypes.Reverse || currentActiveAction[index] == SpecialDriveActionTypes.AvoidReverse)
                {
                    AIEvents.TriggerVehicleChangedStateEvent(index, trafficVehicles.GetCollider(index), SpecialDriveActionTypes.Reverse);
                }

                if (currentActiveAction[index] == SpecialDriveActionTypes.Follow || currentActiveAction[index] == SpecialDriveActionTypes.Overtake)
                {
                    AIEvents.TriggetChangeDrivingStateEvent(index, currentActiveAction[index], GetStopDistance(index));
                    return;
                }

                if (currentActiveAction[index] == SpecialDriveActionTypes.StopInDistance || currentActiveAction[index] == SpecialDriveActionTypes.StopInPoint || currentActiveAction[index] == SpecialDriveActionTypes.GiveWay)
                {
                    AIEvents.TriggerVehicleChangedStateEvent(index, trafficVehicles.GetCollider(index), SpecialDriveActionTypes.StopInDistance);
                }
                AIEvents.TriggetChangeDrivingStateEvent(index, currentActiveAction[index], GetActionValue(currentActiveAction[index]));
            }
        }


        internal float GetStopDistance(int index)
        {
            return Vector3.Distance(vehiclePositioningSystem.GetPosition(index), vehiclePositioningSystem.GetPosition(vehicleToFollow[index])) - trafficVehicles.GetVehicleLength(index) - 1.5f;
        }

        /// <summary>
        /// Returns execution times for each action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        float GetActionValue(SpecialDriveActionTypes action)
        {
            switch (action)
            {
                case SpecialDriveActionTypes.Forward:
                    return 0;
                case SpecialDriveActionTypes.StopInDistance:
                    return 2.5f;
                case SpecialDriveActionTypes.AvoidReverse:
                case SpecialDriveActionTypes.Reverse:
                    return 2;
                case SpecialDriveActionTypes.TempStop:
                    return 5;
                case SpecialDriveActionTypes.StopNow:
                    return Random.Range(3, 5);
                case SpecialDriveActionTypes.Follow:
                    return 1;
                default:
                    return Mathf.Infinity;
            }
        }


        /// <summary>
        /// Called when a waypoint state changed to update the current vehicle actions
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stopState"></param>
        /// <param name="giveWayState"></param>
        private void StopStateChanged(int index, bool stopState)
        {
                NewDriveActionArrived(index, SpecialDriveActionTypes.StopInPoint, stopState);
        }

        private void GiveWayStateChanged(int index, bool giveWayState)
        {
                NewDriveActionArrived(index, SpecialDriveActionTypes.GiveWay, giveWayState);
        }


        /// <summary>
        /// Called when a vehicle needs a new waypoint
        /// </summary>
        /// <param name="index"></param>
        /// <param name="carType"></param>
        public void WaypointRequested(int index, VehicleTypes carType)
        {
            int freeWaypointIndex;
            for (int i = 0; i < driveActions[index].activeActions.Count; i++)
            {
                if (driveActions[index].activeActions[i] == SpecialDriveActionTypes.StopInPoint)
                {
                    //if current action is stop in point -> no new waypoint is needed
                    return;
                }

                if (driveActions[index].activeActions[i] == SpecialDriveActionTypes.Overtake)
                {
                    //ContinueStraight(index, carType);
                    //return;
                    //if the current vehicle can overtake
                    freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(index, (int)carType);
                    if (freeWaypointIndex == -1)
                    {
                        //if cannot change lane
                        ContinueStraight(index, carType);
                    }
                    else
                    {

                        if (waypointManager.AllPreviousWaypointsAreFree(index, waypointManager.GetNrOfWaypointsToCheck(index, trafficVehicles.GetOvertakeDistance(index)), freeWaypointIndex))
                        {
                            //if can change lane -> start blinking
                            SetNextWaypoint(index, freeWaypointIndex, BlinkReasons.Overtake);
                        }
                        else
                        {
                            ContinueStraight(index, carType);
                        }
                    }
                    return;
                }

                if (driveActions[index].activeActions[i] == SpecialDriveActionTypes.GiveWay)
                {
                    if (waypointManager.IsInIntersection(index))
                    {
                        if (waypointManager.CanEnterIntersection(index))
                        {
                            freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(index, (int)carType);
                            if (freeWaypointIndex != -1)
                            {
                                NewDriveActionArrived(index, SpecialDriveActionTypes.GiveWay, false);
                                SetNextWaypoint(index, freeWaypointIndex, BlinkReasons.ChangeLane);
                            }
                        }
                    }
                    else
                    {
                        freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(index, (int)carType);
                        if (freeWaypointIndex != -1)
                        {
                            CheckFreePath(index, freeWaypointIndex, BlinkReasons.ChangeLane);
                        }
                        else
                        {
                            freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(index, (int)carType);
                            if (freeWaypointIndex != -1)
                            {
                                CheckFreePath(index, freeWaypointIndex, BlinkReasons.ChangeLane);
                            }
                            else
                            {
                                NewDriveActionArrived(index, SpecialDriveActionTypes.NoWaypoint, true);
                            }
                        }
                    }
                    //If current vehicle has to give way -> wait until new waypoint is free
                    return;
                }
            }

            //if current vehicle is in no special state -> set next waypoint without any special requirements

            freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(index, (int)carType);
            if (freeWaypointIndex != -1)
            {
                SetNextWaypoint(index, freeWaypointIndex, BlinkReasons.Stop);
                //remove the no waypoint action if waypoints are found -> used for temporary disable waypoints
                if (driveActions[index].activeActions.Count > 0)
                {
                    if (driveActions[index].activeActions[0] == SpecialDriveActionTypes.NoWaypoint)
                    {
                        NewDriveActionArrived(index, SpecialDriveActionTypes.NoWaypoint, false);
                    }
                }
            }
            else
            {
                NoWaypointsAvailable(index);
            }
        }




        private void CheckFreePath(int index, int freeWaypointIndex, BlinkReasons blinkReason)
        {
            if (waypointManager.AllPreviousWaypointsAreFree(index, waypointManager.GetNrOfWaypointsToCheck(index, trafficVehicles.GetSafeDistance(index)), freeWaypointIndex))
            {
                NewDriveActionArrived(index, SpecialDriveActionTypes.GiveWay, false);
                SetNextWaypoint(index, freeWaypointIndex, blinkReason);
            }
        }

        void ContinueStraight(int index, VehicleTypes carType)
        {
            //get new waypoint on the same lane
            int freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(index, (int)carType);
            if (freeWaypointIndex != -1)
            {
                SetNextWaypoint(index, freeWaypointIndex, BlinkReasons.Stop);
            }
            else
            {
                NoWaypointsAvailable(index);
            }
        }

        void NoWaypointsAvailable(int index)
        {
            NewDriveActionArrived(index, SpecialDriveActionTypes.NoWaypoint, true);
        }

        void SetNextWaypoint(int index, int freeWaypointIndex, BlinkReasons blinkReason)
        {
            Blink(blinkReason, index, waypointManager.GetTargetWaypointIndex(index), freeWaypointIndex);
            waypointManager.SetNextWaypoint(index, freeWaypointIndex);
            waypointSpeed[index] = waypointManager.GetMaxSpeed(index);
            AIEvents.TriggerChangeDestinationEvent(index);
        }

        #region Blinking
        /// <summary>
        /// Determine if blink is required
        /// </summary>
        /// <param name="possibleLaneChange"></param>
        /// <param name="index"></param>
        /// <param name="oldWaypoint"></param>
        /// <param name="newWaypoint"></param>
        /// <param name="oldPoz"></param>
        /// <param name="forward"></param>
        public void Blink(BlinkReasons blinkReason, int index, int oldWaypointIndex, int newWaypointindex)
        {
            Vector3 forward = trafficVehicles.GetForwardVector(index);
            Waypoint oldWaypoint = waypointManager.GetWaypoint<Waypoint>(oldWaypointIndex);
            Waypoint newWaypoint = waypointManager.GetWaypoint<Waypoint>(newWaypointindex);
            Waypoint targetWaypoint = newWaypoint;
            if (blinkReason == BlinkReasons.Stop)
            {
                if (oldWaypoint.neighbors.Count > 1)
                {
                    blinkReason = BlinkReasons.ChangeLane;
                }
            }
            switch (blinkReason)
            {
                case BlinkReasons.Overtake:
                    float angle = Vector3.SignedAngle(forward, newWaypoint.position - oldWaypoint.position, Vector3.up);
                    SetBlinkType(angle, index);
                    break;

                case BlinkReasons.ChangeLane:
                    for (int i = 0; i < 5; i++)
                    {
                        if (targetWaypoint.neighbors.Count > 0)
                        {
                            targetWaypoint = waypointManager.GetWaypoint<Waypoint>(targetWaypoint.neighbors[0]);
                        }
                    }
                    angle = Vector3.SignedAngle(oldWaypoint.position - waypointManager.GetWaypoint<Waypoint>(oldWaypoint.prev[0]).position, targetWaypoint.position - oldWaypoint.position, Vector3.up);
                    SetBlinkType(angle, index);
                    break;

                case BlinkReasons.Stop:
                    if (newWaypoint.neighbors.Count > 0)
                    {
                        targetWaypoint = waypointManager.GetWaypoint<Waypoint>(targetWaypoint.neighbors[0]);
                        angle = Vector3.SignedAngle(oldWaypoint.position - newWaypoint.position, oldWaypoint.position - targetWaypoint.position, Vector3.up);
                        if (Mathf.Abs(angle) < 1)
                        {
                            blinkTypes[index] = BlinkType.Stop;
                        }
                    }
                    break;

                case BlinkReasons.NoWaypoint:
                    blinkTypes[index] = BlinkType.Stop;
                    break;
            }
        }


        /// <summary>
        /// Returns blink type
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BlinkType GetBlinkType(int index)
        {
            return blinkTypes[index];
        }


        /// <summary>
        /// Determine the blink direction
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="index"></param>
        private void SetBlinkType(float angle, int index)
        {
            if (Mathf.Abs(angle) < 1)
            {
                blinkTypes[index] = BlinkType.Stop;
            }
            else
            {
                if (angle > 5)
                {
                    blinkTypes[index] = BlinkType.BlinkRight;
                }
                else
                {
                    if (angle < -5)
                    {
                        blinkTypes[index] = BlinkType.BlinkLeft;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Changes current action to overtake
        /// </summary>
        /// <param name="index"></param>
        public void Overtake(int index)
        {
            NewDriveActionArrived(index, SpecialDriveActionTypes.Overtake, true);
        }


        /// <summary>
        /// Removed the reverse action
        /// </summary>
        /// <param name="index"></param>
        public void ReverseDone(int index)
        {
            NewDriveActionArrived(index, SpecialDriveActionTypes.Reverse, false);
        }


        /// <summary>
        /// Removes the temp stop action
        /// </summary>
        /// <param name="index"></param>
        public void TempStopDone(int index)
        {
            NewDriveActionArrived(index, SpecialDriveActionTypes.TempStop, false);
        }


        /// <summary>
        /// Compute current maximum available speed in m/s
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetMaxSpeedMS(int index)
        {
            return ComputeMaxPossibleSpeed(index) / 3.6f;
        }


        float ComputeMaxPossibleSpeed(int index)
        {
            float maxSpeed;

            if (currentActiveAction[index] == SpecialDriveActionTypes.Follow || currentActiveAction[index] == SpecialDriveActionTypes.Overtake)
            {
                //Debug.Log("ComputeMaxPossibleSpeed follow speed " + trafficVehicles.GetCurrentSpeed(vehicleToFollow[index]) + " max speed " + maxVehicleSpeed[index] + " waypoint speed " + waypointSpeed[index]);
                maxSpeed = Mathf.Min(trafficVehicles.GetCurrentSpeed(vehicleToFollow[index]), maxVehicleSpeed[index], waypointSpeed[index]);

            }
            else
            {
                maxSpeed = Mathf.Min(maxVehicleSpeed[index], waypointSpeed[index]);
            }

            return maxSpeed;
        }

        /// <summary>
        /// Events cleanup
        /// </summary>
        private void OnDestroy()
        {
            VehicleEvents.onNewObjectInTrigger -= NewObjectInTriggerHandler;
            VehicleEvents.onVehicleCrash -= VehicleCrashHandler;
            GleyUrbanAssets.WaypointEvents.onStopStateChanged -= StopStateChanged;
            GleyUrbanAssets.WaypointEvents.onGiveWayStateChanged -= GiveWayStateChanged;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (actionsDebug)
            {
                List<VehicleComponent> allVehicles = trafficVehicles.GetVehicleList();
                for (int i = 0; i < allVehicles.Count; i++)
                {
                    string text = allVehicles[i].GetIndex() + ". Action " + allVehicles[i].GetCurrentAction() + "\n";
                    if (speedDebug)
                    {
                        text += "Current Speed " + allVehicles[i].GetCurrentSpeed().ToString("N1") + "\n" +
                        "Follow Speed " + trafficVehicles.GetCurrentSpeed(vehicleToFollow[allVehicles[i].GetIndex()]).ToString("N1") + "\n" +
                        "Waypoint Speed " + waypointSpeed[allVehicles[i].GetIndex()].ToString("N1") + "\n" +
                        "Max Speed" + maxVehicleSpeed[allVehicles[i].GetIndex()].ToString("N1") + "\n" +
                        "Vehicle To Follow " + vehicleToFollow[allVehicles[i].GetIndex()].ToString("N1") + "\n";
                    }

                    if (aiDebug)
                    {
                        text += newObjDebugText[i] + "\n" +
                            triggerProcessText[i] + "\n" +
                            collisionProcessText[i];
                    }

                    Handles.Label(allVehicles[i].transform.position + new Vector3(1, 1, 1), text);
                }
            }
        }
#endif
    }
}


