namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;

public interface IProducerSettings
{
    public string Host { get; }
    public int Port { get; }
    public string Exchange { get; }
    public string RoutingKey { get; }
    public string Username { get; }
    public string Password { get; }
}