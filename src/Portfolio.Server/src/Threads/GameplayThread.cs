using System;
using System.Threading;
using Portfolio.Gameplay;

namespace Portfolio.Server.Threads;

public class GameplayThread
{
    private readonly Game _game;
    private readonly ILogger _logger;

    public GameplayThread(Game game, ILogger logger)
    {
        _game = game;
        _logger = logger;
    }

    public void Start()
    {
        var thread = new Thread(ThreadStart);
        thread.Name = nameof(GameplayThread);

        _logger.Information($"Starting {thread.Name}");

        thread.Start();
    }

    private void ThreadStart()
    {
        try
        {
            while (true)
            {
                _game.Update();
                Thread.Sleep(1000 / 60);
            }
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
