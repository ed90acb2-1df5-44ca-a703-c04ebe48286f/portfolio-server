using System;
using System.Buffers;
using System.Numerics;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;

namespace Portfolio.Gameplay.Queries;

public readonly struct WorldStateQuery : IQuery<WorldStateQuery.Result>
{
    public Result Execute(World world)
    {
        var query = new QueryBuilder(world)
            .Require<Position>()
            .Require<Networked>()
            .Build();

        var entities = world.Query(query);
        var result = new Result(entities.Length);

        for (var i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var position = world.GetComponent<Position>(entity).Value;
            result.Entities[i] = new EntityState(entity.Index, position, 1);
        }

        return result;
    }

    public readonly struct Result : IDisposable
    {
        public readonly EntityState[] Entities;

        public Result(int count)
        {
            Entities = ArrayPool<EntityState>.Shared.Rent(count);
        }

        public void Dispose()
        {
            ArrayPool<EntityState>.Shared.Return(Entities);
        }
    }

    public readonly struct EntityState
    {
        public readonly int EntityId;
        public readonly Vector2 Position;
        public readonly int Health;

        public EntityState(int entityId, Vector2 position, int health)
        {
            EntityId = entityId;
            Position = position;
            Health = health;
        }
    }
}
