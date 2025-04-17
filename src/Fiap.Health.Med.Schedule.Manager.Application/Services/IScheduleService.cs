using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public interface IScheduleService
{
    Task<Result<Domain.Models.Schedule>> GetByScheduleId(long scheduleId, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByPatientId(int patientId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByDoctorId(int doctorId, CancellationToken cancellationToken);
    Task<Result> DeclineScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task HandleCreateAsync(CreateScheduleMessage? deserialize, CancellationToken cancellationToken);
    Task<Result<int>> RequestCreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken);
    Task<Result> AcceptScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken);
    Task<Result> ScheduleToPatientAsync(PatientScheduleRequestDto scheduleData, CancellationToken ct);
    Task<Result> CancelScheduleAsync(CancelScheduleRequestDto cancelData, CancellationToken ct);
}