using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Portfolio.Application;
using Portfolio.Application.Controllers;
using Portfolio.Application.Net;
using Portfolio.Application.Security;
using Portfolio.Protocol.Commands;
using Portfolio.Startup.Extensions;
using Portfolio.Startup.Logging;
using Portfolio.Startup.Net;
using Portfolio.Startup.Security;
using ILogger = Portfolio.Application.ILogger;

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
            .AddSingleton<Server>()
            .AddSingleton<INetworking, LiteNetLibNetworking>()
            .AddSingleton<IPasswordHasher, BcryptPasswordHasher>()
            .AddSingleton<SessionStorage>()
            .AddSingleton<ILogger>(services => new GameplayLogger(services.GetRequiredService<ILoggerFactory>().CreateLogger("Server")))
            .AddScoped<Authentication>()
            .AddCommandHandlers()
            .AddOptions(configuration)
            .AddRepositories(configuration)
            .AddMigrations(configuration)
            .AddSerilog(configuration)
            .BuildServiceProvider();

        RegisterControllers(services);
        RunMigrations(services);

        services.GetRequiredService<Server>().Start();
    }

    private static void RunMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static void RegisterControllers(IServiceProvider services)
    {
        var networking = services.GetRequiredService<INetworking>();
        networking.RegisterController<LoginCommand, LoginController>();
    }
}
