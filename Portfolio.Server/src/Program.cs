using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portfolio.Server;
using Serilog;

await Host.CreateDefaultBuilder(args)
    .ConfigureHostOptions(options =>
    {
        options.ServicesStartConcurrently = true;
        options.ServicesStopConcurrently = false;
    })
    .ConfigureServices((context, services) =>
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .CreateLogger();

        services.AddHostedService<GameLoop>();
        services.AddSingleton<ServerNetworkingService>();
        services.Configure<ServerSettings>(context.Configuration.GetSection("Server"));
    })
    .UseSerilog()
    .Build()
    .RunAsync();
