
using Unity.Entities;

public struct GameConfiguration: IComponentData
{
    public Entity BeeTeamAPrefab;
    public Entity BeeTeamBPrefab;
    public Entity FoodPrefab;
    public Entity BloodDropletPrefab;
    public float HivePosition;
}
