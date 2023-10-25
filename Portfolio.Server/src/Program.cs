using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Server;
using Portfolio.Server.Extensions;
using Portfolio.Server.Services;

await Host.CreateDefaultBuilder(args)
    .ConfigureHostOptions(options =>
    {
        options.ServicesStartConcurrently = true;
        options.ServicesStopConcurrently = false;
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddHostedService<GameService>()
            .AddSingleton<NetworkingService>()
            .AddScoped<AuthenticationService>()
            .AddScoped<SessionService>()
            .AddCommandHandlers()
            .AddOptions(context)
            .AddRepositories(context)
            .AddMigrations(context)
            .AddSerilog(context);
    })
    .Build()
    .RunMigrations()
    .RegisterCommandHandlers()
    .RunAsync();
