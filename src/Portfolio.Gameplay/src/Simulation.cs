using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Portfolio.Entities;
using Portfolio.Gameplay.Components;
using Portfolio.Gameplay.Systems;

namespace Portfolio.Gameplay;

public class Simulation
{
    private readonly World _world;
    private readonly ISystem[] _systems;
    private readonly ILogger _logger;

    private readonly Dictionary<int, Entity> _characters = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public Simulation(ILogger logger)
    {
        _logger = logger;
        _world = new World();

        _systems = new ISystem[]
        {
            new MovementSystem(_world)
        };
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
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void SpawnPlayerCharacter(int peer)
    {
        try
        {
            _lock.EnterWriteLock();

            var entity = _world.Create();
            _world.SetComponent(entity, new PositionComponent());
            _world.SetComponent(entity, new VelocityComponent());
            _characters[peer] = entity;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public Entity GetPlayerCharacter(int peer)
    {
        try
        {
            _lock.EnterReadLock();
            return _characters[peer];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Command(ICommand command)
    {
        try
        {
            _lock.EnterWriteLock();
            command.Execute(_world);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public T Query<T>(IQuery<T> query)
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
