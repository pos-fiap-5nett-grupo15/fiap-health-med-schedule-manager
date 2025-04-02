namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public interface IScheduleService
{
    Task<IEnumerable<Domain.Models.Schedule>>GetAsync(CancellationToken cancellationToken);
    Task CreateSchedule(Domain.Models.Schedule schedule, CancellationToken cancellationToken);
}