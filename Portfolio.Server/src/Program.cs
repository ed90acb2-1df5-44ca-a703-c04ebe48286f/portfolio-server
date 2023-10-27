using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Protocol.Commands;
using Portfolio.Server.Extensions;
using Portfolio.Server.Handlers;
using Portfolio.Server.Services;

namespace Portfolio.Server;

public static class Program
{
    public static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<ServerNetworkingService>()
            .AddSingleton<GameLoop>()
            .AddScoped<AuthenticationService>()
            .AddScoped<SessionService>()
            .AddCommandHandlers()
            .AddOptions(configuration)
            .AddRepositories(configuration)
            .AddMigrations(configuration)
            .AddSerilog(configuration)
            .BuildServiceProvider();

        RegisterCommandHandlers(services);
        RunMigrations(services);

        await services.GetRequiredService<GameLoop>().Start();
    }

    private static void RunMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static void RegisterCommandHandlers(IServiceProvider services)
    {
        var networking = services.GetRequiredService<ServerNetworkingService>();

        networking.RegisterHandler(CreateCommandHandlerDelegate<LoginCommand, LoginCommandHandler>());

        return;

        Action<int, TCommand> CreateCommandHandlerDelegate<TCommand, THandler>() where THandler : ICommandHandler<TCommand>
        {
            return async (peerId, command) =>
            {
                await using var scope = services.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<THandler>().Handle(peerId, command);
            };
        }
    }
}
