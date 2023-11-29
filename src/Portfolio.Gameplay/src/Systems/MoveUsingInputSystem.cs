using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Systems;

public class MoveUsingInputSystem : ISystem
{
    private readonly World _world;
    private readonly Query _query;

    public MoveUsingInputSystem(World world)
    {
        _world = world;
        _query = new QueryBuilder(world)
            .Require<Input>()
            .Require<Velocity>()
            .Require<Attributes>()
            .Require<MoveUsingInput>()
            .Build();
    }

    public void Tick(float delta)
    {
        var entities = _world.Query(_query);

        for (var i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var input = _world.GetComponent<Input>(entity);
            var attributes = _world.GetComponent<Attributes>(entity);
            var velocity = _world.GetComponent<Velocity>(entity);

            var direction = Vector2.Normalize(input.Direction);
            velocity.Value = direction * attributes.Speed;
        }
    }
}
