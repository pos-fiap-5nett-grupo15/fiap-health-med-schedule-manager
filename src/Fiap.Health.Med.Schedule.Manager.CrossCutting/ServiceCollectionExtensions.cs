using Fiap.Health.Med.Schedule.Manager.Infrastructure.Migrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.Health.Med.Schedule.Manager.CrossCutting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        return services;
    }
    
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }
    public static IServiceCollection Migrations(this IServiceCollection services, IConfiguration configuration)
    {
        using (var serviceProvider = BuildFluentMigrationServiceProvider(services, configuration))
        using (var scope = serviceProvider.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        return services;
    }
    private static ServiceProvider BuildFluentMigrationServiceProvider(IServiceCollection sc, IConfiguration configuration)
    {
        var strConnection = configuration.GetConnectionString("DatabaseDllConnection");
        if (string.IsNullOrEmpty(strConnection))
            throw new InvalidOperationException("DatabaseDllConnection is not defined.");
            
        return  new ServiceCollection().AddFluentMigratorCore()
            .ConfigureRunner( rb => 
                rb.AddSqlServer()
                    .WithGlobalConnectionString(strConnection)
                    .ScanIn(typeof(CreateDoctorsTable).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}