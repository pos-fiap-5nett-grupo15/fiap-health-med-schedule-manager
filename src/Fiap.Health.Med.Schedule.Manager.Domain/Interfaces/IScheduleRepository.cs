using Fiap.Health.Med.Schedule.Manager.Domain.Enum;

namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<bool> CreateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken);
    Task<(Models.Schedule?, string)> GetScheduleByIdAndDoctorIdAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<(bool, string)> UpdatescheduleStatusAsync(long scheduleId, EScheduleStatus newStatus, CancellationToken ct);
    Task<Models.Schedule?> GetScheduleByIdAsync(long scheduleId, CancellationToken cancellationToken);
    Task<int> UpdateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
}