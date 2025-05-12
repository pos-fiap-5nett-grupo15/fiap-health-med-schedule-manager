namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;

public interface IConsumerSettings
{
    public ApplicationQueues? Queues { get; }
    public string Host { get; }
    public string Username { get; }
    public string Password { get; }
    int Port { get; }
}