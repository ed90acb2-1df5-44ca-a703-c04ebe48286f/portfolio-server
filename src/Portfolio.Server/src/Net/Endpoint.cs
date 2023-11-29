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

public class Endpoint<TRequest, TController> : Endpoint
    where TController : IController<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IEndpointHandler _endpointHandler;

    public Endpoint(ILogger logger, IEndpointHandler endpointHandler)
    {
        _logger = logger;
        _endpointHandler = endpointHandler;
    }

    public async Task Handle(Connection connection, TRequest request)
    {
        for (var i = 0; i < Filters.Count; i++)
        {
            if (!Filters[i].Filter(connection, request))
            {
                _logger.Warning($"Filter not passed: '{Filters[i].GetType().Name}'");
                return;
            }
        }

        try
        {
            await _endpointHandler.Handle<TRequest, TController>(connection, request);
        }
        catch (Exception exception)
        {
            _logger.Exception(exception);
        }
    }
}
