using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Protocol.Commands;
using Portfolio.Server.CommandHandlers;
using Portfolio.Server.Extensions;
using Portfolio.Server.Net;
using Portfolio.Server.Security;

namespace Portfolio.Server;

public static class Program
{
    public static void Main()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<NetworkDispatcher>()
            .AddSingleton<SessionStorage>()
            .AddSingleton<Application>()
            .AddScoped<Authentication>()
            .AddCommandHandlers()
            .AddOptions(configuration)
            .AddRepositories(configuration)
            .AddMigrations(configuration)
            .AddSerilog(configuration)
            .BuildServiceProvider();

        RegisterCommandHandlers(services);
        RunMigrations(services);

        services.GetRequiredService<Application>().Start();
    }

    private static void RunMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static void RegisterCommandHandlers(IServiceProvider services)
    {
        var networking = services.GetRequiredService<NetworkDispatcher>();

        networking.RegisterHandler(CreateCommandHandlerDelegate<LoginCommand, LoginCommandHandler>(false));

        return;

        Action<Player, TCommand> CreateCommandHandlerDelegate<TCommand, THandler>(bool isAuthenticationRequired = false) where THandler : ICommandHandler<TCommand>
        {
            return async (player, command) =>
            {
                try
                {
                    if (isAuthenticationRequired && !services.GetRequiredService<Authentication>().IsAuthenticated(player))
                    {
                        return;
                    }

                    await using var scope = services.CreateAsyncScope();
                    await scope.ServiceProvider.GetRequiredService<THandler>().Handle(player, command);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            };
        }
    }
}
