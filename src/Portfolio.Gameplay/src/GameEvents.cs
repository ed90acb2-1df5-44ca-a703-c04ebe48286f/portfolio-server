using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Portfolio.Gameplay;

public class GameEvents
{
    private readonly Dictionary<Type, object> _events = new();

    public void Add<TEvent>(TEvent @event)
    {
        var eventType = typeof(TEvent);

        if (!_events.ContainsKey(eventType))
        {
            _events.Add(eventType, new ConcurrentQueue<TEvent>());
        }

        ((ConcurrentQueue<TEvent>) _events[eventType]).Enqueue(@event);
    }

    public ConcurrentQueue<TEvent> Get<TEvent>()
    {
        var eventType = typeof(TEvent);

        if (!_events.ContainsKey(eventType))
        {
            _events.Add(eventType, new ConcurrentQueue<TEvent>());
        }

        return (ConcurrentQueue<TEvent>) _events[eventType];
    }
}
