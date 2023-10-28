using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolio.Gameplay;
using Portfolio.Protocol.Commands;
using Portfolio.Server.CommandHandlers;
using Portfolio.Server.Logging;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly GameplayLogger _gameplayLogger;
    private readonly NetworkDispatcher _networking;
    private readonly Thread _networkingThread;
    private readonly Thread _simulationThread;

    public Application(ILogger<Application> logger, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, NetworkDispatcher networking)
    {
        _logger = logger;
        _networking = networking;

        _gameplayLogger = new GameplayLogger(loggerFactory.CreateLogger("Gameplay"));

        _networkingThread = new Thread(NetworkingLoop);
        _networkingThread.Name = "Networking";

        _simulationThread = new Thread(SimulationLoop);
        _simulationThread.Name = "Simulation";

        RegisterCommandHandlers(serviceProvider);
    }

    public void Start()
    {
        _networkingThread.Start();
        _simulationThread.Start();
    }

    private void NetworkingLoop()
    {
        _logger.LogInformation("Starting networking...");
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
            _logger.LogCritical($"Networking: Unhandled exception: {exception.Message}");
            throw;
        }
        finally
        {
            _networking.Stop();
        }
    }

    private void SimulationLoop()
    {
        _logger.LogInformation("Starting simulation...");
        var simulation = new Simulation(_gameplayLogger);

        try
        {
            while (true)
            {
                simulation.Update();
            }
        }
        catch (Exception exception)
        {
            _logger.LogCritical($"Simulation: Unhandled exception: {exception.Message}");
            throw;
        }
    }

    private static void RegisterCommandHandlers(IServiceProvider services)
    {
        var networking = services.GetRequiredService<NetworkDispatcher>();

        networking.RegisterHandler(CreateCommandHandlerDelegate<LoginCommand, LoginCommandHandler>(false));

        return;

        Func<Player, TCommand, Task> CreateCommandHandlerDelegate<TCommand, THandler>(bool isAuthenticationRequired = false) where THandler : ICommandHandler<TCommand>
        {
            return async (player, command) =>
            {
                try
                {
                    if (isAuthenticationRequired && !services.GetRequiredService<Authentication>().IsAuthenticated(player))
                    {
                        return;
                    }

                    await using var scope = services.CreateAsyncScope();
                    await scope.ServiceProvider.GetRequiredService<THandler>().Handle(player, command);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            };
        }
    }
}
