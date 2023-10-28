using System;

namespace Portfolio.Gameplay;

public interface ILogger
{
    void Debug(string message);

    void Information(string message);

    void Error(string message);

    void Exception(Exception exception);
}
