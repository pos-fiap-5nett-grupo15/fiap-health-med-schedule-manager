using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.UpdateSchedule;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public interface IScheduleService
{
    Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task CreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken);
    Task<Result> RefuseScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken);
}