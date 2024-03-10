using System;
using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class GleyUtilities
    {
        static Camera sceneCamera;
        private static Vector3 oldPivot;
        private static float oldCameraDistance;


        public static bool IsPointInsideView(Vector3 position)
        {
            if (sceneCamera == null)
            {
                sceneCamera = SceneView.lastActiveSceneView.camera;
            }
            Vector3 screenPosition = sceneCamera.WorldToViewportPoint(position);
            if (screenPosition.x > 1 || screenPosition.x < 0 || screenPosition.y > 1 || screenPosition.y < 0)
            {
                return false;
            }
            return true;
        }


        public static void TeleportSceneCamera(Vector3 cam_position, float height = 1)
        {
            var scene_view = SceneView.lastActiveSceneView;
            if (scene_view != null)
            {
                scene_view.Frame(new Bounds(cam_position, Vector3.one * height), false);
            }
        }


        public static bool SceneCameraMoved()
        {
            if (oldPivot != SceneView.lastActiveSceneView.pivot || oldCameraDistance != SceneView.lastActiveSceneView.cameraDistance)
            {
                oldPivot = SceneView.lastActiveSceneView.pivot;
                oldCameraDistance = SceneView.lastActiveSceneView.cameraDistance;
                return true;
            }
            return false;
        }
    }
}
