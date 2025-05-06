using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.UnitOfWork;
using RabbitMQ.Client;

namespace Fiap.Health.Med.Schedule.Manager.Worker;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var consumerSettings = new ConsumerSettings();
        builder.Configuration.GetSection("ConsumerSettings").Bind(consumerSettings);

        builder.Services.AddSingleton(consumerSettings);


        var producerSettings = new ProducerSettings();
        builder.Configuration.GetSection("ProducerSettings").Bind(producerSettings);

        builder.Services.AddSingleton(producerSettings);
        builder.Services.AddSingleton<IProducerSettings>(producerSettings);

        builder.Services.AddSingleton(new RabbitMqConnector(consumerSettings));

        builder.Services.AddScoped<IHealthDatabase, HealthDatabase>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


        builder.Services.AddScoped<IScheduleService, ScheduleService>();
        builder.Services.AddHostedService<Producers.Worker>();

        var host = builder.Build();
        host.Run();
    }
}

public class RabbitMqConnector
{
    public RabbitMqConnector(ConsumerSettings consumerSettings)
    {
        this._factory = new ConnectionFactory
        {
            HostName = consumerSettings.Host,
            Port = consumerSettings.Port,
            UserName = consumerSettings.Username,
            Password = consumerSettings.Password
        };
    }

    private readonly ConnectionFactory _factory;

    public async Task<IConnection> GetConnection(CancellationToken cancellationToken = default)
    {
        return await this._factory.CreateConnectionAsync(cancellationToken);
    }
}