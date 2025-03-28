namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<IEnumerable<Models.Schedule>> GetAsync(CancellationToken cancellationToken);
}