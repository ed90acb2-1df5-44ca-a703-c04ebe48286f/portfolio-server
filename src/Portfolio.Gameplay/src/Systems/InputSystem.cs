using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Systems;

public class InputSystem : ISystem
{
    private readonly World _world;
    private readonly Query _query;

    public InputSystem(World world)
    {
        _world = world;
        _query = new QueryBuilder(world)
            .Require<Input>()
            .Require<Velocity>()
            .Require<Attributes>()
            .Build();
    }

    public void Tick(float delta)
    {
        var inputs = _world.Components<Input>();
        var velocities = _world.Components<Velocity>();
        var attributes = _world.Components<Attributes>();

        foreach (var entity in _world.Query(_query))
        {
            var direction = Vector2.Normalize(inputs[entity].Direction);
            velocities[entity].Value = direction * attributes[entity].Speed;
        }
    }
}
