using System;
using System.Collections.Generic;
using System.Threading;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Events;
using Portfolio.Gameplay.Queries;
using Portfolio.Protocol.Messages;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Requests;
using Portfolio.Protocol.Values;
using Portfolio.Server.Controllers;
using Portfolio.Server.Filters;
using Portfolio.Server.Mappers;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server;

public class Server
{
    private readonly ILogger _logger;
    private readonly INetworkKernel _network;
    private readonly Thread _networkingThread;
    private readonly Thread _gameThread;
    private readonly Dictionary<Type, object> _mappers = new();

    public Server(ILogger logger, Router router, INetworkKernel network, Authentication authentication)
    {
        _logger = logger;
        _network = network;

        _networkingThread = new Thread(NetworkingLoop);
        _networkingThread.Name = "Networking";

        _gameThread = new Thread(GameLoop);
        _gameThread.Name = "Game";

        router.CreateEndpoint<LoginRequest, LoginController>();
        router.CreateEndpoint<RegistrationRequest, RegistrationController>();

        new EndpointGroup()
            .Add(router.CreateEndpoint<InputRequest, InputController>())
            .Filter(new AuthenticationConnectionFilter(authentication));

        _network.SetRouter(router);

        RegisterMapper(new EntityDamagedEventMapper());
        RegisterMapper(new PlayerSpawnedEventMapper());
    }

    private void RegisterMapper<TEvent, TMessage>(IMapper<TEvent, TMessage> mapper)
    {
        _mappers.Add(typeof(TEvent), mapper);
    }

    public void Start()
    {
        _networkingThread.Start();
        _gameThread.Start();
    }

    private void NetworkingLoop()
    {
        _logger.Information("Starting networking...");
        _network.Start();

        while (true)
        {
            _network.Update();

            Thread.Sleep(1000 / 60);
        }
    }

    private void GameLoop()
    {
        _logger.Information("Starting game...");
        var game = new Game();

        _logger.Information("Starting game event broadcast...");
        var broadcastGameEventsThread = new Thread(BroadcastGameEventsLoop);
        broadcastGameEventsThread.Name = "BroadcastGameEvents";
        broadcastGameEventsThread.Start(game);

        _logger.Information("Starting game state broadcast...");
        var broadcastGameStateThread = new Thread(BroadcastGameStateLoop);
        broadcastGameStateThread.Name = "BroadcastGameState";
        broadcastGameStateThread.Start(game);

        while (true)
        {
            try
            {
                game.Update();
            }
            catch (Exception exception)
            {
                _logger.Exception(exception);
            }

            Thread.Sleep(1000 / 60);
        }
    }

    private void BroadcastGameEventsLoop(object? context)
    {
        var game = (Game) context!;

        while (true)
        {
            try
            {
                BroadcastEvents<PlayerSpawnedEvent, PlayerSpawnedMessage>(game);
                BroadcastEvents<EntityDamagedEvent, EntityDamagedMessage>(game);
            }
            catch (Exception exception)
            {
                _logger.Exception(exception);
            }

            Thread.Sleep(1000 / 60);
        }
    }

    private void BroadcastEvents<TEvent, TMessage>(Game game)
    {
        var events = game.Events<TEvent>();

        while (events.TryDequeue(out var @event))
        {
            var message = ((IMapper<TEvent, TMessage>) _mappers[typeof(TEvent)]).Map(@event);
            _network.Broadcast(message, DeliveryMethod.Reliable);
        }
    }

    private void BroadcastGameStateLoop(object? context)
    {
        var game = (Game) context!;

        var message = new WorldStateMessage();

        while (true)
        {
            try
            {
                using var result = game.Query<WorldStateQuery, WorldStateQuery.Result>(new WorldStateQuery());

                message.Entities.Clear();

                foreach (var entity in result.Entities)
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

                _network.Broadcast(message, DeliveryMethod.Unreliable);
            }
            catch (Exception exception)
            {
                _logger.Exception(exception);
            }

            Thread.Sleep(100);
        }
    }
}
