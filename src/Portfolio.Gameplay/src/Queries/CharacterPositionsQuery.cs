using System;
using System.Buffers;
using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Queries;

public class CharacterPositionsQuery : IQuery<CharacterPositionsQuery.Result>
{
    public Result Execute(World world)
    {
        var query = new QueryBuilder(world)
            .Require<Player>()
            .Require<Position>()
            .Build();

        var entities = world.Query(query);
        var result = new Result(entities.Length);
        var index = 0;

        foreach (var entity in entities)
        {
            var position = world.GetComponent<Position>(entity).Value;
            result.Positions[index++] = position;
        }

        return result;
    }

    public readonly struct Result : IDisposable
    {
        public readonly Vector2[] Positions;

        public Result(int count)
        {
            Positions = ArrayPool<Vector2>.Shared.Rent(count);
        }

        public void Dispose()
        {
            ArrayPool<Vector2>.Shared.Return(Positions);
        }
    }
}
