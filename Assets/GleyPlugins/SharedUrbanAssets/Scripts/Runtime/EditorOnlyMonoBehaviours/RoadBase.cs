using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class RoadBase : MonoBehaviour
    {
        public List<Lane> lanes;
        public Path path;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public float laneWidth = 4;
        public float waypointDistance = 4;
        public int nrOfLanes = 2;
        public int selectedSegmentIndex = -1;
        public bool draw;
        public bool isInsidePrefab;

        public void SetRoadProperties(int globalMaxSpeed, int nrOfCars)
        {
            draw = true;
            lanes = new List<Lane>();
            for (int i = 0; i < nrOfLanes; i++)
            {
                lanes.Add(new Lane(nrOfCars, i % 2 == 0, globalMaxSpeed));
            }
        }

        public RoadBase SetDefaults(int nrOfLanes, float laneWidth, float waypointDistance)
        {
            this.nrOfLanes = nrOfLanes;
            this.laneWidth = laneWidth;
            this.waypointDistance = waypointDistance;
            return this;
        }

        public void UpdateLaneNumber(int maxSpeed, int nrOfCars)
        {
            if (lanes.Count > nrOfLanes)
            {
                lanes.RemoveRange(nrOfLanes, lanes.Count - nrOfLanes);
            }
            if (lanes.Count < nrOfLanes)
            {
                for (int i = lanes.Count; i < nrOfLanes; i++)
                {
                    lanes.Add(new Lane(nrOfCars, i % 2 == 0, maxSpeed));
                }
            }
        }

        public Path CreatePath(Vector3 startPosition, Vector3 endPosition)
        {
            path = new Path(startPosition, endPosition);
            return path;
        }

        public void AddLaneConnector(WaypointSettingsBase inConnector, WaypointSettingsBase outConnector, int index)
        {
            if (inConnector != null)
            {
                inConnector.name = inConnector.transform.parent.parent.parent.name + "-" + inConnector.transform.parent.name + Constants.inWaypointEnding;
            }
            if (outConnector != null)
            {
                outConnector.name = outConnector.transform.parent.parent.parent.name + "-" + outConnector.transform.parent.name + Constants.outWaypointEnding;
            }
            lanes[index].laneEdges = new LaneConnectors(inConnector, outConnector);
        }

        public void SwitchDirection(int laneNumber)
        {
            AddLaneConnector(lanes[laneNumber].laneEdges.outConnector, lanes[laneNumber].laneEdges.inConnector, laneNumber);
        }

        public int GetNrOfLanes()
        {
            return transform.Find(Constants.lanesHolderName).childCount;
        }

        public List<int> GetAllowedCars(int laneNumber)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < lanes[laneNumber].allowedCars.Length; i++)
            {
                if (lanes[laneNumber].allowedCars[i] == true)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}