using System;

namespace Portfolio.Application;

public interface ILogger
{
    void Debug(string message);

    void Information(string message);

    void Error(string message);

    void Critical(string message);

    void Exception(Exception exception);
}
