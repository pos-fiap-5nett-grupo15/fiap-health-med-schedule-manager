using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.CreateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient;
using Fiap.Health.Med.Schedule.Manager.Application.Services.QueueMessages;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public interface IScheduleService
{
    Task<Result<Domain.Models.Schedule>> GetByScheduleId(long scheduleId, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByPatientId(int patientId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByDoctorId(int doctorId, CancellationToken cancellationToken);
    Task<Result> DeclineScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<Result<int>> RequestCreateScheduleAsync(CreateScheduleRequest createScheduleRequest, CancellationToken cancellationToken);
    Task<Result> AcceptScheduleAsync(long scheduleId, int doctorId, CancellationToken ct);
    Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken);
    Task<Result> RequestScheduleToPatientAsync(PatientScheduleRequestDto scheduleData, CancellationToken ct);
    Task<Result> RequestCancelScheduleAsync(CancelScheduleRequestDto cancelData, CancellationToken ct);
    Task HandleCreateAsync(CreateScheduleMessage? deserialize, CancellationToken cancellationToken);
    Task<bool> HandlePatientRequesSchedule(RequestPatientScheduleMessage? requestMessage, CancellationToken cancellationToken);
    Task<bool> HandleCancelScheduleRequest(PatientCancelScheduleMessage? requestMessage, CancellationToken cancellationToken);
}