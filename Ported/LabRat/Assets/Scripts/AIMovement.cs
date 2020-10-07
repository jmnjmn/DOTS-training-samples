using Unity.Entities;
using Unity.Mathematics;

public class AIMovement : SystemBase
{
    static readonly float AISpeed = 10f;
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var randomSeed = (uint)System.DateTime.Now.Ticks;
        var boardSize = GetSingleton<GameInfo>().boardSize;
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<Timer, AICursor>().ForEach((Entity e, int entityInQueryIndex, in Timer timer) =>
        {
            if(timer.Value <= 0f)
                ecb.RemoveComponent<Timer>(entityInQueryIndex, e);
        }).ScheduleParallel();
        
        Entities.WithNone<Timer>().ForEach((Entity e, int entityInQueryIndex, ref AICursor cursor, ref Position position, ref DynamicBuffer<PlayerArrow> arrows) =>
        {
            var direction = cursor.Destination - position.Value;
            var distance = math.lengthsq(direction);
            var movement = deltaTime * AISpeed * math.normalize(direction);
            var movementLength = math.lengthsq(movement);

            if (distance == 0 || movementLength >= distance)
            {
                position.Value = cursor.Destination;
                var random = new Random((uint)(randomSeed + entityInQueryIndex));
                cursor.Destination = new int2(random.NextInt(0, boardSize.x), random.NextInt(0, boardSize.y));
                ecb.AddComponent(entityInQueryIndex, e, new Timer(){Value = random.NextFloat(1f,2f)});
                //TODO : get the cell and add a Direction and player to it?
                PlaceArrow(arrows);
            }
            else
            {
                position.Value += movement;
            }

        }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    static void PlaceArrow(DynamicBuffer<PlayerArrow> arrows)
    {
        if (arrows.Length == 3)
        {
            arrows.RemoveAt(0);
        }

        arrows.Add(new PlayerArrow() { TileIndex = 0 });
    }
}
