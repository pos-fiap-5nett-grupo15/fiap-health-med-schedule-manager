namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;

public class ProducerSettings : CommonSettings,IProducerSettings
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
}