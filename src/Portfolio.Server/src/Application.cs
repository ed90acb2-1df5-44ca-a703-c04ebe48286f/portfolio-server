using Microsoft.Extensions.Logging;
using Portfolio.Gameplay;
using Portfolio.Server.Logging;
using Portfolio.Server.Net;

namespace Portfolio.Server;

public class Application
{
    private readonly ILogger<Application> _logger;
    private readonly GameplayLogger _gameplayLogger;
    private readonly NetworkDispatcher _networking;
    private readonly Thread _networkingThread;
    private readonly Thread _simulationThread;

    public Application(ILogger<Application> logger, ILoggerFactory loggerFactory, NetworkDispatcher networking)
    {
        _logger = logger;
        _networking = networking;

        _gameplayLogger = new GameplayLogger(loggerFactory.CreateLogger("Gameplay"));

        _networkingThread = new Thread(NetworkingLoop);
        _networkingThread.Name = "Networking";

        _simulationThread = new Thread(SimulationLoop);
        _simulationThread.Name = "Simulation";
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
}
