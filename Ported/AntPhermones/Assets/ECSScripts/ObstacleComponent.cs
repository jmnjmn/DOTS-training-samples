using Unity.Entities;
using Unity.Mathematics;

namespace ECSScripts
{
    public struct ObstacleComponent:IComponentData
    {
        public float2 position;
        public float radius;
    }
}