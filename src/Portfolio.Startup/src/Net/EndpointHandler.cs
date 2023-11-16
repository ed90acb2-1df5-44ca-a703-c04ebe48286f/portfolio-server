using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Server.Controllers;
using Portfolio.Server.Net;

namespace Portfolio.Startup.Net;

public class EndpointHandler : IEndpointHandler
{
    private readonly IServiceProvider _serviceProvider;

    public EndpointHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Handle<TRequest, TController>(Connection connection, TRequest request)
        where TController : IController<TRequest>
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<TController>().Handle(connection, request);
    }
}
