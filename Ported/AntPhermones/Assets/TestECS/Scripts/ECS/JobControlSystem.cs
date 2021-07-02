using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class JobControlSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (GameRoot.Instance.useJobSystem)
        {
            float deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Rotation rotate, ref Translation translation,ref RotateSpeed rotateSpeed) =>
            {
                Quaternion q = rotate.Value;
                var ang = q.eulerAngles;
                ang.y = ang.y + deltaTime * rotateSpeed.Speed;
                rotate.Value = Quaternion.Euler(ang);

                Vector3 pos = Random.insideUnitSphere * 40;
                translation.Value = pos;
            }).Run();
        }

        return inputDeps;
    }
}