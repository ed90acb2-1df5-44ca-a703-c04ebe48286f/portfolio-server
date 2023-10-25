using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Protocol.Commands;
using Portfolio.Server.Handlers;

namespace Portfolio.Server.Extensions;

public static class HostExtensions
{
    public static IHost RunMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        return host;
    }

    public static IHost RegisterCommandHandlers(this IHost host)
    {
        ThreadPool.GetMaxThreads(out var workerThreads, out var completionPortThreads);

        Console.WriteLine($"{workerThreads}, {completionPortThreads}");

        var networking = host.Services.GetRequiredService<NetworkingService>();
        networking.RegisterCommandHandler<LoginCommand, LoginCommandHandler>();
        return host;
    }
}
