using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using Portfolio.Entities;
using Portfolio.Gameplay.Commands;
using Portfolio.Gameplay.Components;
using Portfolio.Gameplay.Events;
using Portfolio.Gameplay.Queries;
using Portfolio.Gameplay.Systems;

namespace Portfolio.Gameplay;

public class Game
{
    public int Tick { get; private set; }

    private readonly World _world;
    private readonly GameEvents _events;
    private readonly ISystem[] _systems;

    private readonly ConcurrentDictionary<int, Entity> _players = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public Game()
    {
        _world = new World();
        _events = new GameEvents();

        _systems = new ISystem[]
        {
            new TranslationSystem(_world),
        };
    }

    public void SpawnPlayer(int playerId)
    {
        try
        {
            _lock.EnterWriteLock();

            var entity = _world.Create();
            _world.SetComponent(entity, new Player(playerId));
            _world.SetComponent(entity, new Networked());
            _world.SetComponent(entity, new Input());
            _world.SetComponent(entity, new MoveUsingInput());
            _world.SetComponent(entity, new Position());
            _world.SetComponent(entity, new Velocity());
            _world.SetComponent(entity, new Attributes());
            _players[playerId] = entity;

            _events.Add(new PlayerSpawnedEvent(entity, playerId, new Vector2()));
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Update()
    {
        try
        {
            _lock.EnterWriteLock();

            var delta = (float) _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();

            for (var i = 0; i < _systems.Length; i++)
            {
                _systems[i].Tick(delta);
            }

            Tick += 1;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public ConcurrentQueue<TEvent> Events<TEvent>()
    {
        return _events.Get<TEvent>();
    }

    public void Command<TCommand>(int playerId, TCommand command) where TCommand : IPlayerCommand
    {
        try
        {
            _lock.EnterWriteLock();
            command.Execute(_players[playerId], _world);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public TResult Query<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        try
        {
            _lock.EnterReadLock();
            return query.Execute(_world);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}
