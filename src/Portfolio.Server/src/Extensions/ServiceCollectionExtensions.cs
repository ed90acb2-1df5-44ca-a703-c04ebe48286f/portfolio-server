using System.Data;
using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Portfolio.Server.CommandHandlers;
using Portfolio.Server.Repositories;
using Portfolio.Server.Repositories.Dapper;
using Portfolio.Server.Settings;
using Serilog;

namespace Portfolio.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type is {IsClass: true, IsAbstract: false} && type.GetInterfaces().Any(x => x == typeof(ICommandHandler)))
            {
                services.AddScoped(type);
            }
        }

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServerSettings>(configuration.GetSection("Server"));

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("Postgres"))
                .ScanIn(typeof(Program).Assembly).For.Migrations());

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
