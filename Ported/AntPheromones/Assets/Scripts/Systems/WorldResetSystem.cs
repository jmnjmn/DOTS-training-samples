﻿using System;

using UnityEngine;

using Unity.Entities;
using Unity.Mathematics;

public class WorldResetSystem : SystemBase
{
    EntityQuery destroyMapSpawnedEntities;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<ObstacleBuilder>();

        destroyMapSpawnedEntities = GetEntityQuery(new EntityQueryDesc
        {
            Any = new ComponentType[]
                {
                    typeof(Obstacle),
                    typeof(Home),
                    typeof(Food),
                    typeof(RingElement)
                }
        });
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            
            var pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
            var pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);
            
            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                pheromoneBuffer[i] = 0;
            }
            
            ecb.DestroyEntity(destroyMapSpawnedEntities);

            // Can't be part of Query as it has linked entity reference
            Entities
                .WithAll<AntPathing>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();
            
           Entities
               .WithAll<Initialized>()
               .ForEach((Entity entity) =>
               {
                   ecb.RemoveComponent<Initialized>(entity);
               }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }

    public static bool IsPointInsideRing(RingElement ring, float2 point)
    {
        //if (ring.offsets.x - ring.halfThickness > math.abs(point.x) && ring.offsets.y - ring.halfThickness > math.abs(point.y))
        if (ring.offsets.x-ring.halfThickness >= math.length(point))
        {
            return true;
        }

        return false;
    }

    public static bool IsBetween(float2 ab, float value)
    {
        if (ab.x < ab.y)
            return ab.x < value && ab.y > value;
        else
        {
            float delta = (ab.y + math.PI * 2.0F) - ab.x;
            if (value > ab.x)
                return value - ab.x < delta;

            if (value < ab.y)
                return ab.y - value < delta;

            return false;
        }
    }

    public static bool IsWithinOpening(RingElement ring, float2 point)
    {
        float2 P = math.normalize(point);
        float dot = math.dot(P, new float2(0, 1));

        float yaw = 0 > point.x ? math.acos(dot) * -math.PI : math.acos(dot) * math.PI;
        switch (ring.numberOfOpenings)
        {
            case 1:
                {
                    if (IsBetween(ring.opening0.angles, yaw))
                        return true;
                }
                break;
            case 2:
                {
                    if (IsBetween(ring.opening0.angles, yaw))
                        return true;
                    else if (IsBetween(ring.opening1.angles, yaw))
                        return true;
                }
                break;
        }

        return false;
    }

    public static bool DoesPathCollideWithRing(RingElement ring, float2 start, float2 end, out float2 at)
    {
        bool sinside = IsPointInsideRing(ring, start);
        bool einside = IsPointInsideRing(ring, end);

        if (sinside && !einside)
        {
            at = ClosestIntersection(0, 0, math.length(ring.offsets) - ring.halfThickness, start, end);
            return !IsWithinOpening(ring, at);
        }
        else if (!sinside && einside)
        {
            at = ClosestIntersection(0, 0, math.length(ring.offsets) + ring.halfThickness, start, end);
            return IsWithinOpening(ring, at);
        }

        at = default(float2);

        return false;
    }

    // https://stackoverflow.com/questions/23016676/line-segment-and-circle-intersection
    public static float2 ClosestIntersection(float cx, float cy, float radius, float2 lineStart, float2 lineEnd)
    {
        float2 intersection1;
        float2 intersection2;
        int intersections = FindLineCircleIntersections(cx, cy, radius, lineStart, lineEnd, out intersection1, out intersection2);

        if (intersections == 1)
            return intersection1; // one intersection

        if (intersections == 2)
        {
            double dist1 = math.distance(intersection1, lineStart);
            double dist2 = math.distance(intersection2, lineStart);

            if (dist1 < dist2)
                return intersection1;
            else
                return intersection2;
        }

        return default(float2); // no intersections at all
    }

    private static int FindLineCircleIntersections(float cx, float cy, float radius, float2 point1, float2 point2, out float2 intersection1, out float2 intersection2)
    {
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
        C = (point1.x - cx) * (point1.x - cx) + (point1.y - cy) * (point1.y - cy) - radius * radius;

        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = default(float2);
            intersection2 = default(float2);

            return 0;
        }
        else if (det == 0)
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 = new float2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = default(float2);

            return 1;
        }
        else
        {
            // Two solutions.
            t = (float)((-B + math.sqrt(det)) / (2 * A));
            intersection1 = new float2(point1.x + t * dx, point1.y + t * dy);
            t = (float)((-B - math.sqrt(det)) / (2 * A));
            intersection2 = new float2(point1.x + t * dx, point1.y + t * dy);

            return 2;
        }
    }
}
