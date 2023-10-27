using Microsoft.Extensions.Logging;
using Portfolio.Server.Services;

namespace Portfolio.Server;

public class GameLoop
{
    private readonly ILogger<GameLoop> _logger;
    private readonly ServerNetworkingService _networking;

    public GameLoop(ILogger<GameLoop> logger, ServerNetworkingService networking)
    {
        _logger = logger;
        _networking = networking;
    }

    public async Task Start()
    {
        _logger.LogInformation("Starting game loop...");

        try
        {
            _networking.Start();

            while (true)
            {
                _networking.Update();
                await Task.Delay(1);
            }
        }
        catch (Exception exception)
        {
            _logger.LogCritical($"Unhandled exception: {exception.Message}");
        }
        finally
        {
            _networking.Stop();
        }
    }
}
