

namespace Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;

public interface IScheduleRepository
{
    Task CreateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken);
    Task<Models.Schedule?> GetScheduleByIdAsync(long scheduleId, CancellationToken cancellationToken);
    Task<int> UpdateScheduleAsync(Models.Schedule schedule, CancellationToken cancellationToken);
}