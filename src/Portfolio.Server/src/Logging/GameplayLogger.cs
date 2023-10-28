using Microsoft.Extensions.Logging;

namespace Portfolio.Server.Logging;

public class GameplayLogger : Portfolio.Gameplay.ILogger
{
    private readonly ILogger _logger;

    public GameplayLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void Debug(string message)
    {
        _logger.LogDebug(message);
    }

    public void Information(string message)
    {
        _logger.LogInformation(message);
    }

    public void Error(string message)
    {
        _logger.LogError(message);
    }

    public void Exception(Exception exception)
    {
        _logger.LogCritical(exception.Message);
    }
}
