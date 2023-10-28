using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        RunMigrations(services);

        services.GetRequiredService<Application>().Start();
    }

    private static void RunMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
