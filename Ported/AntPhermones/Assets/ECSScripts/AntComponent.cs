using Unity.Entities;
using Unity.Mathematics;

namespace ECSScripts
{
    public struct AntComponent:IComponentData
    {
        public float2 position;
        public float facingAngle;
        public float speed;
        public bool holdingResource;
        public float brightness;
    }
}