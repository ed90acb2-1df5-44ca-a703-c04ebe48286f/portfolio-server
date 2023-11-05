using System;
using System.Data;
using System.Linq;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Portfolio.Server.Controllers;
using Portfolio.Server.Repositories;
using Portfolio.Startup.Repositories.Dapper;
using Portfolio.Startup.Settings;
using Serilog;

namespace Portfolio.Startup.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()))
        {
            var controllerType = typeof(IController<>);

            if (type is {IsClass: true, IsAbstract: false} &&
                type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == controllerType))
            {
                services.AddScoped(type);
            }
        }

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NetworkingSettings>(configuration.GetSection("Networking"));

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("Postgres"))
                .ScanIn(typeof(ServiceCollectionExtensions).Assembly).For.Migrations());

        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(builder =>
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            builder.AddSerilog();
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));
        services.AddScoped<IUserRepository, DapperUserRepository>();

        return services;
    }
}
