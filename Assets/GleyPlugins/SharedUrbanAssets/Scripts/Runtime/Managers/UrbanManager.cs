using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if USE_GLEY_PEDESTRIANS || USE_GLEY_TRAFFIC
using Unity.Mathematics;
#endif

namespace GleyUrbanAssets
{
    public class UrbanManager : MonoBehaviour
    {
        protected GridManager gridManager;
        internal static UrbanManager urbanManagerInstance;

        internal GridManager GetGridManager()
        {
            if (gridManager == null)
            {
                Debug.LogWarning("Grid manager is null");
            }
            return gridManager;
        }

#if USE_GLEY_PEDESTRIANS || USE_GLEY_TRAFFIC
        protected void AddGridManager(CurrentSceneData currentSceneData, NativeArray<float3> activeCameraPositions)
        {

            gridManager = currentSceneData.gameObject.GetComponent<GridManager>();
            if (gridManager == null)
            {

                gridManager = currentSceneData.gameObject.AddComponent<GridManager>().Initialize(currentSceneData, activeCameraPositions);
            }
    }
#endif
    }
}