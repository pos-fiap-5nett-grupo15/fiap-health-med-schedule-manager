using Fiap.Health.Med.Schedule.Manager.Application.Common;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public interface IScheduleService
{
    Task<IEnumerable<Domain.Models.Schedule>>GetAsync(CancellationToken cancellationToken);
    Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken);
    Task<Result> RefuseScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<Result> AcceptScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
}