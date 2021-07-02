using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MainECS : MainBase
{
    public override void CreateObject(Material mat, int count, Mesh mesh)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager; //获取默认世界的实体管理器


        // 创建entity 块类型
        EntityArchetype archetype = manager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(RenderBounds),
            typeof(RotateSpeed)
        );


        // 创建entity
        var entities = new NativeArray<Entity>(count, Allocator.Temp);
        manager.CreateEntity(archetype, entities);

        // 给entity 组件赋值
        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;

            RenderBounds rb = new RenderBounds()
            {
                Value = new Unity.Mathematics.AABB()
                {
                    Center = pos,
                    Extents = new Unity.Mathematics.float3(0.5f, 0.5f, 0.5f)
                }
            };

            //为实体设置数据
            manager.SetComponentData<RenderBounds>(entity, rb); //只需添加archerType即可，不需要手动设置数据
            manager.SetComponentData<Rotation>(entity, new Rotation() {Value = quaternion.identity});
            manager.SetComponentData<Translation>(entity, new Translation() {Value = pos});
            manager.SetComponentData(entity, new RotateSpeed() {Speed = 100});
            manager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh()
            {
                material = mat,
                mesh = mesh,
                subMesh = 0,
                castShadows = UnityEngine.Rendering.ShadowCastingMode.On,
                receiveShadows = true,
                needMotionVectorPass = true
            });
        }

        entities.Dispose();
    }
}