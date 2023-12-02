using Portfolio.Gameplay;
using Portfolio.Server.Net;
using Portfolio.Server.Security;
using Portfolio.Server.Threads;

namespace Portfolio.Server;

public class Server
{
    private readonly ILogger _logger;

    private readonly GameplayThread _gameplayThread;
    private readonly NetworkKernelThread _networkKernelThread;
    private readonly BroadcastMessagesThread _broadcastMessagesThread;
    private readonly BroadcastGameStateThread _broadcastGameStateThread;

    public Server(ILogger logger, Game game, Router router, INetworkKernel networkKernel, Authentication authentication)
    {
        _logger = logger;
        _gameplayThread = new GameplayThread(game, logger);
        _networkKernelThread = new NetworkKernelThread(networkKernel, router, authentication, logger);
        _broadcastMessagesThread = new BroadcastMessagesThread(game, networkKernel, logger);
        _broadcastGameStateThread = new BroadcastGameStateThread(game, networkKernel, logger);
    }

    public void Start()
    {
        _logger.Information("Starting Server...");

        _networkKernelThread.Start();
        _gameplayThread.Start();
        _broadcastMessagesThread.Start();
        _broadcastGameStateThread.Start();
    }
}
