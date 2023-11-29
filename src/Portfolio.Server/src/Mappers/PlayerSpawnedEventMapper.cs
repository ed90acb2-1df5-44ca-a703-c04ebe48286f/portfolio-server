using Portfolio.Gameplay.Events;
using Portfolio.Protocol.Messages;
using Portfolio.Protocol.Values;

namespace Portfolio.Server.Mappers;

public class PlayerSpawnedEventMapper : IMapper<PlayerSpawnedEvent, PlayerSpawnedMessage>
{
    public PlayerSpawnedMessage Map(PlayerSpawnedEvent source)
    {
        return new PlayerSpawnedMessage
        {
            EntityId = source.Entity.Index,
            PlayerId = source.PlayerId,
            Position = new Vector2()
        };
    }
}
