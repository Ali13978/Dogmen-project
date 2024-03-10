#if USE_GLEY_TRAFFIC
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Rotates the vehicle trigger on the heading direction
    /// </summary>
    [BurstCompile]
    public struct UpdateTriggerJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float> turnAngle;
        [ReadOnly] public NativeArray<SpecialDriveActionTypes> specialDriveAction;
        public void Execute(int index, TransformAccess transform)
        {
            if (specialDriveAction[index] != SpecialDriveActionTypes.AvoidForward)
            {
                transform.localRotation = quaternion.EulerZXY(0, math.radians(turnAngle[index]), 0);
            }
        }
    }
}
#endif
