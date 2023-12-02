using System;
using System.Threading;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Queries;
using Portfolio.Protocol.Messages;
using Portfolio.Protocol.Values;
using Portfolio.Server.Net;

namespace Portfolio.Server.Threads;

public class BroadcastGameStateThread
{
    private readonly Game _game;
    private readonly INetwork _network;
    private readonly ILogger _logger;

    public BroadcastGameStateThread(Game game, INetwork network, ILogger logger)
    {
        _game = game;
        _network = network;
        _logger = logger;
    }

    public void Start()
    {
        var thread = new Thread(ThreadStart);
        thread.Name = nameof(BroadcastGameStateThread);

        _logger.Information($"Starting {thread.Name}");

        thread.Start();
    }

    private void ThreadStart()
    {
        try
        {
            var message = new WorldStateMessage();

            while (true)
            {
                using var result = _game.Query<WorldStateQuery, WorldStateQuery.Result>(new WorldStateQuery());

                message.Entities.Clear();

                foreach (var entity in result.Entities())
                {
                    message.Entities.Add(
                        new WorldStateMessage.Types.EntityState
                        {
                            EntityId = entity.EntityId,
                            Position = new Vector2
                            {
                                X = entity.Position.X,
                                Y = entity.Position.Y
                            },
                        });
                }

                _network.Fanout(message, DeliveryMethod.Unreliable);

                Thread.Sleep(100);
            }
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
