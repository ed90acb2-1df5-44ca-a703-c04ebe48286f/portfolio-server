using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Server;
using Portfolio.Server.Controllers;
using Portfolio.Server.Net;

namespace Portfolio.Startup.Net;

public class CommandHandler : ICommandHandler
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public CommandHandler(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Handle<TCommand, TController>(Connection connection, TCommand command)
        where TController : IController<TCommand>
    {
        _logger.Debug($"{GetType().Name}: {typeof(TController).Name}.Handle({typeof(TCommand).Name}) ConnectionId: {connection.Id} ThreadId: {Environment.CurrentManagedThreadId.ToString()}");

        await using var scope = _serviceProvider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<TController>().Handle(connection, command);
    }
}
