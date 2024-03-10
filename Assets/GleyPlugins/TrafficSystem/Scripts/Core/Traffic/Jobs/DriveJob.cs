#if USE_GLEY_TRAFFIC
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Handles driving part of the vehicle
    /// </summary>
    [BurstCompile]
    public struct DriveJob : IJobParallelFor
    {
        public NativeArray<float3> bodyForce;
        public NativeArray<float> actionValue;
        public NativeArray<float> wheelRotation;
        public NativeArray<float> turnAngle;
        public NativeArray<float> vehicleRotationAngle;
        public NativeArray<int> gear;
        public NativeArray<bool> readyToRemove;
        public NativeArray<bool> needsWaypoint;
        public NativeArray<bool> isBraking;

        [ReadOnly] public NativeArray<SpecialDriveActionTypes> specialDriveAction;
        [ReadOnly] public NativeArray<float3> targetWaypointPosition;
        [ReadOnly] public NativeArray<float3> allBotsPosition;
        [ReadOnly] public NativeArray<float3> groundDirection;
        [ReadOnly] public NativeArray<float3> forwardDirection;
        [ReadOnly] public NativeArray<float3> rightDirection;
        [ReadOnly] public NativeArray<float3> downDirection;
        [ReadOnly] public NativeArray<float3> carVelocity;
        [ReadOnly] public NativeArray<float3> cameraPositions;
        [ReadOnly] public NativeArray<float> wheelCircumferences;
        [ReadOnly] public NativeArray<float> maxSteer;
        [ReadOnly] public NativeArray<float> powerStep;
        [ReadOnly] public NativeArray<float> brakeStep;
        [ReadOnly] public NativeArray<float> drag;
        [ReadOnly] public NativeArray<float> maxSpeed;
        [ReadOnly] public NativeArray<float> wheelDistance;
        [ReadOnly] public NativeArray<float> steeringStep;
        [ReadOnly] public NativeArray<float> distanceBetweenVehicles;
        [ReadOnly] public NativeArray<int> wheelSign;
        [ReadOnly] public float3 worldUp;
        [ReadOnly] public float distanceToRemove;
        [ReadOnly] public float fixedDeltaTime;

        const float distanceToCheckWaypoint = 10;

        private float3 waypointDirection;
        private float minWaypointDistance;
        private float targetSpeed; //required car speed in next frame
        private float currentSpeed; //speed in current frame
        private float dotProduct;
        private float waypointDistance;
        private float angle;
        private float maxSpeedForRoadType;
        private float nextFrameSpeed;
        private bool avoidBackward;
        private bool avoidForward;


        public void Execute(int index)
        {
            //reset variables
            avoidForward = false;
            avoidBackward = false;
            isBraking[index] = false;

            //compute current frame values
            targetSpeed = currentSpeed = math.length(carVelocity[index]);
            waypointDirection = targetWaypointPosition[index] - allBotsPosition[index];
            dotProduct = math.dot(waypointDirection, forwardDirection[index]);// used to check if vehicle passed the current waypoint
            waypointDirection.y = 0;
            waypointDistance = math.distance(targetWaypointPosition[index], allBotsPosition[index]);

            //change the distance to change waypoints based on vehicle speed
            //at 50 kmh -> min distance =1.5
            //at 100 kmh -> min distance =2.5
            //kmh to ms => 50/3.6 = 13.88
            if (currentSpeed < 13.88f)
            {
                minWaypointDistance = 1.5f;
            }
            else
            {
                minWaypointDistance = 1.5f + (currentSpeed * 3.6f - 50) / 50;
            }
            //brake if vehicle goes faster than max speed
            maxSpeedForRoadType = SetMaxSpeed(index, turnAngle[index], maxSteer[index], maxSpeed[index]);

            //compute acceleration based on the current vehicle actions
            ComputeForces(index);

            //compute target speed for next frame
            nextFrameSpeed = GetSpeed(index, targetSpeed, maxSpeedForRoadType, gear[index]);

            if (index == 1)
            {
                //Debug.Log("currentSpeed " + currentSpeed * 3.6f + " targetSpeed " + targetSpeed * 3.6f + " maxSpeedForRoadType " + maxSpeedForRoadType * 3.6f + " " + maxSpeed[index] * 3.6f);
            }
            //compute forces required for the target speed to be achieved
            bodyForce[index] = ComputeBodyForce(index, nextFrameSpeed);

            //check if a new waypoint is required
            ChangeWaypoint(index);

            //compute the wheel turn amount
            ComputeWheelRotationAngle(index);

            //compute steering angle
            ComputeSteerAngle(index, maxSteer[index], nextFrameSpeed);


            //check if vehicle is far enough for the player and it can be removed
            RemoveVehicle(index);
#if DEBUG_TRAFFIC
            turnAngle[index] = 0;
            vehicleRotationAngle[index] = 0;
#endif
        }


        /// <summary>
        /// Brake if the car goes faster than the speed limit
        /// </summary>
        /// <param name="index">current vehicle index</param>
        /// <param name="turnAngle">current steering angle</param>
        /// <param name="maxSteer">max steering angle</param>
        private float SetMaxSpeed(int index, float turnAngle, float maxSteer, float maxSpeed)
        {
            //brake if the car is steering
            if (maxSpeed / targetSpeed < 2)
            {
                if (turnAngle > 3)
                {
                    if (specialDriveAction[index] != SpecialDriveActionTypes.Follow && specialDriveAction[index] != SpecialDriveActionTypes.Overtake)
                    {
                        ApplyBrakes(index, 1 + 2 * turnAngle / maxSteer);
                    }
                }
            }

            //brake if the vehicle runs faster than the max allowed speed
            if (targetSpeed > maxSpeed)
            {
                if (targetSpeed - maxSpeed > 1)
                {
                    //turn on braking lights
                    isBraking[index] = true;
                }

                if (specialDriveAction[index] == SpecialDriveActionTypes.Follow || specialDriveAction[index] == SpecialDriveActionTypes.Overtake)
                {
                    ApplyFollowBrakes(index, maxSpeed);
                }
                else
                {
                    ApplyBrakes(index, 1);
                }

                return targetSpeed;
            }
            return maxSpeed;
        }


        /// <summary>
        /// Slows the car down to match the front car speed
        /// </summary>
        /// <param name="index"></param>
        /// <param name="followSpeed"></param>
        private void ApplyFollowBrakes(int index, float followSpeed)
        {
            //compute per frame velocity
            float velocityPerFrame = (targetSpeed - followSpeed) * fixedDeltaTime;

            if (actionValue[index] == math.INFINITY)
            {
                ApplyBrakes(index, 1.1f);
                return;
            }
            //check number of frames required to slow down
            //the vehicle needs to achieve the from vehicle in the action value time(currently 1s)
            int nrOfFrames = (int)(actionValue[index] / velocityPerFrame);
            if (nrOfFrames == 0)
            {
                ApplyBrakes(index, 1);
                return;
            }



            //number of frames required to brake
            int brakeNrOfFrames = (int)((targetSpeed - followSpeed) / brakeStep[index]);

            if (nrOfFrames < 0)
            {
                ApplyBrakes(index, brakeNrOfFrames + 1);
                return;
            }

            //calculate the required brake power 
            if (brakeNrOfFrames >= nrOfFrames)
            {
                ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
            }
            else
            {
                ApplyBrakes(index, 1);
            }
        }


        /// <summary>
        /// Compute acceleration value based on the current vehicle`s driving actions
        /// </summary>
        /// <param name="index">index of the current vehicle </param>
        private void ComputeForces(int index)
        {
            if (actionValue[index] > 0)
            {
                switch (specialDriveAction[index])
                {
                    case SpecialDriveActionTypes.Reverse:
                        Reverse(index);
                        actionValue[index] -= fixedDeltaTime;
                        break;
                    case SpecialDriveActionTypes.AvoidReverse:
                        AvoidReverse(index);
                        break;
                    case SpecialDriveActionTypes.StopNow:
                    case SpecialDriveActionTypes.NoWaypoint:
                        actionValue[index] -= fixedDeltaTime;
                        StopNow(index);
                        isBraking[index] = true;
                        break;
                    case SpecialDriveActionTypes.StopInDistance:
                    case SpecialDriveActionTypes.TempStop:
                        StopInDistance(index);
                        break;
                    case SpecialDriveActionTypes.AvoidForward:
                        AvoidForward(index);
                        break;
                    case SpecialDriveActionTypes.StopInPoint:
                    case SpecialDriveActionTypes.GiveWay:
                        StopInPoint(index);
                        break;
                    case SpecialDriveActionTypes.Follow:
                        actionValue[index] -= fixedDeltaTime;
                        ApplyAcceleration(index);
                        break;
                    case SpecialDriveActionTypes.Overtake:
                        ApplyAcceleration(index);
                        break;
                }
            }
            else
            {
                switch (specialDriveAction[index])
                {
                    case SpecialDriveActionTypes.StopNow:
                    case SpecialDriveActionTypes.StopInDistance:
                    case SpecialDriveActionTypes.NoWaypoint:
                        StopNow(index);
                        break;

                    default:
                        ApplyAcceleration(index);
                        break;
                }
            }
        }


        /// <summary>
        /// Compute required linear speed for next frame
        /// </summary>
        /// <param name="index">current vehicle index</param>
        /// <param name="targetSpeed">speed to reach in the next frame</param>
        /// <param name="maxSpeed">max possible speed</param>
        /// <param name="gear">movement direction</param>
        /// <returns>speed for next frame</returns>
        float GetSpeed(int index, float targetSpeed, float maxSpeed, int gear)
        {
            if (gear == -1)
            {
                if (targetSpeed < -maxSpeed / 5)
                {
                    targetSpeed = -maxSpeed / 5;
                }
            }
            else
            {
                if (targetSpeed > maxSpeed)
                {
                    targetSpeed = maxSpeed;
                }
            }
            return targetSpeed + GetDrag(index, targetSpeed);
        }


        /// <summary>
        /// Compute the next frame force to be applied to RigidBody
        /// </summary>
        /// <param name="index">current vehicle index</param>
        /// <param name="targetVelocity">target linear speed</param>
        /// <returns></returns>
        private float3 ComputeBodyForce(int index, float targetVelocity)
        {
            return -carVelocity[index] + targetVelocity * groundDirection[index] * gear[index];
        }


        /// <summary>
        /// Check if new waypoint is required
        /// </summary>
        /// <param name="index">current vehicle index</param>
        void ChangeWaypoint(int index)
        {
            if (waypointDistance < minWaypointDistance || (dotProduct < 0 && waypointDistance < minWaypointDistance * 5))
            {
                needsWaypoint[index] = true;
            }
        }


        /// <summary>
        /// Compute the wheel turn amount
        /// </summary>
        /// <param name="index">current vehicle index</param>
        void ComputeWheelRotationAngle(int index)
        {
            wheelRotation[index] += (360 * (currentSpeed / wheelCircumferences[index]) * fixedDeltaTime) * gear[index];
        }


        /// <summary>
        /// Compute the required steering angle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="maxSteer"></param>
        /// <param name="targetVelocity"></param>
        void ComputeSteerAngle(int index, float maxSteer, float targetVelocity)
        {
            float currentTurnAngle = turnAngle[index];
            float currentStep = steeringStep[index];

            //determine the target angle
            if (avoidBackward)
            {
                angle = wheelSign[index] * -maxSteer;
            }
            else
            {
                if (avoidForward)
                {
                    angle = maxSteer;
                }
                else
                {
                    angle = SignedAngle(forwardDirection[index], waypointDirection, worldUp);
                }
            }

            if (!avoidBackward && !avoidForward)
            {
                //if car is stationary, do not turn the wheels
                if (currentSpeed < 1)
                {
                    angle = 0;
                }

                //check if the car can turn at current speed         
                float framesToReach = waypointDistance / (targetVelocity * fixedDeltaTime);
                //if it is close to the waypoint turn at normal speed 
                if (framesToReach > 5)
                {
                    float framesToRotate;
                    float difference;
                    if (angle > currentTurnAngle)
                    {
                        difference = angle - currentTurnAngle;
                    }
                    else
                    {
                        difference = currentTurnAngle - angle;
                    }
                    framesToRotate = difference / currentStep;

                    //used to straight the wheels after a curve
                    //if target and current angle have opposite signs
                    if (math.sign(currentTurnAngle) != math.sign(angle) && math.abs(currentTurnAngle - angle) > 5)
                    {
                        currentStep *= framesToRotate / 5;
                    }
                    else
                    {
                        //increase the speed turn amount to be able to corner
                        if (framesToRotate > framesToReach + 5)
                        {
                            currentStep *= framesToRotate / framesToReach;
                        }
                    }
                }
            }

            //apply turning speed
            if (angle - currentTurnAngle < -currentStep)
            {
                currentTurnAngle -= currentStep;
            }
            else
            {
                if (angle - currentTurnAngle > currentStep)
                {
                    currentTurnAngle += currentStep;
                }
                else
                {
                    currentTurnAngle = angle;
                }
            }

            //clamp the value
            if (currentTurnAngle > maxSteer)
            {
                currentTurnAngle = maxSteer;
            }

            if (currentTurnAngle < -maxSteer)
            {
                currentTurnAngle = -maxSteer;
            }

            //compute the body turn angle based on wheel turn amount
            float turnRadius = wheelDistance[index] / math.tan(math.radians(currentTurnAngle));
            vehicleRotationAngle[index] = (180 * targetVelocity * fixedDeltaTime) / (math.PI * turnRadius) * gear[index];

            turnAngle[index] = currentTurnAngle/**gear[index]*/;
        }


        /// <summary>
        /// Checks if the vehicle can be removed from scene
        /// </summary>
        /// <param name="index">the list index of the vehicle</param>
        void RemoveVehicle(int index)
        {
            bool remove = true;
            for (int i = 0; i < cameraPositions.Length; i++)
            {
                if (math.distancesq(allBotsPosition[index], cameraPositions[i]) < distanceToRemove)
                {
                    remove = false;
                    break;
                }
            }
            readyToRemove[index] = remove;
        }


        /// <summary>
        /// Determine if a car has can change the heading direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsInGear(int index)
        {
            switch (specialDriveAction[index])
            {
                case SpecialDriveActionTypes.Reverse:
                case SpecialDriveActionTypes.AvoidReverse:
                    if (gear[index] != -1)
                    {
                        if (targetSpeed == 0)
                        {
                            gear[index] = -1;
                        }
                        else
                        {
                            ApplyBrakes(index, 1);
                            return false;
                        }
                    }
                    break;

                default:
                    if (gear[index] != 1)
                    {
                        if (targetSpeed == 0)
                        {
                            gear[index] = 1;
                        }
                        else
                        {
                            ApplyBrakes(index, 1);
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }


        /// <summary>
        /// Go backwards in opposite direction
        /// </summary>
        /// <param name="index"></param>
        private void AvoidReverse(int index)
        {
            if (actionValue[index] != math.INFINITY)
            {
                StopInDistance(index);
                if (targetSpeed == 0)
                {
                    actionValue[index] = math.INFINITY;
                }
            }
            else
            {
                AvoidBackward();
                Reverse(index);
            }
        }


        /// <summary>
        /// Go backwards
        /// </summary>
        /// <param name="index"></param>
        void Reverse(int index)
        {
            ApplyAcceleration(index);
        }


        /// <summary>
        /// Opposite direction is required in reverse
        /// </summary>
        void AvoidBackward()
        {
            avoidBackward = true;
        }


        /// <summary>
        /// Opposite direction is required forward 
        /// </summary>
        /// <param name="index"></param>
        void AvoidForward(int index)
        {
            avoidForward = true;
            ApplyAcceleration(index);
        }


        /// <summary>
        /// Compute sign angle between 2 directions
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        float SignedAngle(float3 dir1, float3 dir2, float3 normal)
        {
            if (dir1.Equals(float3.zero))
            {
                return 0;
            }
            dir1 = math.normalize(dir1);
            return math.degrees(math.atan2(math.dot(math.cross(dir1, dir2), normal), math.dot(dir1, dir2)));
        }


        /// <summary>
        /// Stop the car in a given distance
        /// </summary>
        /// <param name="index"></param>
        private void StopInDistance(int index)
        {
            isBraking[index] = true;
            if (currentSpeed <= brakeStep[index])
            {
                StopNow(index);
                return;
            }

            float velocityPerFrame = currentSpeed * fixedDeltaTime;
            actionValue[index] -= velocityPerFrame;
            if (actionValue[index] <= 0)
            {
                StopNow(index);
                return;
            }

            int nrOfFrames = (int)((actionValue[index]) / velocityPerFrame) + 1;
            int brakeNrOfFrames = (int)(currentSpeed / brakeStep[index]);
            if (brakeNrOfFrames >= nrOfFrames)
            {
                ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
            }
        }


        /// <summary>
        /// Stop the vehicle precisely on a waypoint
        /// </summary>
        /// <param name="index"></param>
        void StopInPoint(int index)
        {
            isBraking[index] = true;
            //stop if the waypoint is behind the vehicle
            if (dotProduct < 0)
            {
                StopNow(index);
                return;
            }

            //the vehicle has very small speed
            if (currentSpeed < brakeStep[index])
            {
                //if the next waypoint is far -> accelerate
                if (waypointDistance > distanceToCheckWaypoint)
                {
                    ApplyAcceleration(index);
                }
                else
                {
                    if (specialDriveAction[index] == SpecialDriveActionTypes.GiveWay)
                    {
                        ApplyAcceleration(index);
                        return;
                    }

                    //wait for the waypoint state to change
                    StopNow(index);
                    if (specialDriveAction[index] == SpecialDriveActionTypes.TempStop)
                    {
                        actionValue[index] -= fixedDeltaTime;
                    }
                    return;
                }
            }

            //compute per frame velocity
            float velocityPerFrame = currentSpeed * fixedDeltaTime;

            //check number of frames required to reach next waypoint
            int nrOfFrames = (int)(waypointDistance / velocityPerFrame);

            //if vehicle is in target -> stop
            if (nrOfFrames == 0)
            {
                StopNow(index);
                return;
            }

            //number of frames required to brake
            int brakeNrOfFrames = (int)(currentSpeed / brakeStep[index]);

            //calculate the required brake power 
            if (brakeNrOfFrames > nrOfFrames)
            {
                ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
                isBraking[index] = true;
            }

            //if target waypoint is far -> accelerate
            if (nrOfFrames - brakeNrOfFrames > 60)
            {
                ApplyAcceleration(index);
            }
        }


        /// <summary>
        /// Brake the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="power"></param>
        void ApplyBrakes(int index, float power)
        {
            targetSpeed -= brakeStep[index] * power;
            if (targetSpeed < 0)
            {
                StopNow(index);
            }
        }


        /// <summary>
        /// Stop vehicle immediately
        /// </summary>
        /// <param name="index"></param>
        void StopNow(int index)
        {
            targetSpeed = 0;
            IsInGear(index);
        }


        /// <summary>
        /// Accelerate current vehicle
        /// </summary>
        /// <param name="index"></param>
        void ApplyAcceleration(int index)
        {
            if (IsInGear(index))
            {
                float additionalForce;
                angle = SignedAngle(worldUp, groundDirection[index], rightDirection[index]);
                if (angle >= 90)
                {
                    additionalForce = 0;
                }
                else
                {
                    additionalForce = (90 / angle - 1);
                    //add additional gravity force to go uphill
                    //remove some power to accelerate slower on ramp
                    additionalForce = additionalForce * 10 * fixedDeltaTime /*- powerStep[index] * additionalForce*/;
                }
                targetSpeed += powerStep[index] + additionalForce;
            }
        }

        /// <summary>
        /// Compensate the drag from the physics engine
        /// </summary>
        /// <param name="index"></param>
        /// <param name="targetSpeed"></param>
        /// <returns></returns>
        float GetDrag(int index, float targetSpeed)
        {
            float result = targetSpeed / (1 - drag[index] * fixedDeltaTime) - targetSpeed;
            return result;
        }
    }
}
#endif