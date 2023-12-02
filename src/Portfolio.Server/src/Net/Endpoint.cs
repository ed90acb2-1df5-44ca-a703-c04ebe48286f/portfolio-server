using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portfolio.Server.Controllers;
using Portfolio.Server.Filters;

namespace Portfolio.Server.Net;

public class Endpoint
{
    protected readonly List<IConnectionFilter> Filters = new();

    public Endpoint Filter(IConnectionFilter filter)
    {
        Filters.Add(filter);
        return this;
    }
}

public class Endpoint<TCommand, TController> : Endpoint
    where TController : IController<TCommand>
    where TCommand : notnull
{
    private readonly ILogger _logger;
    private readonly ICommandHandler _commandHandler;

    public Endpoint(ILogger logger, ICommandHandler commandHandler)
    {
        _logger = logger;
        _commandHandler = commandHandler;
    }

    public async Task Handle(Connection connection, TCommand command)
    {
        for (var i = 0; i < Filters.Count; i++)
        {
            if (!Filters[i].Filter(connection, command))
            {
                _logger.Warning($"Filter not passed: '{Filters[i].GetType().Name}'");
                return;
            }
        }

        try
        {
            await _commandHandler.Handle<TCommand, TController>(connection, command);
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
