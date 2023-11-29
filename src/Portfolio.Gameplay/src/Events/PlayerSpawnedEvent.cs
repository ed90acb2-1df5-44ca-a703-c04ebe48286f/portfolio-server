using System.Numerics;
using Portfolio.Entities;

namespace Portfolio.Gameplay.Events;

public readonly struct PlayerSpawnedEvent
{
    public readonly Entity Entity;
    public readonly int PlayerId;
    public readonly Vector2 Position;

    public PlayerSpawnedEvent(Entity entity, int playerId, Vector2 position)
    {
        Entity = entity;
        PlayerId = playerId;
        Position = position;
    }
}
