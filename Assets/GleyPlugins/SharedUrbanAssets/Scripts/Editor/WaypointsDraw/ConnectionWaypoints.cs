using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public abstract class ConnectionWaypoints : Editor
    {
        protected abstract void AddLaneConnectionWaypoints(ConnectionPool connections, int index, float waypointDistance);

        internal void GenerateConnectorWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            RemoveConnectionWaipoints(connections.GetHolder(index));
            AddLaneConnectionWaypoints(connections, index, waypointDistance);
            EditorUtility.SetDirty(connections);
            AssetDatabase.SaveAssets();
        }


        internal void RemoveConnectionHolder(Transform holder)
        {
            RemoveConnectionWaipoints(holder);
            GleyPrefabUtilities.DestroyTransform(holder);
        }


        private void RemoveConnectionWaipoints(Transform holder)
        {
            if (holder)
            {
                for (int i = holder.childCount - 1; i >= 0; i--)
                {
                    WaypointSettingsBase waypoint = holder.GetChild(i).GetComponent<WaypointSettingsBase>();
                    for (int j = 0; j < waypoint.neighbors.Count; j++)
                    {
                        if (waypoint.neighbors[j] != null)
                        {
                            waypoint.neighbors[j].prev.Remove(waypoint);
                        }
                        else
                        {
                            Debug.LogError(waypoint.name + " has null neighbors", waypoint);
                        }
                    }
                    for (int j = 0; j < waypoint.prev.Count; j++)
                    {
                        if (waypoint.prev[j] != null)
                        {
                            waypoint.prev[j].neighbors.Remove(waypoint);
                        }
                        else
                        {
                            Debug.LogError(waypoint.name + " has null prevs", waypoint);
                        }
                    }
                    DestroyImmediate(waypoint.gameObject);
                }
            }
        }
    }
}