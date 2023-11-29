using Portfolio.Entities;
using Portfolio.Gameplay.Components;
using Portfolio.Gameplay.Events;

namespace Portfolio.Gameplay.Systems;

public class DamageSystem : ISystem
{
    private readonly World _world;
    private readonly GameEvents _events;
    private readonly Query _query;

    public DamageSystem(World world, GameEvents events)
    {
        _world = world;
        _events = events;
        _query = new QueryBuilder(world)
            .Require<IncomingDamage>()
            .Require<Health>()
            .Build();
    }

    public void Tick(float delta)
    {
        var entities = _world.Query(_query);

        for (var i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var incomingDamage = _world.GetComponent<IncomingDamage>(entity);
            var health = _world.GetComponent<Health>(entity);

            while (incomingDamage.Value.TryDequeue(out var damage))
            {
                health.Value -= damage.Amount;

                //_events.Add(new EntityDamagedEvent(damage, entity, entity));
            }
        }
    }
}
