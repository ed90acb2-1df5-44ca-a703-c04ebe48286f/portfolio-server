using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Portfolio.Gameplay;

public class MessageQueue
{
    private readonly Dictionary<Type, object> _messages = new();

    public void Direct<TPayload>(int playerId, TPayload payload)
    {
        Enqueue(playerId, MessageReceiver.Player, payload);
    }

    public void Fanout<TPayload>(TPayload payload)
    {
        Enqueue(-1, MessageReceiver.Fanout, payload);
    }

    public void FanoutExcept<TPayload>(TPayload payload, int playerId)
    {
        Enqueue(playerId, MessageReceiver.FanoutExcept, payload);
    }

    public ConcurrentQueue<Message<TPayload>> Get<TPayload>()
    {
        var eventType = typeof(TPayload);

        if (!_messages.TryGetValue(eventType, out var queue))
        {
            queue = new ConcurrentQueue<Message<TPayload>>();
            _messages.Add(eventType, queue);
        }

        return (ConcurrentQueue<Message<TPayload>>) queue;
    }

    private void Enqueue<TPayload>(int playerId, MessageReceiver receiver, TPayload payload)
    {
        var eventType = typeof(TPayload);

        if (!_messages.TryGetValue(eventType, out var queue))
        {
            queue = new ConcurrentQueue<Message<TPayload>>();
            _messages.Add(eventType, queue);
        }

        ((ConcurrentQueue<Message<TPayload>>) queue).Enqueue(new Message<TPayload>(playerId, receiver, payload));
    }
}
