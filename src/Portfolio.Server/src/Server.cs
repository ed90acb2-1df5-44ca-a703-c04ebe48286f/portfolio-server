using System.Threading;
using Portfolio.Protocol.Messages;
using Portfolio.Gameplay;
using Portfolio.Gameplay.Queries;
using Portfolio.Protocol.Authentication;
using Portfolio.Protocol.Requests;
using Portfolio.Protocol.Values;
using Portfolio.Server.Controllers;
using Portfolio.Server.Net;

namespace Portfolio.Server;

public class Server
{
    private readonly ILogger _logger;
    private readonly INetworking _networking;
    private readonly Thread _networkingThread;
    private readonly Thread _gameThread;

    public Server(ILogger logger, INetworking networking)
    {
        _logger = logger;
        _networking = networking;

        _networkingThread = new Thread(NetworkingLoop);
        _networkingThread.Name = "Networking";

        _gameThread = new Thread(GameLoop);
        _gameThread.Name = "Simulation";

        networking.RegisterController<LoginRequest, LoginController>();
        networking.RegisterController<RegistrationRequest, RegistrationController>();
        networking.RegisterController<InputRequest, InputController>();
    }

    public void Start()
    {
        _networkingThread.Start();
        _gameThread.Start();
    }

    private void NetworkingLoop()
    {
        _logger.Information("Starting networking...");
        _networking.Start();

        while (true)
        {
            _networking.Update();
        }
    }

    private void GameLoop()
    {
        _logger.Information("Starting simulation...");
        var game = new Game();

        var broadcastThread = new Thread(BroadcastLoop);
        broadcastThread.Name = "Broadcast";
        broadcastThread.Start(game);

        while (true)
        {
            game.Update();
        }
    }

    private void BroadcastLoop(object? context)
    {
        var game = (Game) context!;

        while (true)
        {
            using var result = game.Query(new CharacterPositionsQuery());

            var message = new BroadcastMessage();

            foreach (var position in result.Positions)
            {
                var vector2 = new Vector2();
                vector2.X = position.X;
                vector2.Y = position.Y;
                message.Positions.Add(vector2);
            }

            _networking.Broadcast(message, DeliveryMethod.Unreliable);

            Thread.Sleep(1000 / 5);
        }

        // ...
    }
}
