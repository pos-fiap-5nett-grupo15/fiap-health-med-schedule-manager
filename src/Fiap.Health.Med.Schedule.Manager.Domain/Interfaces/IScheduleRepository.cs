using Fiap.Health.Med.Schedule.Manager.Domain.Enum;

namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<bool> CreateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetSchedulesByPatientIdAsync(int patientId, CancellationToken cancellationToken);
    Task<(Models.Schedule?, string)> GetScheduleByIdAndDoctorIdAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<(bool, string)> UpdatescheduleStatusAsync(long scheduleId, EScheduleStatus newStatus, CancellationToken ct);
    Task<int> CreatePendingScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<Models.Schedule> GetScheduleByIdAsync(long scheduleId, CancellationToken ct);
    Task<(bool, string)> DeleteScheduleStatusAsync(long scheduleId, CancellationToken ct);
    Task<int> UpdateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<int> ScheduleToPatientAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<int> CancelScheduleAsync(Models.Schedule schedule, CancellationToken ct);
}