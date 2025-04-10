using System.Runtime.CompilerServices;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using Fiap.Health.Med.Schedule.Manager.Worker.Producers;
using Newtonsoft.Json;
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
        
        builder.Services.AddSingleton(new RabbitMqConnector(consumerSettings));
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