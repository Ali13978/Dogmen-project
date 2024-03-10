#if USE_GLEY_TRAFFIC
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Compotes the suspension force for each wheel
    /// </summary>
    [BurstCompile]
    public struct WheelJob : IJobParallelFor
    {
        public NativeArray<float3> wheelSuspensionForce;

        [ReadOnly] public NativeArray<float3> wheelNormalDirection;
        [ReadOnly] public NativeArray<float> springForces;
        [ReadOnly] public NativeArray<float> wheelRaycastDistance;
        [ReadOnly] public NativeArray<float> wheelRadius;
        [ReadOnly] public NativeArray<float> wheelMaxSuspension;
        [ReadOnly] public NativeArray<float> targetCompression;
        [ReadOnly] public NativeArray<int> startWheelIndex;
        [ReadOnly] public NativeArray<int> nrOfCarWheels;
        [ReadOnly] public NativeArray<int> wheelAssociatedCar;
        public void Execute(int i)
        {
            if (wheelMaxSuspension[i] != 0)
            {
                wheelSuspensionForce[i] = ComputeSuspensionForce(springForces[wheelAssociatedCar[i]], 1f - (wheelRaycastDistance[i] - wheelRadius[i]) / wheelMaxSuspension[i], wheelNormalDirection[i], targetCompression[wheelAssociatedCar[i]]);
            }
            else
            {
                wheelSuspensionForce[i] = ComputeSuspensionForce(springForces[wheelAssociatedCar[i]], 1, wheelNormalDirection[i], 1);
            }
        }

        float3 ComputeSuspensionForce(float springForce, float compression, float3 normalPoint, float targetCompression)
        {
            return springForce * (compression / targetCompression) * normalPoint;
        }
    }
}
#endif
