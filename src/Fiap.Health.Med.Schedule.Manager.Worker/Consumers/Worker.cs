using System.Text;
using System.Text.Json;
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fiap.Health.Med.Schedule.Manager.Worker.Producers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _queueName;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public Worker(
        RabbitMqConnector connector,
        ConsumerSettings settings,
        IServiceProvider serviceProvider,
        ILogger<Worker> logger)
    {
        this._connection = connector.GetConnection().Result;
        
        this._channel = this._connection.CreateChannelAsync().Result;
        
        this._serviceProvider = serviceProvider;
        this._queueName = settings.Queue;
        this._logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(this._channel);
        consumer.ReceivedAsync += async (_, eventArgs) => 
        {
            var body = eventArgs.Body.ToArray();

            if (Encoding.UTF8.GetString(body) is var message)
            {
                Console.WriteLine($"Received message: {message}");

                using (var scope = this._serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                    var model = JsonSerializer.Deserialize<CreateScheduleMessage>(message,new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await service.HandleCreateAsync(model, stoppingToken);
                }
            }

            await this._channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
        };
        await this._channel.BasicConsumeAsync(this._queueName, false, consumer, cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(3000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the background service.");
                throw;
            }
        }
    }

    public override void Dispose()
    {
        this._channel.Dispose();
        this._connection.Dispose();
        base.Dispose();
    }
}