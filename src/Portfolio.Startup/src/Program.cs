using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolio.Gameplay;
using Portfolio.Server.Net;
using Portfolio.Server.Security;
using Portfolio.Startup.Extensions;
using Portfolio.Startup.Logging;
using Portfolio.Startup.Net;
using Portfolio.Startup.Security;
using ILogger = Portfolio.Server.ILogger;

namespace Portfolio.Startup;

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
            .AddSingleton<Server.Server>()
            .AddSingleton<Game>()
            .AddSingleton<Router>()
            .AddSingleton<LiteNetLibNetworkKernel>()
            .AddSingleton<INetworkKernel>(services => services.GetRequiredService<LiteNetLibNetworkKernel>())
            .AddSingleton<INetwork>(services => services.GetRequiredService<LiteNetLibNetworkKernel>())
            .AddSingleton<IPasswordHasher, BcryptPasswordHasher>()
            .AddSingleton<ICommandHandler, CommandHandler>()
            .AddSingleton<IPacketSerializer, PacketSerializer>()
            .AddSingleton<ISessionStorage, InMemorySessionStorage>()
            .AddSingleton<ILogger>(services => new GameplayLogger(services.GetRequiredService<ILoggerFactory>().CreateLogger("Server")))
            .AddScoped<Authentication>()
            .AddCommandHandlers()
            .AddOptions(configuration)
            .AddRepositories(configuration)
            .AddMigrations(configuration)
            .AddSerilog(configuration)
            .BuildServiceProvider();

        RunMigrations(services);

        services.GetRequiredService<Server.Server>().Start();
    }

    private static void RunMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
