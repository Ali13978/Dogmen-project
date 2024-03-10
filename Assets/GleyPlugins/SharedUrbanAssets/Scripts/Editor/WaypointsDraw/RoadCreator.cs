using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class RoadCreator
    {
        
        static Transform roadWaypointsHolder;


        internal T Create<T>(Vector3 startPosition, string trafficWaypointsHolderName, RoadDefaults roadDefaults) where T : RoadBase
        {
            int roadNumber = GetFreeRoadNumber(trafficWaypointsHolderName);
            GameObject roadHolder = new GameObject("Road_" + roadNumber);
            roadHolder.tag = Constants.editorTag;
            roadHolder.transform.SetParent(GetRoadWaypointsHolder(trafficWaypointsHolderName));
            roadHolder.transform.SetSiblingIndex(roadNumber);
            roadHolder.transform.position = startPosition;
            T road = roadHolder.AddComponent<T>();
            road.SetDefaults(roadDefaults.nrOfLanes, roadDefaults.laneWidth, roadDefaults.waypointDistance);

            EditorUtility.SetDirty(road);
            AssetDatabase.SaveAssets();
            return road;
        }


        public static Transform GetRoadWaypointsHolder(string trafficWaypointsHolderName)
        {
            bool editingInsidePrefab = GleyPrefabUtilities.EditingInsidePrefab();

            if (roadWaypointsHolder == null)
            {
                GameObject holder = null;
                if (editingInsidePrefab)
                {
                    GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                    Transform waypointsHolder = prefabRoot.transform.Find(trafficWaypointsHolderName);
                    if (waypointsHolder == null)
                    {
                        waypointsHolder = new GameObject(trafficWaypointsHolderName).transform;
                        waypointsHolder.SetParent(prefabRoot.transform);
                        waypointsHolder.gameObject.AddComponent<ConnectionPool>();
                        waypointsHolder.gameObject.tag = Constants.editorTag;
                    }
                    holder = waypointsHolder.gameObject;
                }
                else
                {
                    GameObject[] allObjects = Object.FindObjectsOfType<GameObject>().Where(obj => obj.name == trafficWaypointsHolderName).ToArray();
                    if (allObjects.Length > 0)
                    {
                        for (int i = 0; i < allObjects.Length; i++)
                        {
                            if (!GleyPrefabUtilities.IsInsidePrefab(allObjects[i]))
                            {
                                holder = allObjects[i];
                                break;
                            }
                        }
                    }
                    if (holder == null)
                    {
                        holder = new GameObject(trafficWaypointsHolderName);
                        holder.AddComponent<ConnectionPool>();
                    }
                }
                roadWaypointsHolder = holder.transform;
            }
            return roadWaypointsHolder;
        }


        private int GetFreeRoadNumber(string trafficWaypointsHolderName)
        {
            int nr = 0;
            for (int i = 0; i < GetRoadWaypointsHolder(trafficWaypointsHolderName).childCount; i++)
            {
                if ("Road_" + nr != roadWaypointsHolder.GetChild(i).name)
                {
                    return nr;
                }
                nr++;
            }
            return nr;
        }
    }
}
