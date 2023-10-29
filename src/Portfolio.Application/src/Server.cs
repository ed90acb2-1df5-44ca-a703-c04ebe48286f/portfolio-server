using System;
using System.Threading;
using Portfolio.Application.Controllers;
using Portfolio.Application.Net;
using Portfolio.Protocol.Commands;
using Portfolio.Protocol.Messages;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Queries;

namespace Portfolio.Application;

public class Server
{
    private readonly ILogger _logger;
    private readonly INetworking _networking;
    private readonly Thread _networkingThread;
    private readonly Thread _simulationThread;

    public Server(ILogger logger, INetworking networking)
    {
        _logger = logger;
        _networking = networking;

        _networkingThread = new Thread(NetworkingLoop);
        _networkingThread.Name = "Networking";

        _simulationThread = new Thread(SimulationLoop);
        _simulationThread.Name = "Simulation";

        networking.RegisterController<LoginCommand, LoginController>();
    }

    public void Start()
    {
        _networkingThread.Start();
        _simulationThread.Start();
    }

    private void NetworkingLoop()
    {
        _logger.Information("Starting networking...");
        _networking.Start();

        try
        {
            while (true)
            {
                _networking.Update();
            }
        }
        catch (Exception exception)
        {
            _logger.Critical($"Networking: Unhandled exception: {exception.Message}");
            throw;
        }
        finally
        {
            _networking.Stop();
        }
    }

    private void SimulationLoop()
    {
        _logger.Information("Starting simulation...");
        var simulation = new Game();

        var broadcastThread = new Thread(BroadcastLoop);
        broadcastThread.Name = "Broadcast";
        broadcastThread.Start(simulation);

        try
        {
            while (true)
            {
                simulation.Update();
            }
        }
        catch (Exception exception)
        {
            _logger.Critical($"Simulation: Unhandled exception: {exception.Message}");
            throw;
        }
        finally
        {
            broadcastThread.Join();
        }
    }

    private void BroadcastLoop(object? context)
    {
        var simulation = (Game) context!;

        while (true)
        {
            using var result = simulation.Query(new CharacterPositionsQuery());

            var message = new BroadcastMessage();

            foreach (var position in result.Positions)
            {
                var vector2 = new BroadcastMessage.Types.Vector2();
                vector2.X = position.X;
                vector2.Y = position.Y;
                message.Positions.Add(vector2);
            }

            _networking.Broadcast(message, DeliveryMethod.Unreliable);

            Thread.Sleep(1000);
        }

        // ...
    }
}
