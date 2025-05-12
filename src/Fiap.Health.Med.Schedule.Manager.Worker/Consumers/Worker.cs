using System.Text;
using System.Text.Json;
using Fiap.Health.Med.Schedule.Manager.Application.Services;
using Fiap.Health.Med.Schedule.Manager.Application.Services.QueueMessages;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fiap.Health.Med.Schedule.Manager.Worker.Producers;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ApplicationQueues _queues;
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
        this._queues = settings.Queues ?? throw new ArgumentNullException("queue settings");
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RegisterConsumerAsync(_queues.CreateSchedule, HandleCreateScheduleAsync);
        RegisterConsumerAsync(_queues.RequestSchedule, HandlePatientRequestScheduleAsync);
        RegisterConsumerAsync(_queues.PatientCancelSchedule, HandlePatientCancelScheduleAsync);

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

    private void RegisterConsumerAsync(string? queueName, Func<string, CancellationToken, Task> handler)
    {
        if (string.IsNullOrEmpty(queueName))
        {
            _logger.LogError("Queue name is null or empty.");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(this._channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            try
            {
                await handler(message, eventArgs.CancellationToken);
                await this._channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the message. queue:{queueName}");
            }
        };

        this._channel.BasicConsumeAsync(queueName, false, consumer);
    }
    private async Task HandleCreateScheduleAsync(string message, CancellationToken ct)
    {
        try
        {
            using (var scope = this._serviceProvider.CreateScope())
            {
                var requestMessage = JsonSerializer.Deserialize<CreateScheduleMessage>(message);
                var service = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                await service.HandleCreateAsync(requestMessage, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to handle create schedule");
        }
    }
    private async Task HandlePatientRequestScheduleAsync(string message, CancellationToken ct)
    {
        try
        {
            using (var scope = this._serviceProvider.CreateScope())
            {
                var requestMessage = JsonSerializer.Deserialize<RequestPatientScheduleMessage>(message);
                var service = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                await service.HandlePatientRequesSchedule(requestMessage, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to handle patient request schedule");
        }
    }
    private async Task HandlePatientCancelScheduleAsync(string message, CancellationToken ct)
    {
        try
        {
            using (var scope = this._serviceProvider.CreateScope())
            {
                var requestMessage = JsonSerializer.Deserialize<PatientCancelScheduleMessage>(message);
                var service = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                await service.HandleCancelScheduleRequest(requestMessage, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to handle cancel schedule");
        }
    }

    public override void Dispose()
    {
        this._channel.Dispose();
        this._connection.Dispose();
        base.Dispose();
    }
}