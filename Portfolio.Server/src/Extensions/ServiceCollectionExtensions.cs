using System.Data;
using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Portfolio.Server.Handlers;
using Portfolio.Server.Repositories;
using Portfolio.Server.Repositories.Dapper;
using Serilog;

namespace Portfolio.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type is {IsClass: true, IsAbstract: false} && type.GetInterfaces().Any(x => x == typeof(IHandler)))
            {
                services.AddScoped(type);
            }
        }

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<ServerSettings>(context.Configuration.GetSection("Server"));

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services, HostBuilderContext context)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(context.Configuration.GetConnectionString("Postgres"))
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, HostBuilderContext context)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .CreateLogger();

        services.AddSerilog();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, HostBuilderContext context)
    {
        services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(context.Configuration.GetConnectionString("Postgres")));
        services.AddScoped<IUserRepository, DapperUserRepository>();

        return services;
    }
}
