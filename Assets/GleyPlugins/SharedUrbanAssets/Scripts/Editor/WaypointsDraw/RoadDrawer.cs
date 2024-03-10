using UnityEditor;
using UnityEngine;

namespace GleyUrbanAssets
{
    public class RoadDrawer : Editor
    {
        const float segmentSelectDistanceThreshold = 1f;

        static RoadDrawer instance;
        static GUIStyle style;


        public static RoadDrawer Initialize()
        {
            if (instance == null)
            {
                instance = CreateInstance<RoadDrawer>();
            }
            style = new GUIStyle();
            return instance;
        }


        internal void DrawPath(RoadBase road, MoveTools moveTool, Color roadColor, Color anchorColor, Color controlColor, Color textColor)
        {
            if (road.isInsidePrefab && !GleyPrefabUtilities.EditingInsidePrefab())
            {
                return;
            }
            Path path = road.path;
            //draw path
            for (int i = 0; i < path.NumSegments; i++)
            {
                Vector3[] points = path.GetPointsInSegment(i, road.positionOffset);
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], roadColor, null, 2);
            }
            style.normal.textColor = textColor;
            Handles.Label(road.path[0], road.gameObject.name, style);
            Handles.Label(road.path[path.NumPoints - 1], road.gameObject.name, style);

            //draw points
            for (int i = 0; i < path.NumPoints; i++)
            {
                float handleSize;
                Color handleColor;
                if (i % 3 == 0)
                {
                    handleSize = Customizations.GetControlPointSize(SceneView.lastActiveSceneView.camera.transform.position, path.GetPoint(i, road.positionOffset));
                    handleColor = controlColor;
                }
                else
                {
                    handleSize = Customizations.GetAnchorPointSize(SceneView.lastActiveSceneView.camera.transform.position, path.GetPoint(i, road.positionOffset));
                    handleColor = anchorColor;
                }
                Handles.color = handleColor;
                Vector3 newPos = path[i];

                switch (moveTool)
                {
                    case MoveTools.None:
                        break;
                    case MoveTools.Move3D:
                        newPos = Handles.PositionHandle(path.GetPoint(i, road.positionOffset), Quaternion.identity);
                        Handles.SphereHandleCap(0, path.GetPoint(i, road.positionOffset), Quaternion.identity, handleSize, EventType.Repaint);
                        break;
                    case MoveTools.Move2D:
                        var fmh_72_96_638456009657963200 = Quaternion.identity; newPos = Handles.FreeMoveHandle(path.GetPoint(i, road.positionOffset), handleSize, Vector2.zero, Handles.SphereHandleCap);
                        newPos.y = path.GetPoint(i, road.positionOffset).y;
                        break;
                }
                if (path[i] != newPos)
                {
                    Undo.RecordObject(road, "Move point");
                    path.MovePoint(i, newPos - road.positionOffset);
                }
            }
        }


        internal void SelectSegmentIndex(RoadBase road, Vector3 mousePosition)
        {
            float minDstToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;

            for (int i = 0; i < road.path.NumSegments; i++)
            {
                Vector3[] points = road.path.GetPointsInSegment(i, road.positionOffset);
                float dst = HandleUtility.DistancePointBezier(mousePosition, points[0], points[3], points[1], points[2]);
                if (dst < minDstToSegment)
                {
                    minDstToSegment = dst;
                    newSelectedSegmentIndex = i;
                }
            }

            if (newSelectedSegmentIndex != road.selectedSegmentIndex)
            {
                road.selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }


        internal void AddPathPoint(Vector3 mousePosition, RoadBase road)
        {
            if (road.selectedSegmentIndex == -1)
            {
                Undo.RecordObject(road, "Add segment");
                road.path.AddSegment(mousePosition);
            }
            else
            {
                Undo.RecordObject(road, "Split segment");
                road.path.SplitSegment(mousePosition, road.selectedSegmentIndex);
            }
        }


        internal void Delete(RoadBase road, Vector3 mousePosition)
        {
            float minDstToAnchor = 1 * .5f;
            int closestAnchorIndex = -1;

            for (int i = 0; i < road.path.NumPoints; i += 3)
            {
                float dst = Vector2.Distance(mousePosition, road.path[i]);
                if (dst < minDstToAnchor)
                {
                    minDstToAnchor = dst;
                    closestAnchorIndex = i;
                }
            }

            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(road, "Delete segment");
                road.path.DeleteSegment(closestAnchorIndex);
            }
        }
    }
}
