using GleyUrbanAssets;
using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    [System.Serializable]
    public partial class TrafficLightsIntersection : GenericIntersection
    {
        public List<IntersectionStopWaypointsIndex> stopWaypoints;
        public float greenLightTime;
        public float yellowLightTime;

        //for every road color is set here
        TrafficLightsColor[] intersectionState;
        private float currentTime;
        private int nrOfRoads;
        private int currentRoad;
        private bool yellowLight;
        private bool stopUpdate;
        public bool hasPedestrians;
        private TrafficLightsBehaviour trafficLightsBehaviour;
        private TrafficLightsBehaviour TrafficLightsBehaviour
        {
            get
            {
                if (trafficLightsBehaviour == null)
                {
                    trafficLightsBehaviour = TrafficLightsBehaviours.DefaultBehaviour;
                }
                return trafficLightsBehaviour;
            }
        }


        /// <summary>
        /// Constructor used for conversion from editor intersection type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stopWaypoints"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        public TrafficLightsIntersection(string name, List<IntersectionStopWaypointsIndex> stopWaypoints, float greenLightTime, float yellowLightTime)
        {
            this.name = name;
            this.stopWaypoints = stopWaypoints;
            this.greenLightTime = greenLightTime;
            this.yellowLightTime = yellowLightTime;
        }


        /// <summary>
        /// Assigns corresponding waypoints to work with this intersection and setup traffic lights
        /// </summary>
        /// <param name="waypointManager"></param>
        /// <param name="greenLightTime"></param>
        /// <param name="yellowLightTime"></param>
        public override void Initialize(WaypointManagerBase waypointManager, float greenLightTime, float yellowLightTime)
        {
            nrOfRoads = stopWaypoints.Count;
            GetPedestrianRoads();
            if (nrOfRoads == 0)
            {
                Debug.LogWarning("Intersection " + name + " has some unassigned references");
                return;
            }

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
                {
                    waypointManager.GetWaypoint<Waypoint>(stopWaypoints[i].roadWaypoints[j]).SetIntersection(this, false, true, false, false);
                }
            }

            intersectionState = new TrafficLightsColor[nrOfRoads];

            currentRoad = Random.Range(0, nrOfRoads);
            ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Green);
            ChangeAllRoadsExceptSelectd(currentRoad, TrafficLightsColor.Red);
            ApplyColorChanges();

            currentTime = 0;
            if (greenLightTime >= 0)
            {
                this.greenLightTime = greenLightTime;
            }
            if (yellowLightTime >= 0)
            {
                this.yellowLightTime = yellowLightTime;
            }
        }

        partial void GetPedestrianRoads();


        /// <summary>
        /// Change traffic lights color
        /// </summary>
        public override void UpdateIntersection(float realtimeSinceStartup)
        {
            if (stopUpdate)
                return;

            if (yellowLight == false)
            {
                if (realtimeSinceStartup - currentTime > greenLightTime)
                {
                    ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Yellow);
                    ApplyColorChanges();
                    yellowLight = true;
                    currentTime = realtimeSinceStartup;
                }
            }
            else
            {
                if (realtimeSinceStartup - currentTime > yellowLightTime)
                {
                    ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Red);
                    currentRoad++;
                    currentRoad = GetValidValue(currentRoad);
                    ChangeCurrentRoadColors(currentRoad, TrafficLightsColor.Green);
                    yellowLight = false;
                    currentTime = realtimeSinceStartup;
                    ApplyColorChanges();
                }
            }
        }


        /// <summary>
        /// Used for editor applications
        /// </summary>
        /// <returns></returns>
        public override List<IntersectionStopWaypointsIndex> GetWaypoints()
        {
            return stopWaypoints;
        }


        /// <summary>
        /// Used to set up custom behaviour for traffic lights
        /// </summary>
        /// <param name="trafficLightsBehaviour"></param>
        internal override void SetTrafficLightsBehaviour(TrafficLightsBehaviour trafficLightsBehaviour)
        {
            base.SetTrafficLightsBehaviour(trafficLightsBehaviour);
            this.trafficLightsBehaviour = trafficLightsBehaviour;
        }


        internal override void SetGreenRoad(int roadIndex, bool doNotChangeAgain)
        {
            base.SetGreenRoad(roadIndex, doNotChangeAgain);
            stopUpdate = doNotChangeAgain;
            ChangeCurrentRoadColors(roadIndex, TrafficLightsColor.Green);
            ChangeAllRoadsExceptSelectd(roadIndex, TrafficLightsColor.Red);
            ApplyColorChanges();
        }

        /// <summary>
        /// After all intersection changes have been made this method apply them to the waypoint system and traffic lights 
        /// </summary>
        private void ApplyColorChanges()
        {
            for (int i = 0; i < intersectionState.Length; i++)
            {
                //change waypoint color
                UpdateCurrentIntersectionWaypoints(i, intersectionState[i] != TrafficLightsColor.Green);
                
                if (i < stopWaypoints.Count)
                {
                    //change traffic lights color
                    TrafficLightsBehaviour(intersectionState[i], stopWaypoints[i].redLightObjects, stopWaypoints[i].yellowLightObjects, stopWaypoints[i].greenLightObjects, name);
                }
                else
                {
                    Debug.Log("Update pedestrian waypoints");
                }
            }
        }


        /// <summary>
        /// Trigger state changes for specified waypoints
        /// </summary>
        /// <param name="road"></param>
        /// <param name="stop"></param>
        private void UpdateCurrentIntersectionWaypoints(int road, bool stop)
        {
            if (hasPedestrians && road >= stopWaypoints.Count)
            {
                TriggerPedestrianWaypointsUpdate(stop);
                return;
            }
            for (int j = 0; j < stopWaypoints[road].roadWaypoints.Count; j++)
            {
                WaypointEvents.TriggerTrafficLightChangedEvent(stopWaypoints[road].roadWaypoints[j], stop);
            }
        }

        partial void TriggerPedestrianWaypointsUpdate(bool stop);


        /// <summary>
        /// Change color for specified road
        /// </summary>
        /// <param name="currentRoad"></param>
        /// <param name="newColor"></param>
        private void ChangeCurrentRoadColors(int currentRoad, TrafficLightsColor newColor)
        {
            if (currentRoad < intersectionState.Length)
            {
                intersectionState[currentRoad] = newColor;
            }
            else
            {
                Debug.LogError(currentRoad + "is grated than the max number of roads for intersection " + name);
            }
        }


        /// <summary>
        /// Change color for all roads except the specified one
        /// </summary>
        /// <param name="currentRoad"></param>
        /// <param name="newColor"></param>
        private void ChangeAllRoadsExceptSelectd(int currentRoad, TrafficLightsColor newColor)
        {
            for (int i = 0; i < intersectionState.Length; i++)
            {
                if (i != currentRoad)
                {
                    intersectionState[i] = newColor;
                }
            }
        }


        /// <summary>
        /// Correctly increment the road number
        /// </summary>
        /// <param name="roadNumber"></param>
        /// <returns></returns>
        private int GetValidValue(int roadNumber)
        {
            if (roadNumber >= nrOfRoads)
            {
                roadNumber = roadNumber % nrOfRoads;
            }
            if (roadNumber < 0)
            {
                roadNumber = nrOfRoads + roadNumber;
            }
            return roadNumber;
        }
    }
}
