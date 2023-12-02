using System;
using System.Collections.Generic;
using System.Threading;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Events;
using Portfolio.Protocol.Messages;
using Portfolio.Server.Mappers;
using Portfolio.Server.Net;

namespace Portfolio.Server.Threads;

public class BroadcastMessagesThread
{
    private readonly Game _game;
    private readonly INetwork _network;
    private readonly ILogger _logger;

    private readonly Dictionary<Type, object> _mappers = new();

    public BroadcastMessagesThread(Game game, INetwork network, ILogger logger)
    {
        _game = game;
        _network = network;
        _logger = logger;

        RegisterMessageMapper(new EntityDamagedEventMapper());
        RegisterMessageMapper(new PlayerSpawnedEventMapper());

        return;

        void RegisterMessageMapper<TEvent, TMessage>(IMapper<TEvent, TMessage> mapper)
        {
            _mappers.Add(typeof(TEvent), mapper);
        }
    }

    public void Start()
    {
        var thread = new Thread(ThreadStart);
        thread.Name = nameof(BroadcastMessagesThread);

        _logger.Information($"Starting {thread.Name}");

        thread.Start();
    }

    private void ThreadStart()
    {
        try
        {
            while (true)
            {
                SendMessages<PlayerSpawnedEvent, PlayerSpawnedMessage>(_game);
                SendMessages<EntityDamagedEvent, EntityDamagedMessage>(_game);

                Thread.Sleep(1000 / 60);
            }
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }

    private void SendMessages<TMessage, TNetworkMessage>(Game game)
    {
        var messages = game.Messages<TMessage>();

        while (messages.TryDequeue(out var message))
        {
            var networkMessage = ((IMapper<TMessage, TNetworkMessage>) _mappers[typeof(TMessage)]).Map(message.Payload);

            switch (message.Receiver)
            {
                case MessageReceiver.Player:
                    _network.Direct(new Connection(message.PlayerId), networkMessage, DeliveryMethod.Reliable);
                    break;
                case MessageReceiver.Fanout:
                    _network.Fanout(networkMessage, DeliveryMethod.Reliable);
                    break;
                case MessageReceiver.FanoutExcept:
                    _network.FanoutExcept(networkMessage, DeliveryMethod.Reliable, new Connection(message.PlayerId));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.Receiver), message.Receiver, "Unknown message receiver.");
            }
        }
    }
}
