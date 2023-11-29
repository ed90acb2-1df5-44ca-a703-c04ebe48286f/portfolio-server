using System;
using Microsoft.Extensions.Logging;

namespace Portfolio.Startup.Logging;

public class GameplayLogger : Server.ILogger
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

    public void Warning(string message)
    {
        _logger.LogWarning(message);
    }

    public void Error(string message)
    {
        _logger.LogError(message);
    }

    public void Critical(string message)
    {
        _logger.LogCritical(message);
    }

    public void Exception(Exception exception)
    {
        _logger.LogCritical($"{exception.Message}\n{exception.StackTrace}");
    }
}
