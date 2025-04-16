namespace Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;

public class ConsumerSettings : CommonSettings, IConsumerSettings
{
    public string Queue { get; set; }
}