using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Systems;

public class MovementSystem : ISystem
{
    private readonly World _world;
    private readonly Query _query;

    public MovementSystem(World world)
    {
        _world = world;
        _query = new QueryBuilder(world)
            .Require<PositionComponent>()
            .Require<VelocityComponent>()
            .Build();
    }

    public void Tick(float delta)
    {
        var positions = _world.Components<PositionComponent>();
        var velocities = _world.Components<VelocityComponent>();

        foreach (var entity in _world.Query(_query))
        {
            positions[entity.Index].Value += velocities[entity.Index].Value * delta;
        }
    }
}
