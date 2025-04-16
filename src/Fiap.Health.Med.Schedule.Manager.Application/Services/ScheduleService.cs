using System.Net;
using System.Text;
using Fiap.Health.Med.Schedule.Manager.Application.Common;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Doctor.UpdateSchedule;
using Fiap.Health.Med.Schedule.Manager.Application.DTOs.Patient;
using Fiap.Health.Med.Schedule.Manager.Domain.Enum;
using Fiap.Health.Med.Schedule.Manager.Domain.Interfaces;
using Fiap.Health.Med.Schedule.Manager.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Fiap.Health.Med.Schedule.Manager.Application.Services;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScheduleService> _logger;
    private readonly IProducerSettings _producer;

    public ScheduleService(
        IUnitOfWork unitOfWork,
        IProducerSettings producer,
        ILogger<ScheduleService> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._producer = producer;
    }

    public async Task<IEnumerable<Domain.Models.Schedule>> GetAsync(CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetAsync(cancellationToken);
    }

    public async Task HandleCreateAsync(CreateScheduleMessage? deserialize, CancellationToken cancellationToken)
    {
        if (deserialize == null) throw new NullReferenceException();
        var schedule = await this._unitOfWork.ScheduleRepository.GetScheduleByIdAsync(deserialize.Id, cancellationToken);

        if (IsPassedTime(schedule, DateTime.Now))
            throw new InvalidOperationException();

        var dbModels
            = (await this.GetScheduleByAsync(schedule.DoctorId, schedule.ScheduleTime, cancellationToken)).Where(x => x.Status != EScheduleStatus.UNDEFINED);

        var hasAnyOverlap = dbModels.Select(x => schedule.IsOverlappedBy(x)).Any(x => x == true);

        var newStatus = hasAnyOverlap ? EScheduleStatus.REFUSED : EScheduleStatus.CONFIRMED;

        await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(schedule.Id, newStatus, cancellationToken);
    }

    public async Task<Result<int>> RequestCreateScheduleAsync(Domain.Models.Schedule schedule, CancellationToken cancellationToken)
    {
        try
        {
            schedule.Status = EScheduleStatus.UNDEFINED;
            var id = await this._unitOfWork.ScheduleRepository.CreatePendingScheduleAsync(schedule, cancellationToken);
            await PublishScheduleAsync(new CreateScheduleMessage(id), this._producer, cancellationToken);
            return Result<int>.Success(HttpStatusCode.Created, id);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "An error occurring while creating schedule.");
            throw;
        }

    }


    public static async Task PublishScheduleAsync<T>(T message, IProducerSettings settings,
        CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password
        };

        using (IConnection connection = await factory.CreateConnectionAsync(cancellationToken))
        using (IChannel channel = await connection.CreateChannelAsync(null, cancellationToken))
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await channel.BasicPublishAsync(settings.Exchange, settings.RoutingKey, new ReadOnlyMemory<byte>(body), cancellationToken);
        }
    }

    public async Task<Result> RefuseScheduleAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        (var schedule, var getScheduleError) = await this._unitOfWork.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, ct);
        if (!string.IsNullOrWhiteSpace(getScheduleError))
            return Result.Fail(HttpStatusCode.UnprocessableContent, getScheduleError);

        if (schedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado.");

        if (await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, ct) is (var success, var updateError) && !success)
            return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateError) ? updateError : "Um erro ocorreu ao atualizar status do Agendamento.");

        return Result.Success(HttpStatusCode.NoContent);
    }

    public async Task<Result> AcceptScheduleAsync(long scheduleId, int doctorId, CancellationToken ct)
    {
        (var schedule, var getScheduleError) = await this._unitOfWork.ScheduleRepository.GetScheduleByIdAndDoctorIdAsync(scheduleId, doctorId, ct);
        if (!string.IsNullOrWhiteSpace(getScheduleError))
            return Result.Fail(HttpStatusCode.UnprocessableContent, getScheduleError);

        if (schedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado.");

        if (schedule.Status == EScheduleStatus.PENDING_CONFIRMATION)
        {
            if (schedule.ScheduleTime < DateTime.Now)
            {
                if (await this._unitOfWork.ScheduleRepository.DeleteScheduleStatusAsync(scheduleId, ct) is (var successDelete, var updateDeleteError) && !successDelete)
                    return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateDeleteError) ? updateDeleteError : "Um erro ocorreu ao excluir o Agendamento.");
            }
            else
            {
                if (await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.CONFIRMED, ct) is (var successConfirmed, var updateConfirmedError) && !successConfirmed)
                    return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateConfirmedError) ? updateConfirmedError : "Um erro ocorreu ao atualizar status para confirmar o Agendamento.");
            }

        }
        else if (schedule.Status == EScheduleStatus.CANCELED_BY_DOCTOR)
        {
            if (await this._unitOfWork.ScheduleRepository.UpdatescheduleStatusAsync(scheduleId, EScheduleStatus.REFUSED, ct) is (var successRefused, var updateRefusedError) && !successRefused)
                return Result.Fail(HttpStatusCode.UnprocessableContent, !string.IsNullOrWhiteSpace(updateRefusedError) ? updateRefusedError : "Um erro ocorreu ao atualizar status para cancelar o Agendamento.");
        }
        else
        {
            return Result.Fail(HttpStatusCode.UnprocessableContent, "Não é possível aceitar o Agendamento");
        }

        return Result.Success(HttpStatusCode.NoContent);
    }

    public async Task<Result> UpdateScheduleAsync(UpdateScheduleRequestDto updateScheduleData, CancellationToken cancellationToken)
    {
        if (updateScheduleData.ScheduleTime <= DateTime.Now)
            return Result.Fail(HttpStatusCode.BadRequest, "Data do agendamento deve ser maior que a data atual");

        var foundSchedule = await this.GetScheduleByIdAsync(updateScheduleData.ScheduleId, cancellationToken);

        if (foundSchedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado");

        if (foundSchedule.DoctorId != updateScheduleData.DoctorId)
            return Result.Fail(HttpStatusCode.BadRequest, "Agendamento não pertence ao médico informado");

        var doctorSchedules = await this.GetScheduleByAsync(foundSchedule.DoctorId, foundSchedule.ScheduleTime, cancellationToken);
        foundSchedule.ScheduleTime = updateScheduleData.ScheduleTime;
        foundSchedule.Status = new EScheduleStatus[] { EScheduleStatus.PENDING_CONFIRMATION, EScheduleStatus.CONFIRMED }.Contains(foundSchedule.Status) ? EScheduleStatus.PENDING_CONFIRMATION : EScheduleStatus.AVAILABLE;

        if (doctorSchedules is not null)
        {
            var hasAnyOverlap = doctorSchedules.Select(x => foundSchedule.IsOverlappedBy(x)).Any(x => x);
            if (hasAnyOverlap)
                return Result.Fail(HttpStatusCode.BadRequest, "Data do agendamento em conflito com data(s) existente(s)");
        }

        if (await this._unitOfWork.ScheduleRepository.UpdateScheduleAsync(foundSchedule, cancellationToken) > 0)
            return Result.Success(HttpStatusCode.OK);
        else
            return Result.Fail(HttpStatusCode.InternalServerError, "Erro ao atualizar o agendamento");
    }

    public async Task<Result> ScheduleToPatientAsync(PatientScheduleRequestDto scheduleData, CancellationToken ct)
    {
        if (await _unitOfWork.ScheduleRepository.GetScheduleByIdAsync(scheduleData.ScheduleId, ct) is var foundSchedule && foundSchedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado");

        if (foundSchedule.ScheduleTime < DateTime.Now || foundSchedule.Status != Domain.Enum.EScheduleStatus.AVAILABLE)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento indisponível");

        foundSchedule.PatientId = scheduleData.PatientId;
        foundSchedule.Status = Domain.Enum.EScheduleStatus.PENDING_CONFIRMATION;

        if (await this._unitOfWork.ScheduleRepository.ScheduleToPatientAsync(foundSchedule, ct) > 0)
            return Result.Success(HttpStatusCode.OK);
        else
            return Result.Fail(HttpStatusCode.InternalServerError, "Error ao agendar consulta");
    }

    public async Task<Result<Domain.Models.Schedule>> GetByScheduleId(long scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await GetScheduleByIdAsync(scheduleId, cancellationToken);

        return schedule is null ?
            Result<Domain.Models.Schedule>.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado") :
            Result<Domain.Models.Schedule>.Success(HttpStatusCode.OK, schedule);
    }

    public async Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByPatientId(int patientId, CancellationToken cancellationToken)
    {
        var schedules = await GetSchedulesByPatientIdAsync(patientId, cancellationToken);

        return schedules is not null && schedules.Any() ?
            Result<IEnumerable<Domain.Models.Schedule>>.Success(HttpStatusCode.OK, schedules) :
            Result<IEnumerable<Domain.Models.Schedule>>.Fail(HttpStatusCode.NoContent, "Nenhum agendamento encontrado para o paciente");
    }

    public async Task<Result<IEnumerable<Domain.Models.Schedule>>> GetByDoctorId(int doctorId, CancellationToken cancellationToken)
    {
        var schedules = await GetScheduleByDoctorIdAsync(doctorId, cancellationToken);

        return schedules is not null && schedules.Any() ?
            Result<IEnumerable<Domain.Models.Schedule>>.Success(HttpStatusCode.OK, schedules) :
            Result<IEnumerable<Domain.Models.Schedule>>.Fail(HttpStatusCode.NoContent, "Nenhum agendamento encontrado para o médico");
    }

    public async Task<Result> CancelScheduleAsync(CancelScheduleRequestDto cancelData, CancellationToken ct)
    {
        if (await _unitOfWork.ScheduleRepository.GetScheduleByIdAsync(cancelData.ScheduleId, ct) is var foundSchedule && foundSchedule is null)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não encontrado");

        if (foundSchedule.ScheduleTime < DateTime.Now)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento já realizado");

        if (foundSchedule.PatientId != cancelData.PatientId)
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não pertence ao paciente informado");

        if (!new EScheduleStatus[] { EScheduleStatus.PENDING_CONFIRMATION, EScheduleStatus.CONFIRMED }.Contains(foundSchedule.Status))
            return Result.Fail(HttpStatusCode.NotFound, "Agendamento não reservado anteriormente, não pode ser cancelado");

        foundSchedule.Status = EScheduleStatus.CANCELED_BY_PATIENT;
        foundSchedule.CancelReason = cancelData.Reason;

        if (await this._unitOfWork.ScheduleRepository.CancelScheduleAsync(foundSchedule, ct) > 0)
            return Result.Success(HttpStatusCode.OK);
        else
            return Result.Fail(HttpStatusCode.InternalServerError, "Erro ao cancelar o agendamento");
    }

    #region Private methods:
    private static bool IsPassedTime(Domain.Models.Schedule schedule, DateTime reference)
        => schedule.ScheduleTime <= reference;
    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByAsync(int doctorId, DateTime scheduleTime, CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    private async Task<Domain.Models.Schedule?> GetScheduleByIdAsync(long scheduleId, CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetScheduleByIdAsync(scheduleId, cancellationToken);
    }
    private async Task<IEnumerable<Domain.Models.Schedule>> GetScheduleByDoctorIdAsync(int doctorId, CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetScheduleByDoctorIdAsync(doctorId, cancellationToken);
    }
    private async Task<IEnumerable<Domain.Models.Schedule>> GetSchedulesByPatientIdAsync(int patientId, CancellationToken cancellationToken)
    {
        return await this._unitOfWork.ScheduleRepository.GetSchedulesByPatientIdAsync(patientId, cancellationToken);
    }
    #endregion Private methods.
}