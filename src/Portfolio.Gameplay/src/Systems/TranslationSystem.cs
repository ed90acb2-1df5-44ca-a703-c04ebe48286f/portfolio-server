using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Systems;

public class TranslationSystem : ISystem
{
    private readonly World _world;
    private readonly Query _query;

    public TranslationSystem(World world)
    {
        _world = world;
        _query = new QueryBuilder(world)
            .Require<Position>()
            .Require<Velocity>()
            .Build();
    }

    public void Tick(float delta)
    {
        var positions = _world.Components<Position>();
        var velocities = _world.Components<Velocity>();

        foreach (var entity in _world.Query(_query))
        {
            positions[entity.Index].Value += velocities[entity.Index].Value * delta;
        }
    }
}
