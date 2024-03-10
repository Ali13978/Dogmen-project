using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class RoadsLoader : Editor
    {
        static RoadsLoader instance;


        public static RoadsLoader Initialize()
        {
            if (instance == null)
            {
                instance = CreateInstance<RoadsLoader>();
            }
            return instance;
        }


        internal List<T> LoadAllRoads<T>() where T : RoadBase
        {
            List<T> allRoads;
            if (GleyPrefabUtilities.EditingInsidePrefab())
            {
                GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                allRoads = prefabRoot.GetComponentsInChildren<T>().ToList();
                for (int i = 0; i < allRoads.Count; i++)
                {
                    allRoads[i].positionOffset = prefabRoot.transform.position;
                    allRoads[i].rotationOffset = prefabRoot.transform.localEulerAngles;
                }
            }
            else
            {
                allRoads = FindObjectsOfType<T>().ToList();
                for (int i = 0; i < allRoads.Count; i++)
                {
                    allRoads[i].isInsidePrefab = GleyPrefabUtilities.IsInsidePrefab(allRoads[i].gameObject);
                    if (allRoads[i].isInsidePrefab)
                    {
                        allRoads[i].positionOffset = GleyPrefabUtilities.GetInstancePrefabRoot(allRoads[i].gameObject).transform.position;
                        allRoads[i].rotationOffset = GleyPrefabUtilities.GetInstancePrefabRoot(allRoads[i].gameObject).transform.localEulerAngles;
                    }
                }
            }
            return allRoads;
        }
    }
}
