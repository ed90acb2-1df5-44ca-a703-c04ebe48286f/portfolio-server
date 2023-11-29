using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Portfolio.Gameplay;

public class GameEvents
{
    private readonly Dictionary<Type, object> _queues = new();

    public void Add<TEvent>(TEvent @event)
    {
        var eventType = typeof(TEvent);

        if (!_queues.TryGetValue(eventType, out var queue))
        {
            queue = new ConcurrentQueue<TEvent>();
            _queues.Add(eventType, queue);
        }

        ((ConcurrentQueue<TEvent>) queue).Enqueue(@event);
    }

    public ConcurrentQueue<TEvent> Get<TEvent>()
    {
        var eventType = typeof(TEvent);

        if (!_queues.TryGetValue(eventType, out var queue))
        {
            queue = new ConcurrentQueue<TEvent>();
            _queues.Add(eventType, queue);
        }

        return (ConcurrentQueue<TEvent>) queue;
    }
}
