using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class WaypointManagerBase : MonoBehaviour
    {
        protected bool debugWaypoints;

        private WaypointBase[] allWaypoints;
        private List<WaypointBase> disabledWaypoints;
        protected int[] target;//contains at index the waypoint index of the target waypoint of that agent. Agent at position 2 has the target waypoint index target[2]
        private bool debugDisabledWaypoints;


        internal virtual void Initialize(WaypointBase[] allWaypoints, int nrOfAgents, bool debugWaypoints, bool debugDisabledWaypoints)
        {
            this.allWaypoints = allWaypoints;
            this.debugWaypoints = debugWaypoints;
            this.debugDisabledWaypoints = debugDisabledWaypoints;

            target = new int[nrOfAgents];
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = -1;
            }
            disabledWaypoints = new List<WaypointBase>();
        }


        /// <summary>
        /// Set a waypoint as target
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="waypointIndex"></param>
        internal virtual void SetNextWaypoint(int agentIndex, int waypointIndex)
        {
            SetTargetWaypoint(agentIndex, waypointIndex);
        }


        /// <summary>
        /// Converts index to waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal T GetWaypoint<T>(int waypointIndex) where T : WaypointBase
        {
            if (waypointIndex == -1)
            {
                return null;
            }
            return (T)allWaypoints[waypointIndex];
        }


        /// <summary>
        /// Gets the target waypoint of the agent
        /// </summary>
        /// <typeparam name="T">type of waypoint</typeparam>
        /// <param name="agentIndex">agent index</param>
        /// <returns></returns>
        internal T GetTargetWaypointOfAgent<T>(int agentIndex) where T : WaypointBase
        {
            return GetWaypoint<T>(GetTargetWaypointIndex(agentIndex));
        }


        /// <summary>
        /// Checks if waypoint is temporary disabled
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal bool IsDisabled(int waypointIndex)
        {
            return GetWaypoint<WaypointBase>(waypointIndex).temporaryDisabled;
        }


        /// <summary>
        /// Directly set the target waypoint for the vehicle at index.
        /// Used to set first waypoint after vehicle initialization
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="waypointIndex"></param>
        internal void SetTargetWaypoint(int agentIndex, int waypointIndex)
        {
            MarkWaypointAsPassed(agentIndex);
            SetTargetWaypointIndex(agentIndex, waypointIndex);
        }


        /// <summary>
        /// Remove target waypoint for the agent at index
        /// </summary>
        /// <param name="agentIndex"></param>
        internal void RemoveTargetWaypoint(int agentIndex)
        {
            MarkWaypointAsPassed(agentIndex);
            SetTargetWaypointIndex(agentIndex, -1);
        }


        /// <summary>
        /// Get rotation of the target waypoint
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        internal Quaternion GetTargetWaypointRotation(int agentIndex)
        {
            if (GetTargetWaypointOfAgent<WaypointBase>(agentIndex).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint<WaypointBase>(GetTargetWaypointOfAgent<WaypointBase>(agentIndex).neighbors[0]).position - GetTargetWaypointOfAgent<WaypointBase>(agentIndex).position);
        }


        /// <summary>
        /// Get orientation of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal Quaternion GetOrientation(int waypointIndex)
        {
            if (GetWaypoint<WaypointBase>(waypointIndex).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint<WaypointBase>(GetWaypoint<WaypointBase>(waypointIndex).neighbors[0]).position - GetWaypoint<WaypointBase>(waypointIndex).position);
        }


        /// <summary>
        /// Get position of the target waypoint
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        internal Vector3 GetTargetWaypointPosition(int vehicleIndex)
        {
            return GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex).position;
        }


        /// <summary>
        /// Get position of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        internal Vector3 GetWaypointPosition(int waypointIndex)
        {
            return GetWaypoint<WaypointBase>(waypointIndex).position;
        }


        /// <summary>
        /// Get Target waypoint index of agent
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <returns></returns>
        internal int GetTargetWaypointIndex(int agentIndex)
        {
            return target[agentIndex];
        }


        /// <summary>
        /// Enables unavailable waypoints
        /// </summary>
        internal void EnableAllWaypoints()
        {
            for (int i = 0; i < disabledWaypoints.Count; i++)
            {
                disabledWaypoints[i].temporaryDisabled = false;
            }
            disabledWaypoints = new List<WaypointBase>();
        }


        /// <summary>
        /// Get a free waypoint connected to the current one
        /// </summary>
        /// <param name="agentIndex">agent that requested the waypoint</param>
        /// <param name="agentType">type of the agent that requested the waypoint</param>
        /// <returns></returns>
        internal int GetCurrentLaneWaypointIndex(int agentIndex, int agentType)
        {
            int waypointIndex = -1;
            WaypointBase oldWaypoint = GetTargetWaypointOfAgent<WaypointBase>(agentIndex);

            //check direct neighbors
            if (oldWaypoint.neighbors.Count > 0)
            {
                WaypointBase[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.allowedAgents.Contains(agentType) && cond.temporaryDisabled == false).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                }
            }

            //check other lanes
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    WaypointBase[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.allowedAgents.Contains(agentType) && cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check neighbors that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.neighbors.Count > 0)
                {
                    WaypointBase[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check other lanes that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    WaypointBase[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            return waypointIndex;
        }


        /// <summary>
        /// Check if a change of lane is possible
        /// Used to overtake and give way
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="agentType"></param>
        /// <returns></returns>
        internal int GetOtherLaneWaypointIndex(int agentIndex, int agentType)
        {
            WaypointBase[] possibleWaypoints = new WaypointBase[0];
            WaypointBase currentWaypoint = GetTargetWaypointOfAgent<WaypointBase>(agentIndex);

            if (currentWaypoint.otherLanes.Count > 0)
            {
                possibleWaypoints = GetAllWaypoints(currentWaypoint.otherLanes).Where(cond => cond.allowedAgents.Contains(agentType)).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    return possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                }
                return currentWaypoint.otherLanes[Random.Range(0, currentWaypoint.otherLanes.Count)];
            }

            return -1;
        }


        /// <summary>
        /// Mark a waypoint as disabled
        /// </summary>
        /// <param name="waypoint"></param>
        internal void AddDisabledWaypoints(WaypointBase waypoint)
        {
            disabledWaypoints.Add(waypoint);
            waypoint.temporaryDisabled = true;
        }


        /// <summary>
        /// Check if waypoint is a target for another agent
        /// </summary>
        /// <param name="waypointIndex"></param>
        /// <returns></returns>
        protected bool IsThisWaypointATarget(int waypointIndex)
        {
            return target.Contains(waypointIndex);
        }


        /// <summary>
        /// Set target waypoint for an agent
        /// </summary>
        /// <param name="agentIndex"></param>
        /// <param name="waypointIndex"></param>
        private void SetTargetWaypointIndex(int agentIndex, int waypointIndex)
        {
            target[agentIndex] = waypointIndex;
        }


        /// <summary>
        /// called when a waypoint was passed
        /// </summary>
        /// <param name="agentIndex"></param>
        private void MarkWaypointAsPassed(int agentIndex)
        {
            if (GetTargetWaypointIndex(agentIndex) != -1)
            {
                GetWaypoint<WaypointBase>(GetTargetWaypointIndex(agentIndex)).Passed(agentIndex);
            }
        }


        /// <summary>
        /// Switch the stop value of a waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        protected abstract void TrafficLightChanged(int waypointIndex, bool newValue);


        /// <summary>
        /// Convert list of indexes to list of waypoints
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        protected WaypointBase[] GetAllWaypoints(List<int> indexes)
        {
            WaypointBase[] result = new WaypointBase[indexes.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetWaypoint<WaypointBase>(indexes[i]);
            }
            return result;
        }


      

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugWaypoints)
            {
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] != -1)
                    {
                        Gizmos.color = Color.green;
                        Vector3 position = GetWaypoint<WaypointBase>(target[i]).position;
                        Gizmos.DrawSphere(position, 1);
                        position.y += 1.5f;
                        UnityEditor.Handles.Label(position, i.ToString());
                    }
                }
            }
            if (debugDisabledWaypoints)
            {
                for (int i = 0; i < disabledWaypoints.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(disabledWaypoints[i].position, 1);
                }
            }
        }
#endif
    }
}
